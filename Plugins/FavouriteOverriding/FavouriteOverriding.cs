using NekoClient;
using NekoClient.Helpers;
using NekoClient.Logging;
using NekoClient.Wrappers.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;
using VRC.UI;

namespace FavouriteOverriding
{
    /* at the top of the functions
    ApiAvatar.FetchList() =>
        if (areFavorites)
        {
            List<ApiAvatar> avatars = new List<ApiAvatar>();

            foreach (string id in FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars())
            {
                ApiModelContainer<ApiAvatar> res = new ApiModelContainer<ApiAvatar>
                {
                    OnSuccess = delegate(ApiContainer c)
                    {
                        avatars.Add(c.Model as ApiAvatar);

                        if (id == FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars()[FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars().Count - 1])
                        {
                            successCallback(avatars);
                        }
                    },
                    OnError = delegate(ApiContainer c)
                    {
                        Logger.Log("Could not fetch avatar with error - " + c.Error, DebugLevel.Always, null);
                    }
                };

                API.SendRequest("avatars/" + id, HTTPMethods.Get, res, null, true, true);
            }

            return;
        }

    ApiFavorite.FetchFavouriteIds() =>
        if (favoriteType.value == "avatar")
        {
            successCallback(FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars());
            return;
        }

    ApiFavorite.AddFavorite(string id) =>
        if (favoriteType.value == "avatar")
        {
            FavouriteOverriding.FavouriteOverriding.AddFavourite(objectId);
            successCallback();
            return;
        }

    ApiFavorite.RemoveFavorite(...) =>
        if (objectId.StartsWith("avtr_"))
        {
            FavouriteOverriding.FavouriteOverriding.RemoveFavourite(objectId);
        }
    */

    public class FavouriteOverriding : PluginBase
    {
        private static List<string> m_favouriteAvatars = null;

        private static FileSystem m_fileSystem = new FileSystem("NekoClient\\Configuration");

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            Tick += FavouriteOverriding_Tick;
        }

        public FavouriteOverriding()
        {
            Load += FavouriteOverriding_Load;
        }

        private void FavouriteOverriding_Load()
        {
            try
            {
                m_favouriteAvatars = m_fileSystem.LoadJson<List<string>>("FavouriteAvatars.json");
            }
            catch
            {
                m_favouriteAvatars = new List<string>();
            }

            // remove duplicates
            m_favouriteAvatars = m_favouriteAvatars.Distinct().ToList();

            QueueCoroutine(DelayedStart());
        }

        private void FavouriteOverriding_Tick()
        {
            if (!RoomManagerBaseWrappers.InRoom)
            {
                return;
            }

            try
            {
                PageAvatar pageAvatar = VRCUiManagerWrappers.Instance.GetPage("UserInterface/MenuContent/Screens/Avatar") as PageAvatar;

                if (pageAvatar != null && pageAvatar.GetShown())
                {
                    if (pageAvatar.favoriteButton == null)
                    {
                        return;
                    }

                    if (!pageAvatar.favoriteButton.enabled)
                    {
                        pageAvatar.favoriteButton.enabled = true;
                    }
                }
            }
            catch { }
        }

        private static void SaveAvatarList()
        {
            m_fileSystem.SaveJson("FavouriteAvatars.json", m_favouriteAvatars);
        }

        public static List<string> GetFavouriteAvatars()
        {
            Log.Debug($"returning {m_favouriteAvatars.Count} favourite avatars");

            if (m_favouriteAvatars != null)
            {
                return m_favouriteAvatars;
            }

            return new List<string>();
        }

        public static void AddFavourite(string avatarId)
        {
            Log.Info($"Favouriting avatar {avatarId}");

            if (!m_favouriteAvatars.Contains(avatarId))
            {
                m_favouriteAvatars.Add(avatarId);

                SaveAvatarList();
            }
        }

        public static void RemoveFavourite(string avatarId)
        {
            Log.Info($"Removing favourite {avatarId}");

            if (m_favouriteAvatars.Contains(avatarId))
            {
                m_favouriteAvatars.Remove(avatarId);
                SaveAvatarList();
            }
        }
    }
}