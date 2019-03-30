using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Harmony;
using VRC.Core;
using AmplitudeSDKWrapper;
using System.Reflection;
using System.Collections.Generic;
using static Patches.Patches;
using System;
using ExitGames.Client.Photon;
using NekoClient.Logging;
using System.Collections;
using NekoClient;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using NekoClient.Wrappers.Reflection;

namespace Patches
{
    class ConnectionPatches
    {
        public static DateTime LastLogoutTime;

        public static void Perform(HarmonyInstance harmony)
        {
            // SLOW!!!
            Type networkManagerType = typeof(RoomManager).Assembly.GetType("NetworkManager");

            if (networkManagerType == null)
            {
                Log.Info("networkManagerType was null, not performing connection patches");
                return;
            }

            /*PerformPatch("Photon#Disconnected", () =>
            {
                harmony.Patch(networkManagerType.GetMethod("OnDisconnectedFromPhoton"), GetPatch("DisconnectedFromPhoton", typeof(ConnectionPatches)));
            });*/

            PerformPatch("Photon#PlayerJoined", () =>
            {
                harmony.Patch(networkManagerType.GetMethod("OnVRCPlayerJoined"), GetPatch("PlayerJoined", typeof(ConnectionPatches)));
            });
        }

        private static bool DisconnectedFromPhoton()
        {
            Log.Info("got DC'd from photon");

            if (LastLogoutTime != null && (DateTime.UtcNow - LastLogoutTime).TotalSeconds < 15)
            {
                Log.Info("Returning to previous world due to logout.");

                WorldTracking.WorldTracking.ReturnToLastWorld();

                return false;
            }

            return true;
        }

        private static bool PlayerJoined(VRC.Player __0)
        {
            if (__0.IsModerator())
            {
                VRCUiManagerWrappers.Instance.QueueHudMessage($"MODERATOR {__0.ApiUser().displayName} joined! returning to home!");
                VRCUiManagerWrappers.Instance.QueueHudMessage("");

                VRCFlowManagerWrappers.Instance.GoHome();

                return false;
            }

            return true;
        }
    }
}