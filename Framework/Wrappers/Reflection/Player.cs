using System.Linq;
using System.Reflection;
using VRC;
using VRC.Core;
using VRCSDK2;
using Player = VRC.Player;

namespace NekoClient.Wrappers.Reflection
{
    public static class PlayerWrappers
    {
        private static MethodInfo userMethod = null;
        private static MethodInfo playerNetMethod = null;
        private static MethodInfo isLoadedMethod = null;
        private static MethodInfo isLocalMethod = null;
        private static MethodInfo isModeratorMethod = null;
        private static MethodInfo isVIPMethod = null;
        private static MethodInfo isSuperMethod = null;
        private static MethodInfo isTrustedMethod = null;
        private static MethodInfo isMasterMethod = null;
        private static MethodInfo isTalkingMethod = null;
        private static MethodInfo isValidUserMethod = null;
        private static MethodInfo isRoomAuthorMethod = null;
        private static MethodInfo isInstanceOwnerMethod = null;
        private static MethodInfo isFriendMethod = null;
        private static MethodInfo isFavoriteFriendMethod = null;
        private static MethodInfo isBlockedEitherWayMethod = null;
        internal static PropertyInfo photonPlayerInfo = null;
        private static MethodInfo photonPlayerMethod = null;
        private static MethodInfo userIdMethod = null;
        private static MethodInfo statusMethod = null;
        private static MethodInfo statusDescriptionMethod = null;
        private static MethodInfo playerApiMethod = null;

        public static APIUser ApiUser(this Player p)
            => (APIUser)userMethod.Invoke(p, null);

        public static PlayerNet PlayerNet(this Player p)
            => (PlayerNet)playerNetMethod.Invoke(p, null);

        public static bool IsLoaded(this Player p)
            => (bool)isLoadedMethod.Invoke(p, null);

        public static bool IsLocal(this Player p)
            => (bool)isLocalMethod.Invoke(p, null);

        public static bool IsModerator(this Player p)
            => (bool)isModeratorMethod.Invoke(p, null);

        public static bool IsVIP(this Player p)
            => (bool)isVIPMethod.Invoke(p, null);

        public static bool IsSuper(this Player p)
            => (bool)isSuperMethod.Invoke(p, null);

        public static bool IsTrusted(this Player p)
            => (bool)isTrustedMethod.Invoke(p, null);

        public static bool IsMaster(this Player p)
            => (bool)isMasterMethod.Invoke(p, null);

        public static bool IsTalking(this Player p)
            => (bool)isTalkingMethod.Invoke(p, null);

        public static bool IsValidUser(this Player p)
            => (bool)isValidUserMethod.Invoke(p, null);

        public static bool IsRoomAuthor(this Player p)
            => (bool)isRoomAuthorMethod.Invoke(p, null);

        public static bool IsInstanceOwner(this Player p)
            => (bool)isInstanceOwnerMethod.Invoke(p, null);

        public static bool IsFriend(this Player p)
            => (bool)isFriendMethod.Invoke(p, null);

        public static bool IsFavoriteFriend(this Player p)
            => (bool)isFavoriteFriendMethod.Invoke(p, null);

        public static bool IsBlockedEitherWay(this Player p)
            => (bool)isBlockedEitherWayMethod.Invoke(p, null);

        public static object PhotonPlayer(this Player p)
            => photonPlayerMethod.Invoke(p, null);

        public static string UserId(this Player p)
            => (string)userIdMethod.Invoke(p, null);

        public static string Status(this Player p)
            => (string)statusMethod.Invoke(p, null);

        public static string StatusDescription(this Player p)
            => (string)statusDescriptionMethod.Invoke(p, null);

        public static VRC_PlayerApi PlayerApi(this Player p)
            => (VRC_PlayerApi)playerApiMethod.Invoke(p, null);

        public static Player GetLocalPlayer()
            => PlayerManager.GetAllPlayers().FirstOrDefault(p => p.IsLocal());

        internal static void Initialize()
        {
            PropertyInfo userInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_user");
            userMethod = userInfo?.GetGetMethod();

            PropertyInfo playerNetInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_playerNet");
            playerNetMethod = playerNetInfo?.GetGetMethod();

            PropertyInfo isLoadedInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isLoaded");
            isLoadedMethod = isLoadedInfo?.GetGetMethod();

            PropertyInfo isLocalInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsLocal");
            isLocalMethod = isLocalInfo?.GetGetMethod();

            PropertyInfo isModeratorInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isModerator");
            isModeratorMethod = isModeratorInfo?.GetGetMethod();

            PropertyInfo isVIPInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isVIP");
            isVIPMethod = isVIPInfo?.GetGetMethod();

            PropertyInfo isSuperInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isSuper");
            isSuperMethod = isSuperInfo?.GetGetMethod();

            PropertyInfo isTrustedInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isTrusted");
            isTrustedMethod = isTrustedInfo?.GetGetMethod();

            PropertyInfo isMasterInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsMaster");
            isMasterMethod = isMasterInfo?.GetGetMethod();

            PropertyInfo isTalkingInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isTalking");
            isTalkingMethod = isTalkingInfo?.GetGetMethod();

            PropertyInfo isValidUserInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isValidUser");
            isValidUserMethod = isValidUserInfo?.GetGetMethod();

            PropertyInfo isRoomAuthorInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isRoomAuthor");
            isRoomAuthorMethod = isRoomAuthorInfo?.GetGetMethod();

            PropertyInfo isInstanceOwnerInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isInstanceOwner");
            isInstanceOwnerMethod = isInstanceOwnerInfo?.GetGetMethod();

            PropertyInfo isFriendInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isFriend");
            isFriendMethod = isFriendInfo?.GetGetMethod();

            PropertyInfo isFavoriteFriendInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isFavoriteFriend");
            isFavoriteFriendMethod = isFavoriteFriendInfo?.GetGetMethod();

            PropertyInfo isBlockedEitherWayInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isBlockedEitherWay");
            isBlockedEitherWayMethod = isBlockedEitherWayInfo?.GetGetMethod();

            photonPlayerInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_PhotonPlayer");
            photonPlayerMethod = photonPlayerInfo?.GetGetMethod();

            PropertyInfo userIdInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_userId");
            userIdMethod = userIdInfo?.GetGetMethod();

            PropertyInfo statusInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_status");
            statusMethod = statusInfo?.GetGetMethod();

            PropertyInfo statusDescriptionInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_statusDescription");
            statusDescriptionMethod = statusDescriptionInfo?.GetGetMethod();

            PropertyInfo playerApiInfo = typeof(Player).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_playerApi");
            playerApiMethod = playerApiInfo?.GetGetMethod();
        }
    }
}
 