using ExitGames.Client.Photon;
using NekoClient;
using NekoClient.Helpers;
using NekoClient.Logging;
using NekoClient.UI;
using NekoClient.UI.Decorators;
using NekoClient.Wrappers;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC;
using VRC.Core;

namespace TestPlugin
{
    public class TestPlugin : PluginBase
    {
        private enum UpdateMode
        {
            Normal = 0,
            AnimatePhysics = 1,
            UnscaledTime = 2
        }
        private enum FreezeAxis
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 3
        }

        public enum Direction
        {
            X = 0,
            Y = 1,
            Z = 2
        }

        public enum Bound
        {
            Outside = 0,
            Inside = 1
        }

        private class TheresThree
        {
            public float X;
            public float Y;
            public float Z;
        }

        private class DynamicBoneDump
        {
            public bool m_DistantDisable;
            public FreezeAxis m_FreezeAxis;
            public List<string> m_Exclusions;
            public List<DynamicBoneColliderDump> m_Colliders;
            public TheresThree m_Force;
            public TheresThree m_Gravity;
            public float m_EndLength;
            public AnimationCurve m_RadiusDistrib;
            public TheresThree m_EndOffset;
            public AnimationCurve m_InertDistrib;
            public float m_UpdateRate;
            public float m_Radius;
            public AnimationCurve m_DampingDistrib;
            public float m_Elasticity;
            public float m_Damping;
            public float m_Stiffness;
            public AnimationCurve m_StiffnessDistrib;
            public float m_Inert;
            public AnimationCurve m_ElasticityDistrib;
            internal string m_parentName;
        }

        private class DynamicBoneColliderDump
        {
            public TheresThree m_Center;
            public float m_Radius;
            public float m_Height;
            public Direction m_Direction;
            public Bound m_Bound;
        }

#if TESTUICHANGES
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            VRCUiPageTabGroup[] info = (VRCUiPageTabGroup[])typeof(VRCUiPageTabManager).GetField("CNFIIMIEIOD", (System.Reflection.BindingFlags)62).GetValue(UIWrappers.VRCUiPageTabManagerInstance);

            foreach (VRCUiPageTabGroup group in info)
            {
                Log.Info($"group: {group.gameObject.name}, {group.name}");
            }

            VRCUiPageTab tab = UIWrappers.VRCUiPageTabManagerInstance.CreateTab("NekoClient", "nekoclient", VRCUiPageTabManager.NFBPFIKBDKI.Everywhere, info[0]);
        }
