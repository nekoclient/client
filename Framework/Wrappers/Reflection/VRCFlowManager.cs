using System.Linq;
using System.Reflection;
using VRC.Core;

namespace NekoClient.Wrappers.Reflection
{
    public static class VRCFlowManagerWrappers
    {
        private static MethodInfo instanceMethod = null;
        private static MethodInfo hasConnectionErrorMethod = null;
        private static MethodInfo allowThirdPartyLoginMethod = null;
        private static MethodInfo commandLineMethod = null;
        private static MethodInfo destinationWorldMethod = null;
        private static MethodInfo destinationWorldIdMethod = null;
        private static MethodInfo hasAttemptedCachedLoginMethod = null;
        private static MethodInfo blockPlayerJoinsMethod = null;
        private static MethodInfo blockResetGameFlowMethod = null;

        public static VRCFlowManager Instance
            => (VRCFlowManager)instanceMethod.Invoke(null, null);

        public static bool HasConnectionError
            => (bool)hasConnectionErrorMethod.Invoke(Instance, null);

        public static bool AllowThirdPartyLogin
            => (bool)allowThirdPartyLoginMethod.Invoke(Instance, null);

        public static VRCFlowCommandLine CommandLine
            => (VRCFlowCommandLine)commandLineMethod.Invoke(Instance, null);

        public static ApiWorld DestinationWorld
            => (ApiWorld)destinationWorldMethod.Invoke(Instance, null);

        public static string DestinationWorldId
            => (string)destinationWorldIdMethod.Invoke(Instance, null);

        public static bool HasAttemptedCachedLogin
            => (bool)hasAttemptedCachedLoginMethod.Invoke(Instance, null);

        public static bool BlockPlayerJoins
            => (bool)blockPlayerJoinsMethod.Invoke(Instance, null);

        public static bool BlockResetGameFlow
            => (bool)blockResetGameFlowMethod.Invoke(Instance, null);

        internal static void Initialize()
        {
            PropertyInfo instanceInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Instance");
            instanceMethod = instanceInfo?.GetGetMethod();

            PropertyInfo hasConnectionErrorInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_HasConnectionError");
            hasConnectionErrorMethod = hasConnectionErrorInfo?.GetGetMethod();

            PropertyInfo allowThirdPartyLoginInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_AllowThirdPartyLogin");
            allowThirdPartyLoginMethod = allowThirdPartyLoginInfo?.GetGetMethod();

            PropertyInfo commandLineInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_CommandLine");
            commandLineMethod = commandLineInfo?.GetGetMethod();

            PropertyInfo destinationWorldInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_DestinationWorld");
            destinationWorldMethod = destinationWorldInfo?.GetGetMethod();

            PropertyInfo destinationWorldIdInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_DestinationWorldId");
            destinationWorldIdMethod = destinationWorldIdInfo?.GetGetMethod();

            PropertyInfo hasAttemptedCachedLoginInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_HasAttemptedCachedLogin");
            hasAttemptedCachedLoginMethod = hasAttemptedCachedLoginInfo?.GetGetMethod();

            PropertyInfo blockPlayerJoinsInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_BlockPlayerJoins");
            blockPlayerJoinsMethod = blockPlayerJoinsInfo?.GetGetMethod();

            PropertyInfo blockResetGameFlowInfo = typeof(VRCFlowManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_BlockResetGameFlow");
            blockResetGameFlowMethod = blockResetGameFlowInfo?.GetGetMethod();
        }
    }
}
