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

namespace Patches
{
    class AntiKickPatches
    {
        public static void Perform(HarmonyInstance harmony)
        {
            PerformPatch("AntiKick#KickUserRPC", () =>
            {
                harmony.Patch(typeof(ModerationManager).GetMethod("KickUserRPC", ((BindingFlags)62)), GetPatch("KickUserPrefix", typeof(AntiKickPatches)), null, null);
            });

            PerformPatch("AntiKick#SelfCheckAndEnforceModerations", () =>
            {
                harmony.Patch(typeof(ModerationManager).GetMethod("SelfCheckAndEnforceModerations", ((BindingFlags)62)), GetPatch("ReturnFalseDontContinue", typeof(PatchesInternal)), null, null);
            });

            PerformPatch("AntiKick#RejoinWorld", () =>
            {
                harmony.Patch(typeof(ModerationManager).GetMethod("IsKickedFromWorld", ((BindingFlags)62)), GetPatch("ReturnFalseDontContinue", typeof(PatchesInternal)), null, null);
            });

            PerformPatch("AntiKick#IgnorePublicBan", () =>
            {
                harmony.Patch(typeof(ModerationManager).GetMethod("IsPublicOnlyBannedFromWorld", ((BindingFlags)62)), GetPatch("ReturnFalseDontContinue", typeof(PatchesInternal)), null, null);
            });
        }

        private static bool KickUserPrefix()
        {
            return false;
        }
    }
}