#endif

        public TestPlugin()
        {
            Tick += TestPlugin_Tick;
#if TESTUICHANGES
            QueueCoroutine(DelayedStart());
#endif
        }

        [EntryAttribute(MenuId.PlayerDetails, "Test Command")]
        public static void TestCommand()
        {
            Log.Info("PRESSED TEST COMMAND");
            // this gets run if Test Command is pressed in the Player Details menu
        }

        [EntryAttribute(MenuId.Main, "Open Player Details")]
        public static void OpenPlayerDetails()
        {
            Log.Info("PRESSED OPENPLAYERDETAILS");
        }

        private void TestPlugin_Tick()
        {
            if (Input.GetKeyDown(KeyCode.U) && Input.GetKey(KeyCode.LeftControl))
            {
                string[] text = File.ReadAllText("notiftext.txt").Replace("\r\n", "\n").Split('\n');

                string target = text[0];
                string message = text[1];

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["worldId"] = ":" + RoomManagerBase.currentRoom.currentInstanceIdWithTags;
                dictionary["worldName"] = RoomManagerBase.currentRoom.name;
                ApiNotification.SendNotification(target, ApiNotification.NotificationType.Invite, message, dictionary, null, null);
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                void DumpGameObject(GameObject gameObject, StreamWriter writer, string indent)
                {
                    writer.WriteLine("{0}+{1}", indent, gameObject.name);

                    foreach (Component component in gameObject.GetComponents<Component>())
                    {
                        DumpComponent(component, writer, indent + "\t");
                    }

                    foreach (Transform child in gameObject.transform)
                    {
                        DumpGameObject(child.gameObject, writer, indent + "\t");

                        /*foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(child.gameObject))
                        {
                            string name = descriptor.Name;
                            object value = descriptor.GetValue(child.gameObject);
                            writer.WriteLine($"{name}={value}");
                        }*/
                    }
                }

                void DumpComponent(Component component, StreamWriter writer, string indent)
                {
                    writer.WriteLine("{0}{1}", indent, (component == null ? "(null)" : component.GetType().Name));
                }

                List<GameObject> rootObjects = new List<GameObject>();

                Scene scene = SceneManager.GetActiveScene();

                {
                    List<GameObject> tempRootObjects = new List<GameObject>();

                    scene.GetRootGameObjects(tempRootObjects);

                    rootObjects.AddRange(tempRootObjects);
                }

                new Thread(() =>
                {
                    if (!Directory.Exists("NekoClient\\Objects"))
                    {
                        Directory.CreateDirectory("NekoClient\\Objects");
                    }

                    StreamWriter writer = new StreamWriter($"NekoClient\\Objects\\Objects-{DateTime.Now.ToFileTimeUtc()}.txt", false);

                    for (int i = 0; i < rootObjects.Count; ++i)
                    {
                        //if (!rootObjects[i].name.Contains("VRCPlayer"))
                        {
                            DumpGameObject(rootObjects[i], writer, "");
                        }
                    }

                    writer.Close();
                }).Start();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {                
                VRCPlayer me = PlayerWrappers.GetLocalPlayer().vrcPlayer;
               
                DynamicBone[] bones = me.GetComponentsInChildren<DynamicBone>();
                
                List<DynamicBoneDump> dump = new List<DynamicBoneDump>();
                
                foreach (DynamicBone bone in bones)
                {
                    try
                    {
                        List<string> exclusions = new List<string>();
                        List<DynamicBoneColliderDump> colliders = new List<DynamicBoneColliderDump>();
                    
                        if (bone.m_Exclusions != null)
                        {
                            foreach (Transform t in bone.m_Exclusions)
                            {
                                exclusions.Add(t.gameObject.name);
                            }
                        }

                        if (bone.m_Colliders != null)
                        {
                            foreach (DynamicBoneCollider c in bone.m_Colliders)
                            {
                                colliders.Add(new DynamicBoneColliderDump()
                                {
                                    m_Center = new TheresThree()
                                    {
                                        X = c.m_Center.x,
                                        Y = c.m_Center.y,
                                        Z = c.m_Center.z
                                    },
                                    m_Bound = (Bound)c.m_Bound,
                                    m_Direction = (Direction)c.m_Direction,
                                    m_Height = c.m_Height,
                                    m_Radius = c.m_Radius
                                });
                            }
                        }

                        dump.Add(new DynamicBoneDump()
                        {
                            m_parentName = bone.gameObject.name,
                            m_DistantDisable = bone.m_DistantDisable,
                            m_FreezeAxis = (FreezeAxis)bone.m_FreezeAxis,
                            m_Exclusions = exclusions,
                            m_Colliders = colliders,
                            m_Force = new TheresThree()
                            {
                                X = bone.m_Force.x,
                                Y = bone.m_Force.y,
                                Z = bone.m_Force.z,
                            },
                            m_Gravity = new TheresThree()
                            {
                                X = bone.m_Gravity.x,
                                Y = bone.m_Gravity.y,
                                Z = bone.m_Gravity.z,
                            },
                            m_EndLength = bone.m_EndLength,
                            m_RadiusDistrib = bone.m_RadiusDistrib,
                            m_EndOffset = new TheresThree()
                            {
                                X = bone.m_EndOffset.x,
                                Y = bone.m_EndOffset.y,
                                Z = bone.m_EndOffset.z,
                            },
                            m_InertDistrib = bone.m_InertDistrib,
                            m_UpdateRate = bone.m_UpdateRate,
                            m_Radius = bone.m_Radius,
                            m_DampingDistrib = bone.m_DampingDistrib,
                            m_Elasticity = bone.m_Elasticity,
                            m_Damping = bone.m_Damping,
                            m_Stiffness = bone.m_Stiffness,
                            m_StiffnessDistrib = bone.m_StiffnessDistrib,
                            m_Inert = bone.m_Inert,
                            m_ElasticityDistrib = bone.m_ElasticityDistrib
                        });
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"Ignoring bone cause of: {e.InnerException.Message}");
                    }
                }

                /*VRCAvatarManager am = me.GetVRCAvatarManager();
                ApiAvatar aa = am == null ? null : am.GetApiAvatar();

                string fname = (aa?.name == null ? "ninininini" : am.name) + "_" + DateTime.Now.ToFileTimeUtc();*/
                
                new FileSystem("NekoClient\\DynamicBonesDump").SaveJson($"dump-{DateTime.Now.ToFileTimeUtc()}.json", dump);
            }
        }
    }
}