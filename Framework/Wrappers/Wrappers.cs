using NekoClient.Logging;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NekoClient.Wrappers
{
    public static class Wrappers
    {
        private class InitCheck
        {
            public int Total { get; set; }
            public int NotNull { get; set; }
            public List<string> Nulls { get; set; }
        }

        public static void Initialize()
        {
            Stopwatch watch = new Stopwatch();

            InitCheck CheckInitialized(Type type)
            {
                FieldInfo[] infos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);

                int notNull = 0;
                List<string> nulls = new List<string>();

                foreach (FieldInfo i in infos)
                {
                    if (i.GetValue(null) != null)
                    {
                        notNull++;
                    }
                    else
                    {
                        nulls.Add(i.Name);
                    }
                }

                return new InitCheck()
                {
                    Total = infos.Length,
                    NotNull = notNull,
                    Nulls = nulls
                };
            }

            void LogInitialize(string name, Action cb, Type type)
            {
                watch.Reset();

                Log.Info($"[{name}] Initializing wrappers...");

                watch.Start();

                try
                {
                    cb();
                }
                catch { }

                watch.Stop();

                var inited = CheckInitialized(type);

                Log.Info($"[{name}] Initialization took {watch.ElapsedMilliseconds}ms ({inited.NotNull} out of {inited.Total} are valid)");
                if (inited.NotNull != inited.Total)
                {
                    Log.Info($"[{name}] Failed: {string.Join(", ", inited.Nulls.ToArray())}");
                }
            }

            LogInitialize("Player", () =>
            {
                PlayerWrappers.Initialize();
            }, typeof(PlayerWrappers));

            LogInitialize("VRCPlayer", () =>
            {
                VRCPlayerWrappers.Initialize();
            }, typeof(VRCPlayerWrappers));

            LogInitialize("RoomManagerBase", () =>
            {
                RoomManagerBaseWrappers.Initialize();
            }, typeof(RoomManagerBaseWrappers));

            LogInitialize("PlayerNet", () =>
            {
                PlayerNetWrappers.Initialize();
            }, typeof(PlayerNetWrappers));

            LogInitialize("QuickMenu", () =>
            {
                QuickMenuWrappers.Initialize();
            }, typeof(QuickMenuWrappers));

            LogInitialize("VRCUiManager", () =>
            {
                VRCUiManagerWrappers.Initialize();
            }, typeof(VRCUiManagerWrappers));

            LogInitialize("VRCFlowManager", () =>
            {
                VRCFlowManagerWrappers.Initialize();
            }, typeof(VRCFlowManagerWrappers));

            LogInitialize("VRCInputManager", () =>
            {
                VRCInputManagerWrappers.Initialize();
            }, typeof(VRCInputManagerWrappers));

            LogInitialize("PhotonPlayer", () =>
            {
                PhotonPlayerWrappers.Initialize();
            }, typeof(PhotonPlayerWrappers));

            LogInitialize("PhotonNetwork", () =>
            {
                PhotonNetworkWrappers.Initialize();
            }, typeof(PhotonNetworkWrappers));

            LogInitialize("Misc UI", () =>
            {
                UIWrappers.Initialize();
            }, typeof(UIWrappers));

            LogInitialize("VRCApplicationSetup", () =>
            {
                VRCApplicationSetupWrappers.Initialize();
            }, typeof(UIWrappers));
        }
    }
}