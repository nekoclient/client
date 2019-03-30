using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NekoClient.Wrappers.Reflection
{
    public static class VRCApplicationSetupWrappers
    {
        private static MethodInfo instanceMethod = null;

        public static VRCApplicationSetup Instance
            => (VRCApplicationSetup)instanceMethod.Invoke(null, null);

        internal static void Initialize()
        {
            PropertyInfo instanceInfo = typeof(VRCApplicationSetup).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Instance");
            instanceMethod = instanceInfo?.GetGetMethod();
        }
    }
}
