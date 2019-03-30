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
using Steamworks;

namespace Patches
{
#if false
    class SteamIdPatches
    {
        public static void Perform(HarmonyInstance harmony)
        {
            PerformPatch("SteamId", () =>
            {
                harmony.Patch(typeof(SteamUser).GetMethod("GetSteamID"), GetPatch("SteamIsntRunning", typeof(SteamIdPatches)), null, null);
                VRCPlayer.SetNetworkProperties();
            });
        }

        private static bool SteamIsntRunning(ref ulong __result)
        {
            __result = 0UL;
            return false;
        }
    }
#endif
}