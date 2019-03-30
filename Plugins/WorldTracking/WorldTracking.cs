using NekoClient;
using NekoClient.Logging;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRC.Core;

namespace WorldTracking
{
    public class WorldTracking : PluginBase
    {
        public WorldTracking()
        {
            Tick += WorldTracking_Tick;
        }

        private class WorldInfo
        {
            public string Id { get; set; }
            public string Tags { get; set; }
        }

        private static List<WorldInfo> PreviousWorlds = new List<WorldInfo>();
        private float m_lastCheck = 0f;

        public static void ReturnToLastWorld()
        {
            WorldInfo last = PreviousWorlds.Last();

            VRCFlowManagerWrappers.Instance.EnterWorld(last.Id, last.Tags);

            PreviousWorlds.Clear();
        }

        private void WorldTracking_Tick()
        {
            if (RoomManagerBaseWrappers.InRoom && Time.time - m_lastCheck > 2f)
            {
                ApiWorldInstance currentRoom = RoomManagerBase.currentWorldInstance;

                WorldInfo currentInfo = new WorldInfo()
                {
                    Id = currentRoom.instanceWorld.id,
                    Tags = currentRoom.idWithTags
                };

                if (!PreviousWorlds.Any(w => w.Id == currentInfo.Id && w.Tags == currentInfo.Tags))
                {
                    Log.Debug($"Adding worldId {currentInfo.Id} with tags {currentInfo.Tags} to history");

                    PreviousWorlds.Add(currentInfo);
                }

                m_lastCheck = Time.time;
            }
        }
    }
}