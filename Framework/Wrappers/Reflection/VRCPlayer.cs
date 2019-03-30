using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRC;
using VRC.Core;
using VRCSDK2;

namespace NekoClient.Wrappers.Reflection
{
    public static class VRCPlayerWrappers
    {
        private static MethodInfo pingMethod = null;
        private static MethodInfo pingVarianceMethod = null;
        private static MethodInfo connectionQualityMethod = null;
        private static MethodInfo connectionDisparityMethod = null;
        private static MethodInfo playerMethod = null;
        private static MethodInfo playerNetMethod = null;
        private static MethodInfo numberOfUsersMetInWorldMethod = null;
        private static MethodInfo uSpeakerMethod = null;
        private static MethodInfo playerNameMethod = null;
        private static MethodInfo avatarManagerMethod = null;
        private static MethodInfo initializedMethod = null;
        private static MethodInfo readyMethod = null;
        private static MethodInfo isAvatarChangingMethod = null;
        private static MethodInfo apiPlayerMethod = null;
        private static MethodInfo handsMethod = null;
        private static MethodInfo isLocalMethod = null;
        private static MethodInfo isAvatarLoadedMethod = null;
        private static MethodInfo isProcessingNewAvatarMethod = null;
        private static MethodInfo apiAvatarMethod = null;
        private static MethodInfo isCrawlingMethod = null;
        private static MethodInfo isCrouchingMethod = null;
        private static MethodInfo steamUserIDULongMethod = null;
        private static MethodInfo defaultModTagMethod = null;
        private static MethodInfo modTagMethod = null;
        private static MethodInfo localModTagMethod = null;
        private static MethodInfo isInvisibleMethod = null;
        private static MethodInfo localIsInvisibleMethod = null;
        private static MethodInfo showLocalPlayerAvatarMethod = null;
        private static MethodInfo mustUpdateMethod = null;
        private static MethodInfo shouldUpdateMethod = null;
        private static MethodInfo isAvatarAudioSourcePlayingMethod = null;
        private static MethodInfo areAnyParticleSystemsPlayingMethod = null;
        private static MethodInfo isUsingCustomShadersMethod = null;

        public static short Ping(this VRCPlayer p)
            => (short)pingMethod.Invoke(p, null);

        public static short PingVariance(this VRCPlayer p)
            => (short)pingVarianceMethod.Invoke(p, null);

        public static float ConnectionQuality(this VRCPlayer p)
            => (float)connectionQualityMethod.Invoke(p, null);

        public static float ConnectionDisparity(this VRCPlayer p)
            => (float)connectionDisparityMethod.Invoke(p, null);

        public static VRC.Player Player(this VRCPlayer p)
            => (VRC.Player)playerMethod.Invoke(p, null);

        public static PlayerNet PlayerNet(this VRCPlayer p)
            => (PlayerNet)playerNetMethod.Invoke(p, null);

        public static int NumberOfUsersMetInWorld(this VRCPlayer p)
            => (int)numberOfUsersMetInWorldMethod.Invoke(p, null);

        public static USpeaker USpeaker(this VRCPlayer p)
            => (USpeaker)uSpeakerMethod.Invoke(p, null);

        public static string PlayerName(this VRCPlayer p)
            => (string)playerNameMethod.Invoke(p, null);

        public static VRCAvatarManager AvatarManager(this VRCPlayer p)
            => (VRCAvatarManager)avatarManagerMethod.Invoke(p, null);

        public static bool Initialized(this VRCPlayer p)
            => (bool)initializedMethod.Invoke(p, null);

        public static bool Ready(this VRCPlayer p)
            => (bool)readyMethod.Invoke(p, null);

        public static bool IsAvatarChanging(this VRCPlayer p)
            => (bool)isAvatarChangingMethod.Invoke(p, null);

        public static VRC_PlayerApi ApiPlayer(this VRCPlayer p)
            => (VRC_PlayerApi)apiPlayerMethod.Invoke(p, null);

        public static List<VRCHandGrasper> Hands(this VRCPlayer p)
            => (List<VRCHandGrasper>)handsMethod.Invoke(p, null);

        public static bool IsLocal(this VRCPlayer p)
            => (bool)isLocalMethod.Invoke(p, null);

        public static bool IsAvatarLoaded(this VRCPlayer p)
            => (bool)isAvatarLoadedMethod.Invoke(p, null);

        public static bool IsProcessingNewAvatar(this VRCPlayer p)
            => (bool)isProcessingNewAvatarMethod.Invoke(p, null);

        public static ApiAvatar ApiAvatar(this VRCPlayer p)
            => (ApiAvatar)apiAvatarMethod.Invoke(p, null);

        public static bool IsCrawling(this VRCPlayer p)
            => (bool)isCrawlingMethod.Invoke(p, null);

        public static bool IsCrouching(this VRCPlayer p)
            => (bool)isCrouchingMethod.Invoke(p, null);

        public static ulong SteamUserIdULong(this VRCPlayer p)
            => (ulong)steamUserIDULongMethod.Invoke(p, null);

        public static string DefaultModTag
            => (string)defaultModTagMethod.Invoke(null, null);

        public static string ModTag(this VRCPlayer p)
            => (string)modTagMethod.Invoke(p, null);

        public static string LocalModTag(this VRCPlayer p)
            => (string)localModTagMethod.Invoke(p, null);

        public static bool IsInvisible(this VRCPlayer p)
            => (bool)isInvisibleMethod.Invoke(p, null);

        public static bool LocalIsInvisible(this VRCPlayer p)
            => (bool)localIsInvisibleMethod.Invoke(p, null);

        public static bool ShowLocalPlayerAvatar(this VRCPlayer p)
            => (bool)showLocalPlayerAvatarMethod.Invoke(p, null);

