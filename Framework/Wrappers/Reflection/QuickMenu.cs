using System.Linq;
using System.Reflection;
using VRC.Core;

namespace NekoClient.Wrappers.Reflection
{
    public static class QuickMenuWrappers
    {
        private static MethodInfo instanceMethod = null;
        private static MethodInfo isActiveMethod = null;
        private static MethodInfo isOnRightHandMethod = null;
        private static MethodInfo selectedUserMethod = null;
        private static MethodInfo isSuperMethod = null;
        private static MethodInfo isModeratorMethod = null;
        private static MethodInfo isRoomAuthorMethod = null;
        private static MethodInfo isInstanceOwnerMethod = null;
        private static MethodInfo canRecvReqInviteMethod = null;

        public static QuickMenu Instance
            => (QuickMenu)instanceMethod.Invoke(null, null);

        public static bool IsActive
            => (bool)isActiveMethod.Invoke(Instance, null);

        public static bool IsOnRightHand
            => (bool)isOnRightHandMethod.Invoke(Instance, null);

        public static APIUser SelectedUser
            => (APIUser)selectedUserMethod.Invoke(Instance, null);

        public static bool IsSuper
            => (bool)isSuperMethod.Invoke(Instance, null);

        public static bool IsModerator
            => (bool)isModeratorMethod.Invoke(Instance, null);

        public static bool IsRoomAuthor
            => (bool)isRoomAuthorMethod.Invoke(Instance, null);

        public static bool IsInstanceOwner
            => (bool)isInstanceOwnerMethod.Invoke(Instance, null);

        public static bool CanRecvReqInvite
            => (bool)canRecvReqInviteMethod.Invoke(Instance, null);

        internal static void Initialize()
        {
            PropertyInfo instanceInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Instance");
            instanceMethod = instanceInfo?.GetGetMethod();

            PropertyInfo isActiveInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsActive");
            isActiveMethod = isActiveInfo?.GetGetMethod();

            PropertyInfo isOnRightHandInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsOnRightHand");
            isOnRightHandMethod = isOnRightHandInfo?.GetGetMethod();

            PropertyInfo selectedUserInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_SelectedUser");
            selectedUserMethod = selectedUserInfo?.GetGetMethod();

            PropertyInfo isSuperInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isSuper");
            isSuperMethod = isSuperInfo?.GetGetMethod();

            PropertyInfo isModeratorInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isModerator");
            isModeratorMethod = isModeratorInfo?.GetGetMethod();

            PropertyInfo isRoomAuthorInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isRoomAuthor");
            isRoomAuthorMethod = isRoomAuthorInfo?.GetGetMethod();

            PropertyInfo isInstanceOwnerInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isInstanceOwner");
            isInstanceOwnerMethod = isInstanceOwnerInfo?.GetGetMethod();

            PropertyInfo canRecvReqInviteInfo = typeof(QuickMenu).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_canRecvReqInvite");
            canRecvReqInviteMethod = canRecvReqInviteInfo?.GetGetMethod();
        }
    }
}
