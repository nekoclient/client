using ExitGames.Client.Photon;
using Harmony;
using Helpers;
using NekoClient;
using NekoClient.Logging;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC;
using VRC.Core;
using VRCSDK2;

namespace DynamicBones
{
    public class DynamicBones : PluginBase
    {
        class BoneMap
        {
            public DynamicBone Bone { get; set; }
            public List<DynamicBoneCollider> Colliders { get; set; } = new List<DynamicBoneCollider>();
        }

        class AvatarMap
        {
            public List<BoneMap> ExtraCollisions { get; set; } = new List<BoneMap>();
        }

        static Dictionary<string, AvatarMap> m_map = new Dictionary<string, AvatarMap>();

        static void TrackAvatar(string avatarId, GameObject avatarObject)
        {
            Log.Info($"Tracking avatar {avatarId}");

            // get all of the avatar's bones and colliders
            List<DynamicBone> bones = avatarObject.GetComponentsInChildren<DynamicBone>(true).ToList();
            List<DynamicBoneCollider> colliders = avatarObject.GetComponentsInChildren<DynamicBoneCollider>(true).ToList();

            // store the default ones
            List<BoneMap> newBoneMap = new List<BoneMap>();

            bones.ForEach(bone => {
                newBoneMap.Add(new BoneMap()
                {
                    Bone = bone
                });
            });

            AvatarMap newMap = new AvatarMap()
            {
                ExtraCollisions = newBoneMap
            };

            // add the avatar to the map
            m_map.Add(avatarId, newMap);

            // make the avatar's colliders interact with other avatars
            UpdateCollisions(avatarId, colliders);
        }

        static void UpdateCollisions(string avatarId, List<DynamicBoneCollider> colliders)
        {
            // for every avatar in the map
            foreach (KeyValuePair<string, AvatarMap> kvp in m_map)
            {
                // if the avatar isn't the currently added one
                if (kvp.Key != avatarId)
                {
                    // add the colliders to the other avatars' bones
                    // and store the collider for reference later
                    foreach (BoneMap map in kvp.Value.ExtraCollisions)
                    {
                        colliders.ForEach(collider =>
                        {
                            map.Bone.m_Colliders.Add(collider);
                            map.Colliders.Add(collider);
                        });
                    }
                }
            }
        }

        static void RemoveAvatar(string avatarId)
        {
            Log.Info($"Removing avatar {avatarId}");

            AvatarMap currentMap = m_map[avatarId];

            // remove current avatar's colliders from everywhere
            currentMap.ExtraCollisions.ForEach(bone =>
            {
                bone.Colliders.ForEach(collider => bone.Bone.m_Colliders.Remove(collider));
            });

            m_map.Remove(avatarId);
        }

        private IEnumerator BonesLoop()
        {
            while (true)
            {
                yield return new WaitUntil(() => RoomManagerBaseWrappers.InRoom == true);

                Dictionary<string, GameObject> iteratedAvatars = new Dictionary<string, GameObject>();

                // get everyone's avatars
                foreach (VRC.Player p in PlayerManager.GetAllPlayers())
                {
                    // only iterate if the player is ready
                    if (p.vrcPlayer.Ready())
                    {
                        ApiAvatar apiAvatar = p.vrcPlayer.ApiAvatar();
                        GameObject avatar = p.vrcPlayer.avatarGameObject;

                        iteratedAvatars.Add(apiAvatar.id, avatar);
                    }
                }

                // if the current player list doesn't contain an avatar, remove it from the map
                List<string> keysToRemove = new List<string>();

                foreach (string key in m_map.Keys)
                {
                    if (!iteratedAvatars.Keys.Contains(key))
                    {
                        keysToRemove.Add(key);
                    }
                }

                keysToRemove.ForEach(key => RemoveAvatar(key));

                // add new avatars to the map
                foreach (KeyValuePair<string, GameObject> kvp in iteratedAvatars)
                {
                    if (!m_map.Keys.Contains(kvp.Key))
                    {
                        TrackAvatar(kvp.Key, kvp.Value);
                    }
                }

                yield return new WaitForSeconds(5);
            }
        }

        public DynamicBones()
        {
            Load += DynamicBones_Load;
        }

        private void DynamicBones_Load()
        {
            QueueCoroutine(BonesLoop());
        }
    }
}