        public static bool MustUpdate(this VRCPlayer p)
            => (bool)mustUpdateMethod.Invoke(p, null);

        public static bool ShouldUpdate(this VRCPlayer p)
            => (bool)shouldUpdateMethod.Invoke(p, null);

        public static bool IsAvatarAudioSourcePlaying(this VRCPlayer p)
            => (bool)isAvatarAudioSourcePlayingMethod.Invoke(p, null);

        public static bool AreAnyParticleSystemsPlaying(this VRCPlayer p)
            => (bool)areAnyParticleSystemsPlayingMethod.Invoke(p, null);

        public static bool IsUsingCustomShaders(this VRCPlayer p)
            => (bool)isUsingCustomShadersMethod.Invoke(p, null);

        internal static void Initialize()
        {
            PropertyInfo pingInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_Ping");
            pingMethod = pingInfo?.GetGetMethod();

            PropertyInfo pingVarianceInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_PingVariance");
            pingVarianceMethod = pingVarianceInfo?.GetGetMethod();

            PropertyInfo connectionQualityInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ConnectionQuality");
            connectionQualityMethod = connectionQualityInfo?.GetGetMethod();

            PropertyInfo connectionDisparityInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ConnectionDisparity");
            connectionDisparityMethod = connectionDisparityInfo?.GetGetMethod();

            PropertyInfo playerInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_player");
            playerMethod = playerInfo?.GetGetMethod();

            PropertyInfo playerNetInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_playerNet");
            playerNetMethod = playerNetInfo?.GetGetMethod();

            PropertyInfo numberOfUsersMetInWorldInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_NumberOfUsersMetInWorld");
            numberOfUsersMetInWorldMethod = numberOfUsersMetInWorldInfo?.GetGetMethod();

            PropertyInfo uSpeakerInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_uSpeaker");
            uSpeakerMethod = uSpeakerInfo?.GetGetMethod();

            PropertyInfo playerNameInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_PlayerName");
            playerNameMethod = playerNameInfo?.GetGetMethod();

            PropertyInfo avatarManagerInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_AvatarManager");
            avatarManagerMethod = avatarManagerInfo?.GetGetMethod();

            PropertyInfo initializedInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_initialized");
            initializedMethod = initializedInfo?.GetGetMethod();

            PropertyInfo readyInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ready");
            readyMethod = readyInfo?.GetGetMethod();

            PropertyInfo isAvatarChangingInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isAvatarChanging");
            isAvatarChangingMethod = isAvatarChangingInfo?.GetGetMethod();

            PropertyInfo apiPlayerInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_apiPlayer");
            apiPlayerMethod = apiPlayerInfo?.GetGetMethod();

            PropertyInfo handsInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_hands");
            handsMethod = handsInfo?.GetGetMethod();

            PropertyInfo isLocalInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsLocal");
            isLocalMethod = isLocalInfo?.GetGetMethod();

            PropertyInfo isAvatarLoadedInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsAvatarLoaded");
            isAvatarLoadedMethod = isAvatarLoadedInfo?.GetGetMethod();

            PropertyInfo isProcessingNewAvatarInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsProcessingNewAvatar");
            isProcessingNewAvatarMethod = isProcessingNewAvatarInfo?.GetGetMethod();

            PropertyInfo apiAvatarInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_AvatarModel");
            apiAvatarMethod = apiAvatarInfo?.GetGetMethod();

            PropertyInfo isCrawlingInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isCrawling");
            isCrawlingMethod = isCrawlingInfo?.GetGetMethod();

            PropertyInfo isCrouchingInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isCrouching");
            isCrouchingMethod = isCrouchingInfo?.GetGetMethod();

            PropertyInfo steamUserIDULongInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_SteamUserIDULong");
            steamUserIDULongMethod = steamUserIDULongInfo?.GetGetMethod();

            PropertyInfo defaultModTagInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_DefaultModTag");
            defaultModTagMethod = defaultModTagInfo?.GetGetMethod();

            PropertyInfo modTagInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_modTag");
            modTagMethod = modTagInfo?.GetGetMethod();

            PropertyInfo localModTagInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_LocalModTag");
            localModTagMethod = localModTagInfo?.GetGetMethod();

            PropertyInfo isInvisibleInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_isInvisible");
            isInvisibleMethod = isInvisibleInfo?.GetGetMethod();

            PropertyInfo localIsInvisibleInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_LocalIsInvisible");
            localIsInvisibleMethod = localIsInvisibleInfo?.GetGetMethod();

            PropertyInfo showLocalPlayerAvatarInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_showLocalPlayerAvatar");
            showLocalPlayerAvatarMethod = showLocalPlayerAvatarInfo?.GetGetMethod();

            PropertyInfo mustUpdateInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_MustUpdate");
            mustUpdateMethod = mustUpdateInfo?.GetGetMethod();

            PropertyInfo shouldUpdateInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_ShouldUpdate");
            shouldUpdateMethod = shouldUpdateInfo?.GetGetMethod();

            PropertyInfo isAvatarAudioSourcePlayingInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsAvatarAudioSourcePlaying");
            isAvatarAudioSourcePlayingMethod = isAvatarAudioSourcePlayingInfo?.GetGetMethod();

            PropertyInfo areAnyParticleSystemsPlayingInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_AreAnyParticleSystemsPlaying");
            areAnyParticleSystemsPlayingMethod = areAnyParticleSystemsPlayingInfo?.GetGetMethod();

            PropertyInfo isUsingCustomShadersInfo = typeof(VRCPlayer).GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().Name == "get_IsUsingCustomShaders");
            isUsingCustomShadersMethod = isUsingCustomShadersInfo?.GetGetMethod();
        }
    }
}
 
 