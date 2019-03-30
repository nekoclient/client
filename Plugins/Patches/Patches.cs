using AmplitudeSDKWrapper;
using Harmony;
using NekoClient;
using NekoClient.Logging;
using System;
using System.Reflection;

namespace Patches
{
    public class Patches : PluginBase
    {
        internal static HarmonyMethod GetPatch(string name, Type type)
                => new HarmonyMethod(type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));

        internal static void PerformPatch(string patchName, Action action)
        {
            try
            {
                Log.Info($"Performing {patchName} patch...");
                action();
                Log.Info($"{patchName} patch succeeded!");
            }
            catch (Exception e)
            {
                Log.Info($"Failed {patchName} patch - did the game update? (Exception: {e.InnerException.Message})");
            }
        }

        public Patches()
        {
            Load += Patches_Load;
        }

        private void Patches_Load()
        {
            Log.Info("Loading patches");
            HarmonyInstance harmony = HarmonyInstance.Create("NekoClient.VRChat.Patches");

            AnalyticsPatches.Perform(harmony);
            AntiKickPatches.Perform(harmony);
            NoBlockPatches.Perform(harmony);
            AvatarPatches.Perform(harmony);
            LogoutPatches.Perform(harmony);
            FavouritePatches.Perform(harmony);
            //SteamIdPatches.Perform(harmony);
            ConnectionPatches.Perform(harmony);
        }

        internal static class PatchesInternal
        {
            private static bool DontContinue()
            {
                return false;
            }

            private static bool ReturnFalseDontContinue(bool __result)
            {
                __result = false;
                return false;
            }
        }
    }
}
