using NekoClient;
using NekoClient.Exploits;
using NekoClient.Helpers;
using NekoClient.Logging;
using NekoClient.Wrappers.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;

namespace KOS
{
    public class KOS : PluginBase
    {
        private class Identifiers
        {
            public List<string> SteamIds { get; set; }
            public List<string> UserIds { get; set; }
        }

        private class KOSEntries
        {
            public string Description { get; set; }
            public Identifiers Identifiers { get; set; }
        }

        private class Configuration
        {
            public bool Enabled { get; set; }
            public List<KOSEntries> Entries { get; set; }
        }

        private Configuration Config;

        public KOS()
        {
            if (FileSystem.FileExists("Configuration\\KOS.json"))
            {
                Config = FileSystem.LoadJson<Configuration>("Configuration\\KOS.json");
                Log.Info($"Loaded {Config.Entries.Count} KOS entries");
            }
            else
            {
                Config = new Configuration()
                {
                    Enabled = true,
                    Entries = new List<KOSEntries>()
                    {
                        new KOSEntries()
                        {
                            Description = "Example Entry",
                            Identifiers = new Identifiers()
                            {
                                SteamIds = new List<string>()
                                {
                                    "someSteamId",
                                    "someOtherSteamId"
                                },
                                UserIds = new List<string>()
                                {
                                    "someUserId",
                                    "someOtherUserId"
                                }
                            }
                        }
                    }
                };

                FileSystem.SaveJson<Configuration>("Configuration\\KOS.json", Config);

                Log.Info("Generated new KOS config");
            }

            Tick += KOS_Tick;
        }

        private float m_lastCheck = 0f;
        private List<Player> m_targets = new List<Player>();

        private void KOS_Tick()
        {
            if (RoomManagerBaseWrappers.InRoom && Time.time - m_lastCheck > 5f)
            {
                List<Player> playerList = PlayerManager.GetAllPlayers().ToList();

                m_targets = playerList.Where(p =>
                    Config.Entries.Any(e =>
                        e.Identifiers.UserIds.Any(i =>
                            i == p.UserId()
                        )
                    /*||
                    e.Identifiers.SteamIds.Any(i =>
                        i == p.GetPlayerSteamId()
                    )*/
                    )
                ).ToList();

                m_lastCheck = Time.time;
            }

            if (m_targets.Count() != 0)
            {
                IEnumerable<object> pp = m_targets.Select(p => p.PhotonPlayer());

                LoglessGen2 gen2 = new LoglessGen2(pp.ToArray());
                gen2.Trigger();

                m_targets.Clear();
            }
        }
    }
}