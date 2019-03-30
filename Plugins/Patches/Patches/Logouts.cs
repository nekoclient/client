using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Harmony;
using VRC.Core;
using AmplitudeSDKWrapper;
using System.Reflection;
using System.Collections.Generic;
using static Patches.Patches;
using System;
using ExitGames.Client.Photon;
using NekoClient.Logging;
using System.Collections;
using NekoClient;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using NekoClient.Wrappers.Reflection;

namespace Patches
{
#if STEAM || OCULUS
    class LogoutPatches
    {
        public static void Perform(HarmonyInstance instance)
        {

        }
    }
#elif TODO
    class SomeCustomClass
    {

        public static object Deserialize(byte[] data)
        {
            return 0;
        }

        public static object DeserializeFake(StreamBuffer buffer)
        {
            return new object();
        }

        public static byte[] Serialize(object function)
        {
            return new byte[0];
        }
    }

    class LogoutPatches
    {
        public static void Perform(HarmonyInstance harmony)
        {
            PerformPatch("Logouts#DeserializeEventData", () =>
            {
                harmony.Patch(
                    typeof(Protocol16).GetMethod("DeserializeEventData", BindingFlags.Public | BindingFlags.Instance),
                    GetPatch("GetEventData", typeof(LogoutPatches)),
                    null, null
                );
            });

            /*PerformPatch("Logouts#2", () =>
            {
                harmony.Patch(
                    Assembly.GetAssembly(typeof(VRCPlayer))
                        .GetTypes().First(x => x.GetInterface("IPhotonPeerListener") != null && x.GetMethod("OnOperationResponse", BindingFlags.Public | BindingFlags.Instance) != null)
                        .GetMethod("OnEvent", BindingFlags.Public | BindingFlags.Instance),
                    GetPatch("GetOnEventData", typeof(Logouts)),
                    null, null
                );
            });

            PerformPatch("Logouts#3", () =>
            {
                harmony.Patch(
                    Assembly.GetAssembly(typeof(VRCPlayer))
                        .GetTypes().First(x => x.GetInterface("IPhotonPeerListener") != null && x.GetMethod("OnOperationResponse", BindingFlags.Public | BindingFlags.Instance) != null)
                        .GetMethod("OnOperationResponse", BindingFlags.Public | BindingFlags.Instance),
                    GetPatch("GetResponse", typeof(Logouts)),
                    null, null
                );
            });*/

            bool registered = PhotonPeer.RegisterType(typeof(SomeCustomClass), 122, new SerializeMethod(SomeCustomClass.Serialize), new DeserializeMethod(SomeCustomClass.Deserialize));
            string msg = registered ? "Registered" : "Failed";

            Log.Info($"Custom type: {msg}");
        }

