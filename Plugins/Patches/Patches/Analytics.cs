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
using UnityEngine.Analytics;

namespace Patches
{
    class AnalyticsPatches
    {
        internal static void Perform(HarmonyInstance harmony)
        {
            HarmonyMethod patch = GetPatch("DontContinue", typeof(PatchesInternal));

            PerformPatch("Analytics#DeviceID", () =>
            {
                harmony.Patch(typeof(API).GetProperty("DeviceID").GetGetMethod(), GetPatch("CustomDeviceId", typeof(AnalyticsPatches)), null, null);
            });

            PerformPatch("Analytics#Save", () =>
            {
                harmony.Patch(typeof(ApiAnalyticEvent.EventInfo).GetMethod("Save"), patch, null, null);
            });

            PerformPatch("Analytics#LogEvent", () =>
            {
                harmony.Patch(typeof(AmplitudeWrapper)
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .First((MethodInfo m) => m.Name == "LogEvent" && m.GetParameters().Length == 4),
                    patch, null, null);
            });

            PerformPatch("Analytics#CustomEvent", () =>
            {
                harmony.Patch(typeof(UnityEngine.Analytics.Analytics).GetMethod("CustomEvent", new Type[]
                {
                    typeof(string),
                    typeof(IDictionary<string, object>)
                }), GetPatch("CustomEventPrefix", typeof(AnalyticsPatches)), null, null);
            });
        }

        private static bool CustomEventPrefix(ref AnalyticsResult __result)
        {
            __result = AnalyticsResult.Ok;

            return false;
        }

        private static string m_deviceId = "";

        private static bool CustomDeviceId(ref string __result)
        {
            if (!File.Exists("mac.txt"))
            {
                string newId = (from x in KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-61-{1}A-{2}-{3}{4}-1{5}", new object[]
                {
                        new System.Random().Next(0, 9),
                        new System.Random().Next(0, 9),
                        new System.Random().Next(0, 9),
                        new System.Random().Next(0, 9),
                        new System.Random().Next(0, 9),
                        new System.Random().Next(0, 9)
                })))
                select x.ToString("x2")).Aggregate((string x, string y) => x + y);

                File.WriteAllText("mac.txt", newId);

                __result = newId;
            }
            else
            {
                if (string.IsNullOrEmpty(m_deviceId))
                {
                    m_deviceId = File.ReadAllText("mac.txt");
                }

                __result = m_deviceId;
            }

            return false;
        }
    }
}