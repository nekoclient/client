using Harmony;
using VRC.Core;
using System.Reflection;
using System.Collections.Generic;
using static Patches.Patches;
using System;
using VRC.Core.BestHTTP;

namespace Patches
{
    class FavouritePatches
    {
        public static void Perform(HarmonyInstance harmony)
        {
            PerformPatch("Favourites#FetchList", () =>
            {
                harmony.Patch(typeof(ApiAvatar).GetMethod("FetchList", (BindingFlags)62), GetPatch("FetchList", typeof(FavouritePatches)));
            });

            PerformPatch("Favourites#FetchIds", () =>
            {
                harmony.Patch(typeof(APIUser).GetMethod("FetchFavoriteAvatars", (BindingFlags)62), GetPatch("FetchFavouriteAvatars", typeof(FavouritePatches)));
            });

            PerformPatch("Favourites#Add", () =>
            {
                harmony.Patch(typeof(ApiGroup).GetMethod("AddToGroup", (BindingFlags)62), GetPatch("AddFavouriteAvatar", typeof(FavouritePatches)));
            });

            PerformPatch("Favourites#Remove", () =>
            {
                harmony.Patch(typeof(ApiGroup).GetMethod("RemoveFromGroup", (BindingFlags)62), GetPatch("RemoveFavouriteAvatar", typeof(FavouritePatches)));
            });
        }

        private static bool FetchList(bool areFavorites, Action<List<ApiAvatar>> successCallback)
        {
            if (areFavorites)
            {
                List<ApiAvatar> avatars = new List<ApiAvatar>();

                foreach (string id in FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars())
                {
                    ApiModelContainer<ApiAvatar> res = new ApiModelContainer<ApiAvatar>
                    {
                        OnSuccess = delegate (ApiContainer c)
                        {
                            avatars.Add(c.Model as ApiAvatar);

                            if (id == FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars()[FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars().Count - 1])
                            {
                                successCallback(avatars);
                            }
                        },
                        OnError = delegate (ApiContainer c)
                        {
                            Logger.Log("Could not fetch avatar with error - " + c.Error, DebugLevel.Always, null);
                        }
                    };

                    API.SendRequest("avatars/" + id, HTTPMethods.Get, res, null, true, true);
                }

                return false;
            }

            return true;
        }

        private static bool FetchFavouriteAvatars(Action successCallback)
        {
            APIUser.CurrentUser.favoriteAvatarIds = FavouriteOverriding.FavouriteOverriding.GetFavouriteAvatars();
            APIUser.CurrentUser.hasFetchedFavoriteAvatars = true;

            successCallback();

            return false;
        }

        private static bool AddFavouriteAvatar(string objectId, ApiGroup.GroupType groupType)
        {
            if (groupType == ApiGroup.GroupType.Avatar)
            {
                // TODO: refresh favorites list!

                FavouriteOverriding.FavouriteOverriding.AddFavourite(objectId);
                return false;
            }

            return true;
        }

        private static bool RemoveFavouriteAvatar(string objectId)
        {
            FavouriteOverriding.FavouriteOverriding.RemoveFavourite(objectId);

            return false;
        }
    }
}