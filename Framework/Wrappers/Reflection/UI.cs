using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NekoClient.Wrappers.Reflection
{
    public static class UIWrappers
    {
        private static MethodInfo vrcUiPageTabManagerInstanceMethod = null;

        public static VRCUiPageTabManager VRCUiPageTabManagerInstance
            => (VRCUiPageTabManager)vrcUiPageTabManagerInstanceMethod.Invoke(null, null);

        internal static void Initialize()
        {
            PropertyInfo instanceInfo = typeof(VRCUiPageTabManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Instance");
            vrcUiPageTabManagerInstanceMethod = instanceInfo?.GetGetMethod();
        }
    }
}
