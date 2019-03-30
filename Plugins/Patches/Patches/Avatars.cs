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
using VRCSDK2;
using UnityEngine;

namespace Patches
{
    class AvatarPatches
    {
        public static void Perform(HarmonyInstance harmony)
        {
            PerformPatch("AvatarAnti", () =>
            {
                harmony.Patch(typeof(AvatarValidation).GetMethod("RemoveCameras"), GetPatch("RemoveCamerasPrefix", typeof(AvatarPatches)), null, null);
            });
        }

        private static bool RemoveCamerasPrefix(GameObject currentAvatar, bool localPlayer, bool friend)
        {
            AvatarAnti.AvatarAnti.ScanAvatar(currentAvatar, localPlayer, friend);

            return true;
        }
    }
}