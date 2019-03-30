using NekoClient;
using NekoClient.Helpers;
using NekoClient.Logging;
using NekoClient.UI;
using NekoClient.Wrappers.Reflection;
using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEngine;
using VRC;
using VRC.Core;

namespace PlayerLog
{
    public class PlayerLog : PluginBase
    {
        private GameObject m_uiDump;
        private FileSystem m_fileSystem;

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            m_uiDump = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetPlayerDetailsBase(), VrcOverrideUI.GetPlayerDetailsBase().transform.parent.gameObject, "DUMP", () =>
            {
                APIUser target = QuickMenuWrappers.SelectedUser;
                Player player = PlayerManager.GetPlayer(target.id);

                QueueCoroutine(DumpPlayer(player));
            }, new[] { Vector2.left, Vector2.down });
        }

        private IEnumerator DumpPlayer(Player player)
        {
            var now = DateTime.Now;
            string timeFormat = $"{now.Year}_{now.Month}_{now.Day}_{now.Hour}_{now.Minute}_{now.Second}";

            void AppendToOutput(string prefix, string value, int indent = 2)
            {
                string output = "";

                for (int i = 0; i < indent; i++)
                {
                    output += "\t";
                }

                if (prefix == "")
                {
                    output += value;
                }
                else
                {
                    output += $"{prefix}: {value}";
                }

                output += "\n";

                m_fileSystem.SaveText($"{timeFormat} {player.ApiUser().displayName}.txt", output, true);
            }

            AppendToOutput("", "[[", 0);

            AppendToOutput("", "USER", 1);
            AppendToOutput("Display Name", player.ApiUser().displayName);
            AppendToOutput("Login Name", player.ApiUser().username);
            AppendToOutput("Id", player.ApiUser().id);
            AppendToOutput("SteamId", player.vrcPlayer.SteamUserIdULong().ToString());
            AppendToOutput("Status", player.ApiUser().statusDescription);

            if (player.vrcPlayer.SteamUserIdULong() != 0UL)
            {
                CSteamID id = new CSteamID(player.vrcPlayer.SteamUserIdULong());

                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);

                int version = VRCApplicationSetupWrappers.Instance.GetGameServerVersion().GetHashCode();
                writer.Write(version);
                writer.Write(0);

                byte[] data = ms.ToArray();

                writer.Close();
                ms.Close();

                SteamNetworking.SendP2PPacket(id, data, (uint)data.Length, EP2PSend.k_EP2PSendUnreliable);

                P2PSessionState_t state = default(P2PSessionState_t);

                if (SteamNetworking.GetP2PSessionState(id, out state))
                {
                    Log.Debug("Waiting for connection to be established...");

                    while (state.m_bConnecting == 1)
                    {
                        Log.Debug($"state: m_bConnecting: {state.m_bConnecting}, m_bConnectionActive: {state.m_bConnectionActive}, m_eP2PSessionError: {state.m_eP2PSessionError}");
                        Log.Debug($"m_bUsingRelay: {state.m_bUsingRelay}, m_nBytesQueuedForSend: {state.m_nBytesQueuedForSend}, m_nPacketsQueuedForSend: {state.m_nPacketsQueuedForSend}");
                        Log.Debug($"m_nRemoteIP: {state.m_nRemoteIP}, m_nRemotePort: {state.m_nRemotePort}");

                        yield return new WaitForSeconds(1f);
                    }

                    Log.Debug("Connection established, retreiving ip!");

                    string IpToStr(uint ip)
                    {
                        return new IPAddress(new byte[]
                        {
                        (byte)(ip >> 24 & 255u),
                        (byte)(ip >> 16 & 255u),
                        (byte)(ip >> 8 & 255u),
                        (byte)(ip & 255u)
                        }).ToString();
                    }

                    string relay = (state.m_bUsingRelay == 1) ? "(STEAM RELAY)" : "";

                    AppendToOutput($"IP {relay}", IpToStr(state.m_nRemoteIP));
                }
            }

            ApiAvatar avatar = player.vrcPlayer.ApiAvatar();

            AppendToOutput("", "AVATAR", 1);
            AppendToOutput("Name", avatar.name);
            AppendToOutput("Author", avatar.authorName);
            AppendToOutput("Status", avatar.releaseStatus);
            AppendToOutput("VRCA", avatar.assetUrl);

            AppendToOutput("", "]]", 0);
        }

        private bool m_steamInitialized = false;

        void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback)
        {
            Log.Debug("[" + P2PSessionConnectFail_t.k_iCallback + " - P2PSessionConnectFail] - " + pCallback.m_steamIDRemote + " -- " + pCallback.m_eP2PSessionError);
        }

        void OnSocketStatusCallback(SocketStatusCallback_t pCallback)
        {
            Log.Debug("[" + SocketStatusCallback_t.k_iCallback + " - SocketStatusCallback] - " + pCallback.m_hSocket + " -- " + pCallback.m_hListenSocket + " -- " + pCallback.m_steamIDRemote + " -- " + pCallback.m_eSNetSocketState);
        }

        void OnP2PSessionRequest(P2PSessionRequest_t pCallback)
        {
            SteamNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);
        }

        public PlayerLog()
        {
            Load += PlayerLog_Load;
            Unload += PlayerLog_Unload;
        }

        private void PlayerLog_Load()
        {
            m_fileSystem = new FileSystem("NekoClient\\PlayerDump");
            m_fileSystem.Format = FileSystem.Formats.TimePrefix;

            Log.Debug("Initializing SteamAPI");
            m_steamInitialized = SteamAPI.Init();
            Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
            Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
            Callback<SocketStatusCallback_t>.Create(OnSocketStatusCallback);
            Log.Debug($"SteamAPI state: {m_steamInitialized}");

            QueueCoroutine(DelayedStart());

            if (m_steamInitialized)
            {
                Tick += PlayerLog_Tick;
            }
        }

        private void PlayerLog_Tick()
        {
            SteamAPI.RunCallbacks();
        }

        private void PlayerLog_Unload()
        {
            UnityEngine.Object.DestroyImmediate(m_uiDump);
        }
    }
}