using ExitGames.Client.Photon;
using NekoClient.Logging;
using NekoClient.Wrappers.IL;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using VRC;

namespace NekoClient.Wrappers.Reflection
{
    public static class PhotonPlayerWrappers
    {
        private static Type photonNetworkType = null;
        internal static Type photonPlayerType = null;
        private static MethodInfo photonActorMethod = null;
        private static MethodInfo photonListMethod = null;
        private static MethodInfo customPropertiesMethod = null;
        //private static FieldInfo customPropertiesInfo = null;

        public static int PhotonActor(this object p)
            => (int)photonActorMethod.Invoke(p, null);

        public static object[] PlayerList()
            => (object[])photonListMethod.Invoke(null, null);

        public static object GetById(int id)
            => PlayerList().FirstOrDefault(p => p.PhotonActor() == id);

        /*public static Hashtable CustomProperties(this Player p)
            => (Hashtable)customPropertiesInfo.GetValue(p.vrcPlayer);*/

        public static Hashtable CustomProperties(this Player p)
            => (Hashtable)customPropertiesMethod.Invoke(p, null);

        public static Player GetFromPhotonPlayer(this object pp)
            => PlayerManager.GetAllPlayers().FirstOrDefault(p => p.PhotonPlayer().PhotonActor() == pp.PhotonActor());


        internal static void Initialize()
        {
            photonNetworkType = typeof(PhotonLagSimulationGui).GetMethod("Start").Parse().First((ILInstruction x) => x.OpCode == OpCodes.Ldsfld).GetArgument<FieldInfo>().DeclaringType;
            photonPlayerType = PlayerWrappers.photonPlayerInfo.PropertyType;

            photonListMethod = photonNetworkType.GetProperties().FirstOrDefault((PropertyInfo p) => p.PropertyType.IsArray && p.PropertyType != typeof(PhotonView)).GetGetMethod();

            PropertyInfo photonActorInfo = photonPlayerType.GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().ReturnType == typeof(int));
            photonActorMethod = photonActorInfo?.GetGetMethod();

            //customPropertiesInfo = typeof(VRCPlayer).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault((FieldInfo p) => p.FieldType == typeof(Hashtable));
            PropertyInfo customPropertiesInfo = photonPlayerType.GetProperties().FirstOrDefault((PropertyInfo p) => p.GetGetMethod().ReturnType == typeof(Hashtable));
            customPropertiesMethod = customPropertiesInfo?.GetGetMethod();
        }
    }
}
