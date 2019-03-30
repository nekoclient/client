using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NekoClient.Wrappers.Reflection
{
    public static class RoomManagerBaseWrappers
    {
        private static MethodInfo inRoomMethod = null;
        private static MethodInfo currentAuthorIdMethod = null;
        private static MethodInfo currentOwnerIdMethod = null;
        private static MethodInfo metadataMethod = null;
        private static MethodInfo timeSinceEnteredRoomMethod = null;

        public static bool InRoom
            => (bool)inRoomMethod.Invoke(null, null);

        public static string CurrentAuthorId
            => (string)currentAuthorIdMethod.Invoke(null, null);

        public static string CurrentOwnerId
            => (string)currentOwnerIdMethod.Invoke(null, null);

        public static Dictionary<string, object> Metadata
            => (Dictionary<string, object>)metadataMethod.Invoke(null, null);

        public static float TimeSinceEnteredRoom
            => (float)timeSinceEnteredRoomMethod.Invoke(null, null);

        internal static void Initialize()
        {
            PropertyInfo inRoomInfo = typeof(RoomManagerBase).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_inRoom");
            inRoomMethod = inRoomInfo?.GetGetMethod();

            PropertyInfo currentAuthorIdInfo = typeof(RoomManagerBase).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_currentAuthorId");
            currentAuthorIdMethod = currentAuthorIdInfo?.GetGetMethod();

            PropertyInfo currentOwnerIdInfo = typeof(RoomManagerBase).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_currentOwnerId");
            currentOwnerIdMethod = currentOwnerIdInfo?.GetGetMethod();

            PropertyInfo metadataInfo = typeof(RoomManagerBase).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_metadata");
            metadataMethod = metadataInfo?.GetGetMethod();

            PropertyInfo timeSinceEnteredRoomInfo = typeof(RoomManagerBase).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_TimeSinceEnteredRoom");
            timeSinceEnteredRoomMethod = timeSinceEnteredRoomInfo?.GetGetMethod();
        }
    }
}