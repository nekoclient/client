using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NekoClient.Wrappers.Reflection
{
    public static class VRCInputManagerWrappers
    {
        private static MethodInfo showTooltipsMethod = null;
        private static MethodInfo personalSpaceMethod = null;
        private static MethodInfo voicePrioritizationMethod = null;
        private static MethodInfo legacyGraspMethod = null;
        private static MethodInfo thirdPersonRotationMethod = null;
        private static MethodInfo comfortTurningMethod = null;
        private static MethodInfo headLookWalkMethod = null;
        private static MethodInfo talkToggleMethod = null;
        private static MethodInfo talkDefaultOnMethod = null;
        private static MethodInfo micDeviceNameMethod = null;
        private static MethodInfo micLevelVrMethod = null;
        private static MethodInfo micLevelDeskMethod = null;
        private static MethodInfo locomotionMethodMethod = null;
        private static MethodInfo viveAdvancedMethod = null;
        private static MethodInfo invertedMouseMethod = null;
        private static MethodInfo desktopReticleMethod = null;
        private static MethodInfo allowAvatarCopyingMethod = null;
        private static MethodInfo skipGoButtonInLoadMethod = null;
        private static MethodInfo mouseSensitivityMethod = null;
        private static MethodInfo safetyLevelMethod = null;
        private static MethodInfo showSocialRankMethod = null;
        private static MethodInfo joystickInputNamesMethod = null;
        private static MethodInfo lastInputMethodMethod = null;

        /*private static MethodInfo showTooltipsMethod = null;
        private static MethodInfo personalSpaceMethod = null;
        private static MethodInfo voicePrioritizationMethod = null;
        private static MethodInfo legacyGraspMethod = null;
        private static MethodInfo thirdPersonRotationMethod = null;
        private static MethodInfo comfortTurningMethod = null;
        private static MethodInfo headLookWalkMethod = null;
        private static MethodInfo talkToggleMethod = null;
        private static MethodInfo talkDefaultOnMethod = null;
        private static MethodInfo micDeviceNameMethod = null;
        private static MethodInfo micLevelVrMethod = null;
        private static MethodInfo micLevelDeskMethod = null;
        private static MethodInfo locomotionMethodMethod = null;
        private static MethodInfo viveAdvancedMethod = null;
        private static MethodInfo invertedMouseMethod = null;
        private static MethodInfo desktopReticleMethod = null;
        private static MethodInfo allowAvatarCopyingMethod = null;
        private static MethodInfo skipGoButtonInLoadMethod = null;
        private static MethodInfo mouseSensitivityMethod = null;
        private static MethodInfo safetyLevelMethod = null;
        private static MethodInfo showSocialRankMethod = null;
        private static MethodInfo joystickInputNamesMethod = null;*/

        public static InputMethod LastInputMethod
            => (InputMethod)lastInputMethodMethod.Invoke(null, null);

        public enum InputMethod
        {
            Keyboard,
            Mouse,
            Controller,
            Gaze,
            Vive,
            ViveAdvanced,
            Oculus,
            Daydream,
            Wave,
            Hydra,
            Count
        }

        internal static void Initialize()
        {
            PropertyInfo showTooltipsInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_showTooltips");
            showTooltipsMethod = showTooltipsInfo?.GetGetMethod();

            PropertyInfo personalSpaceInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_personalSpace");
            personalSpaceMethod = personalSpaceInfo?.GetGetMethod();

            PropertyInfo voicePrioritizationInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_voicePrioritization");
            voicePrioritizationMethod = voicePrioritizationInfo?.GetGetMethod();

            PropertyInfo legacyGraspInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_legacyGrasp");
            legacyGraspMethod = legacyGraspInfo?.GetGetMethod();

            PropertyInfo thirdPersonRotationInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_thirdPersonRotation");
            thirdPersonRotationMethod = thirdPersonRotationInfo?.GetGetMethod();

            PropertyInfo comfortTurningInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_comfortTurning");
            comfortTurningMethod = comfortTurningInfo?.GetGetMethod();

            PropertyInfo headLookWalkInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_headLookWalk");
            headLookWalkMethod = headLookWalkInfo?.GetGetMethod();

            PropertyInfo talkToggleInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_talkToggle");
            talkToggleMethod = talkToggleInfo?.GetGetMethod();

            PropertyInfo talkDefaultOnInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_talkDefaultOn");
            talkDefaultOnMethod = talkDefaultOnInfo?.GetGetMethod();

            PropertyInfo micDeviceNameInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_micDeviceName");
            micDeviceNameMethod = micDeviceNameInfo?.GetGetMethod();

            PropertyInfo micLevelVrInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_micLevelVr");
            micLevelVrMethod = micLevelVrInfo?.GetGetMethod();

            PropertyInfo micLevelDeskInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_micLevelDesk");
            micLevelDeskMethod = micLevelDeskInfo?.GetGetMethod();

            PropertyInfo locomotionMethodInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_locomotionMethod");
            locomotionMethodMethod = locomotionMethodInfo?.GetGetMethod();

            PropertyInfo viveAdvancedInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_viveAdvanced");
            viveAdvancedMethod = viveAdvancedInfo?.GetGetMethod();

            PropertyInfo invertedMouseInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_invertedMouse");
            invertedMouseMethod = invertedMouseInfo?.GetGetMethod();

            PropertyInfo desktopReticleInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_desktopReticle");
            desktopReticleMethod = desktopReticleInfo?.GetGetMethod();

            PropertyInfo allowAvatarCopyingInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_allowAvatarCopying");
            allowAvatarCopyingMethod = allowAvatarCopyingInfo?.GetGetMethod();

            PropertyInfo skipGoButtonInLoadInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_skipGoButtonInLoad");
            skipGoButtonInLoadMethod = skipGoButtonInLoadInfo?.GetGetMethod();

            PropertyInfo mouseSensitivityInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_mouseSensitivity");
            mouseSensitivityMethod = mouseSensitivityInfo?.GetGetMethod();

            PropertyInfo safetyLevelInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_safetyLevel");
            safetyLevelMethod = safetyLevelInfo?.GetGetMethod();

            PropertyInfo showSocialRankInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_showSocialRank");
            showSocialRankMethod = showSocialRankInfo?.GetGetMethod();

            PropertyInfo joystickInputNamesInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_JoystickInputNames");
            joystickInputNamesMethod = joystickInputNamesInfo?.GetGetMethod();

            PropertyInfo lastInputMethodInfo = typeof(VRCInputManager).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_LastInputMethod");
            lastInputMethodMethod = lastInputMethodInfo?.GetGetMethod();

        }
    }
}
