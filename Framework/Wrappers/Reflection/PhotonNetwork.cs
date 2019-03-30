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
using VRCSDK2;

namespace NekoClient.Wrappers.Reflection
{
    public static class PhotonNetworkWrappers
    {
        private static Type eventLogType = null;
        private static Type logoutType = null;
        private static FieldInfo typeDictInfo = null;
        private static MethodInfo typeDictMethod = null;
        private static PropertyInfo typeDictItemInfo = null;
        private static FieldInfo codeDictInfo = null;
        private static MethodInfo codeDictMethod = null;
        private static Type customType = null;
        private static FieldInfo serializeMethodInfo = null;
        private static FieldInfo deserializeMethodInfo = null;

        private static SerializeMethod originalSerializeMethod = null;
        private static DeserializeMethod originalDeserializeMethod = null;

        /*private static BindingFlags any = ((System.Reflection.BindingFlags)(-1));

        private static Type vrcEventType = typeof(NetworkMetadata).GetMethod("OnDestroy", any).Parse().LastOrDefault((ILInstruction x) => x.OpCode == OpCodes.Ldftn).GetArgument<MethodInfo>().GetParameters().First<ParameterInfo>().ParameterType;
        private static MethodInfo setVrcEventMethod = vrcEventType.GetProperties(any).FirstOrDefault((PropertyInfo x) => x.PropertyType == typeof(VRC_EventHandler.VrcEvent)).GetSetMethod();
        private static MethodInfo setPhotonTime = vrcEventType.GetProperties(any).FirstOrDefault((PropertyInfo x) => x.PropertyType == typeof(double)).GetSetMethod();

        private static FieldInfo vrcBroadcastTypeField = vrcEventType.GetFields(any).FirstOrDefault((FieldInfo x) => x.FieldType == typeof(VRC_EventHandler.VrcBroadcastType));
        private static FieldInfo someRandomIntField = vrcEventType.GetFields(any).FirstOrDefault((FieldInfo x) => x.FieldType == typeof(int));
        private static FieldInfo someRandomLongField = vrcEventType.GetFields(any).FirstOrDefault((FieldInfo x) => x.FieldType == typeof(long));
        private static FieldInfo someRandomFloatField = vrcEventType.GetFields(any).FirstOrDefault((FieldInfo x) => x.FieldType == typeof(float));
        private static MethodInfo getPhotonTime = typeof(ModerationManager).GetMethod("IsKicked").Parse().First((ILInstruction x) => x.OpCode == OpCodes.Call).GetArgument<MethodInfo>();
        private static MethodInfo getSomeInt = typeof(UserCameraController).GetMethod("onUseDown").Parse().First((ILInstruction x) => x.OpCode == OpCodes.Call).GetArgument<MethodInfo>();*/

        private static MethodInfo rpcSecureMethod = null;

        private static void TypeDictRemove(object value)
        {
            object instance = typeDictInfo.GetValue(null);
            typeDictMethod.Invoke(instance, new object[] { value });
        }

        private static void CodeDictRemove(byte value)
        {
            object instance = codeDictInfo.GetValue(null);
            codeDictMethod.Invoke(instance, new object[] { value });
        }

        public static void RpcSecure(PhotonView pv, string n, object p, bool b, object[] objs)
        {
            rpcSecureMethod.Invoke(pv, new object[]
            {
                    n,
                    p,
                    b,
                    objs
            });
        }

        internal static void Initialize()
        {
            eventLogType = typeof(VRC_EventLog);
            logoutType = eventLogType
                            .GetNestedTypes()
                            .FirstOrDefault(t =>
                                t.GetInterfaces().Length != 0 &&
                                t.GetMethods().Any(m =>
                                    m.Name.ToLowerInvariant() == "tostring"
                                )
                            );

            typeDictInfo = typeof(Protocol).GetField("TypeDict", (BindingFlags)62);
            typeDictMethod = typeDictInfo.FieldType.GetMethod("Remove");
            typeDictItemInfo = typeDictInfo.FieldType.GetProperty("Item");

            codeDictInfo = typeof(Protocol).GetField("CodeDict", (BindingFlags)62);
            codeDictMethod = codeDictInfo.FieldType.GetMethod("Remove");

            customType = typeof(Protocol).Assembly.GetType("ExitGames.Client.Photon.CustomType");
            serializeMethodInfo = customType.GetField("SerializeFunction");
            deserializeMethodInfo = customType.GetField("DeserializeFunction");

            rpcSecureMethod = typeof(PhotonView).GetMethods().Last((MethodInfo x) => x.Name == "RpcSecure" && x.GetParameters().Length == 4 && x.GetParameters()[1].ParameterType == PhotonPlayerWrappers.photonPlayerType);
        }

        public static void SendCustomEvent(int photonId, SerializeMethod method)
        {
            void Clear()
            {
                if (originalSerializeMethod == null || originalDeserializeMethod == null)
                {
                    object instance = typeDictInfo.GetValue(null);
                    object customTypeInfo = typeDictItemInfo.GetValue(instance, new object[] { logoutType });

                    if (originalSerializeMethod == null)
                    {
                        originalSerializeMethod = (SerializeMethod)serializeMethodInfo.GetValue(customTypeInfo);
                    }

                    if (originalDeserializeMethod == null)
                    {
                        originalDeserializeMethod = (DeserializeMethod)deserializeMethodInfo.GetValue(customTypeInfo);
                    }
                }

                TypeDictRemove(logoutType);
                CodeDictRemove(102);
            }

            void RegisterCustom()
            {
                PhotonPeer.RegisterType(logoutType, (byte)102, method, originalDeserializeMethod);
            }

            void Send()
            {
                PhotonView photonView = PhotonView.Get(VRC_EventLog.Instance.Replicator);

                object target = PhotonPlayerWrappers.GetById(photonId);

                if (target != null)
                {
                    RpcSecure(photonView, "ProcessEvent", target, true, new object[]
                    {
                        Activator.CreateInstance(logoutType)
                    });
                }
            }

            void RegisterOriginal()
            {
                PhotonPeer.RegisterType(logoutType, 102, originalSerializeMethod, originalDeserializeMethod);
            }

            Clear();
            RegisterCustom();
            Send();
            Clear();
            RegisterOriginal();
        }
    }
}