        private static Dictionary<byte, object> DeserializeParameterTable(StreamBuffer stream, ref bool deserialized)
        {
            Dictionary<byte, object> dictionary = null;
            try
            {
                short num = Protocol.ProtocolDefault.DeserializeShort(stream);
                dictionary = new Dictionary<byte, object>((int)num);
                for (int i = 0; i < (int)num; i++)
                {
                    byte key = (byte)stream.ReadByte();
                    object value = Deserialize(stream, (byte)stream.ReadByte(), ref deserialized);
                    dictionary[key] = value;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return dictionary;
        }

        private static object Deserialize(StreamBuffer din, byte type, ref bool deserialized)
        {
            switch (type)
            {
                case 12:
                case 19:
                case 24:
                case 25:
                    deserialized = true;
                    return SomeCustomClass.DeserializeFake(din);
                case 99:
                    return DeserializeCustom(din, (byte)din.ReadByte(), ref deserialized);
                case 104:
                    return DeserializeHashTable(din, ref deserialized);
                case 122:
                    return DeserializeObjectArray(din, ref deserialized);
                default:
                    return Protocol.ProtocolDefault.Deserialize(din, type);
            }
        }

        private static Hashtable DeserializeHashTable(StreamBuffer din, ref bool deserialized)
        {
            int num = (int)Protocol.ProtocolDefault.DeserializeShort(din);
            Hashtable hashtable = new Hashtable(num);

            for (int i = 0; i < num; i++)
            {
                object key = Deserialize(din, (byte)din.ReadByte(), ref deserialized);
                object value = Deserialize(din, (byte)din.ReadByte(), ref deserialized);
                hashtable[key] = value;
            }

            return hashtable;
        }

        private static object[] DeserializeObjectArray(StreamBuffer din, ref bool deserialized)
        {
            short num = Protocol.ProtocolDefault.DeserializeShort(din);
            object[] array = new object[(int)num];
            for (int i = 0; i < (int)num; i++)
            {
                byte type = (byte)din.ReadByte();
                array[i] = Deserialize(din, type, ref deserialized);
            }
            return array;
        }

        private static object DeserializeCustom(StreamBuffer din, byte customTypeCode, ref bool deserialized)
        {
            object result = null;
            CustomType customType = null;
            try
            {
                short num = Protocol.ProtocolDefault.DeserializeShort(din);
                bool flag = Protocol.CodeDict.TryGetValue(customTypeCode, out customType);
                if (flag)
                {
                    bool flag2 = customType.DeserializeStreamFunction == null;
                    if (flag2)
                    {
                        try
                        {
                            byte[] array = new byte[(int)num];
                            din.Read(array, 0, (int)num);
                            if (!array.All(x => x == 0))
                            {
                                result = customType.DeserializeFunction(array);
                            }
                            if (result == null && customType.Code == 102)
                            {
                                Log.Write(LogLevel.Error, "Could Not Deserialize Event Log Entry with length of " + array.Length + " bytes");
                                deserialized = false;
                            }
                        }
                        catch
                        {
                            if (customType.Code == 102)
                            {
                                Log.Write(LogLevel.Error, "Could Not Deserialize Event Log Entry");
                                deserialized = false;
                            }
                        }
                    }
                    else
                    {
                        long position = din.Position;
                        object obj = customType.DeserializeStreamFunction(din, num);
                        int num2 = (int)(din.Position - position);
                        bool flag3 = num2 != num;
                        if (flag3)
                        {
                            din.Position = (int)(position + num);
                        }
                        result = obj;
                        if (result == null)
                        {
                            deserialized = false;
                        }
                    }
                }
                else
                {
                    byte[] array2 = new byte[num];
                    din.Read(array2, 0, num);
                    result = array2;
                }
            }
            catch
            {
                deserialized = false;
                Log.Write(LogLevel.Error, "Unable To Deserialize Custom Type");
            }

            return result;
        }


        private static EventData DeserializeEventData(StreamBuffer din, ref bool deserialized)
        {
            try
            {
                return new EventData
                {
                    Code = Protocol.ProtocolDefault.DeserializeByte(din),
                    Parameters = DeserializeParameterTable(din, ref deserialized)
                };
            }
            catch
            {
                return new EventData
                {
                    Code = Protocol.ProtocolDefault.DeserializeByte(din),
                    Parameters = new Dictionary<byte, object>()
                };
            }
        }

        private static IEnumerator DecodeData(StreamBuffer buffer, bool largerbuffer = false)
        {
            if (buffer == null)
            {
                Log.Error("buffer is null?");
            }

            try
            {
                object photonPlayer = null;
                EventData data = null;
                bool deserialized = true;

                try
                {
                    data = DeserializeEventData(buffer, ref deserialized);
                }
                catch
                {
                    Log.Write(LogLevel.Error, "Coulnt Deserialize Event Data!");
                }

                if (data != null)
                {
                    if (data.Parameters.ContainsKey(254))
                    {
                        photonPlayer = PhotonPlayerWrappers.GetById((int)data[254]);
                    }

                    string playerName = "";

                    if (photonPlayer != null)
                    {
                        playerName = (string)((Dictionary<string, object>)photonPlayer.GetFromPhotonPlayer().CustomProperties()["user"])["displayName"];
                    }

                    if (data.Code == 200)
                    {
                        var paramtable = data[245] as Hashtable;

                        if (paramtable.ContainsKey(4) && deserialized)
                        {
                            var eventlog = (object[])paramtable[4];

                            var entry = eventlog.First() as VRC_EventLog.INGKOGDACII;

                            if (entry.HILCOMNAMAI.ParameterBytes.All(x => x == 0) || entry.HILCOMNAMAI.ParameterBytes.Length >= 2400)
                            {
                                if (entry.HILCOMNAMAI.ParameterBytes.Length > 250)
                                {
                                    Log.Info($"{playerName} tried logging you out!");
                                }
                                else if (entry.HILCOMNAMAI.ParameterString.ToLowerInvariant() == "myrpc")
                                {
                                    Log.Info("${playerName} sent a myRPC :thinking:");
                                }
                                else
                                {
                                    Log.Info($"{playerName} tried crashing you!");
                                }

                                ConnectionPatches.LastLogoutTime = DateTime.UtcNow;
                            }
                        }
                        else
                        {

                        }
                    }

                    if (data.Code == 200 || data.Code == 1 && photonPlayer != null)
                    {
                        if (largerbuffer || !deserialized)
                        {
                            Log.Info($"{playerName} tried logging you out!");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());

                buffer = null;
                yield break;
            }

            buffer = null;
            yield break;
        }

        private static bool GetEventData(StreamBuffer __0, ref EventData __result)
        {
            StreamBuffer buffer = new StreamBuffer(__0.GetBuffer())
            {
                Position = __0.Position
            };

            if (__0.Length > 1024)
            {
                string msg = $"Received large ({__0.Length}b) buffer!";
                Log.Info(msg);
            }

            if (__0.Length > 4096)
            {
                Patches.QueueCoroutine(DecodeData(buffer, true));
            }
            else
            {
                Patches.QueueCoroutine(DecodeData(buffer));
            }

            __result = new EventData
            {
                Code = 0,
                Parameters = new Dictionary<byte, object>()
            };

            return true;
        }
    }
#endif
}