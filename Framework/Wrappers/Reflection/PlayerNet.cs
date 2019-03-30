using System.Linq;
using System.Reflection;
using VRC;

namespace NekoClient.Wrappers.Reflection
{
    internal static class PlayerNetWrappers
    {
        private static MethodInfo playerMethod = null;
        private static MethodInfo playerNetMethod = null;
        private static MethodInfo wasRecentlyDiscontinuousMethod = null;
        private static MethodInfo pingMethod = null;
        private static MethodInfo pingVarianceMethod = null;
        private static MethodInfo transitTimeAverageMSMethod = null;
        private static MethodInfo connectionQualityMethod = null;
        private static MethodInfo connectionDisparityMethod = null;

        internal static void Initialize()
        {
            PropertyInfo playerInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_player");
            playerMethod = playerInfo?.GetGetMethod();

            PropertyInfo playerNetInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_playerNet");
            playerNetMethod = playerNetInfo?.GetGetMethod();

            PropertyInfo wasRecentlyDiscontinuousInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_WasRecentlyDiscontinuous");
            wasRecentlyDiscontinuousMethod = wasRecentlyDiscontinuousInfo?.GetGetMethod();

            PropertyInfo pingInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Ping");
            pingMethod = pingInfo?.GetGetMethod();

            PropertyInfo pingVarianceInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_PingVariance");
            pingVarianceMethod = pingVarianceInfo?.GetGetMethod();

            PropertyInfo transitTimeAverageMSInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_TransitTimeAverageMS");
            transitTimeAverageMSMethod = transitTimeAverageMSInfo?.GetGetMethod();

            PropertyInfo connectionQualityInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ConnectionQuality");
            connectionQualityMethod = connectionQualityInfo?.GetGetMethod();

            PropertyInfo connectionDisparityInfo = typeof(PlayerNet).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ConnectionDisparity");
            connectionDisparityMethod = connectionDisparityInfo?.GetGetMethod();
        }
    }
}