using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NekoClient.Wrappers.Reflection
{
    public static class VRCUiManagerWrappers
    {
        private static MethodInfo instanceMethod = null;
        private static MethodInfo activeScreensMethod = null;
        private static MethodInfo popupsMethod = null;

        public static VRCUiManager Instance
            => (VRCUiManager)instanceMethod.Invoke(null, null);

        public static Dictionary<string, VRCUiPage> ActiveScreens
            => (Dictionary<string, VRCUiPage>)activeScreensMethod.Invoke(Instance, null);

        public static VRCUiPopupManager Popups
            => (VRCUiPopupManager)popupsMethod.Invoke(Instance, null);

        internal static void Initialize()
        {
            PropertyInfo instanceInfo = typeof(VRCUiManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Instance");
            instanceMethod = instanceInfo?.GetGetMethod();

            PropertyInfo activeScreensInfo = typeof(VRCUiManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ActiveScreens");
            activeScreensMethod = activeScreensInfo?.GetGetMethod();

            PropertyInfo popupsInfo = typeof(VRCUiManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_popups");
            popupsMethod = popupsInfo?.GetGetMethod();

        }
    }
}
