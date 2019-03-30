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
    class NoBlockPatches
    {
        public static void Perform(HarmonyInstance harmony)
        {
            PerformPatch("NoBlock", () =>
            {
                harmony.Patch(typeof(ModerationManager).GetMethod("IsBlockedEitherWay"), GetPatch("ReturnFalseDontContinue", typeof(PatchesInternal)), null, null);
            });
        }
    }
}