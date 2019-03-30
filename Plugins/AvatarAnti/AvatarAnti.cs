using NekoClient;
using NekoClient.Helpers;
using NekoClient.Logging;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace AvatarAnti
{
    public class AvatarAnti : PluginBase
    {
        private static AvatarAnti Instance;

        private class ParticleScanOptions
        {
            public bool Collision { get; set; }
            public bool LightSizeConstraint { get; set; }
            public int MaxKill { get; set; }
            public int MinKill { get; set; }
            public int MaxLights { get; set; }
            public int Emission { get; set; }
            public int Bursts { get; set; }
            public int MaxSystems { get; set; }
        }

        private class ComponentBlacklist
        {
            public bool MaterialsEnabled { get; set; }
            public bool ShadersEnabled { get; set; }
            public bool ReplaceWithPoiyomi { get; set; }
            public bool AudioSourcesEnabled { get; set; }
            public bool ParticleSystemsEnabled { get; set; }
            public bool MeshFiltersEnabled { get; set; }
            public bool LegacyAnimatorEnabled { get; set; }
            public bool StandardAnimatorEnabled { get; set; }
            public bool SpringJointEnabled { get; set; }
            public bool DynamicBonesEnabled { get; set; }
            public bool ClothEnabled { get; set; }
            public bool ReplaceInsteadDelete { get; set; }
            public bool IgnoreSelf { get; set; }
            public bool IgnoreFriends { get; set; }
            public bool ReplaceStandard { get; set; }
            public bool DisableEmotes { get; set; }
            public bool DisableGestures { get; set; }
        }

        private class Configuration
        {
            public ParticleScanOptions ParticleScanOptions { get; set; }
            public ComponentBlacklist ComponentBlacklist { get; set; }
        }

        private class ShaderInfo
        {
            public string Name { get; set; }
            public bool Blocked { get; set; }
        }

        private static Configuration Config;

        private static List<ShaderInfo> m_shaderList = new List<ShaderInfo>();
        private static List<string> m_passwords = new List<string>();

        private static FileSystem m_fileSystem = new FileSystem("NekoClient\\Configuration");

        public AvatarAnti()
        {
            Load += AvatarAnti_Load;
        }

        private void AvatarAnti_Load()
        {
            void GenerateConfig()
            {
                Config = new Configuration()
                {
                    ParticleScanOptions = new ParticleScanOptions()
                    {
                        Collision = false,
                        LightSizeConstraint = true,
                        MaxKill = 30000,
                        MinKill = 0,
                        MaxLights = 50,
                        Emission = 60000,
                        Bursts = 30000,
                        MaxSystems = 600
                    },
                    ComponentBlacklist = new ComponentBlacklist()
                    {
                        MaterialsEnabled = true,
                        ShadersEnabled = true,
                        ReplaceWithPoiyomi = false,
                        AudioSourcesEnabled = true,
                        ParticleSystemsEnabled = true,
                        MeshFiltersEnabled = false,
                        LegacyAnimatorEnabled = true,
                        StandardAnimatorEnabled = true,
                        SpringJointEnabled = true,
                        DynamicBonesEnabled = true,
                        ClothEnabled = true,
                        ReplaceInsteadDelete = true,
                        IgnoreSelf = true,
                        IgnoreFriends = true,
                        ReplaceStandard = true,
                        DisableEmotes = false,
                        DisableGestures = false
                    }
                };
            }

            if (!m_fileSystem.FileExists("AvatarAnti.json"))
            {
                GenerateConfig();
                m_fileSystem.SaveJson("AvatarAnti.json", Config);
            }
            else
            {
                Config = m_fileSystem.LoadJson<Configuration>("AvatarAnti.json");

                if (Config == null)
                {
                    GenerateConfig();
                    m_fileSystem.SaveJson("AvatarAnti.json", Config);
                }
            }

            m_shaderList.Clear();
            m_passwords.Clear();

            if (File.Exists(m_fileSystem.GetFilePath("Shaders.json")))
            {
                LoadShaderList();
                int blocked = m_shaderList.Where(s => s.Blocked == true).Count();
                Log.Info($"{m_shaderList.Count()} shaders tracked ({blocked} blocked)");
            }
            else
            {
                Log.Error("Shader blacklist file does not exist - skipping init");
            }

            string passwordFile = m_fileSystem.GetFilePath("Passwords.txt");

            // load gameobject passwords
            if (File.Exists(passwordFile))
            {
                StreamReader reader = new StreamReader(passwordFile);
                m_passwords = reader.ReadToEnd().Replace("\r\n", "\n").Split('\n').ToList();
                reader.Close();
                reader.Dispose();

                Log.Info($"{m_passwords.Count()} passwords tracked");
            }
            else
            {
                Log.Error("Skipping password removal - password list doesn't exist");
            }

            Instance = this;

            if (collideBones)
            {
                QueueCoroutine(UpdateBones());
            }
        }

        private IEnumerator UpdateBones()
        {
            void UpdateCollisionsIfNeeded(string type, List<DynamicBone> bones, List<DynamicBoneCollider> colliders)
            {
                int boneCount = 0;
                int colliderCount = 0;

                List<DynamicBone> bonesToRemove = new List<DynamicBone>();

                foreach (DynamicBone bone in bones)
                {                    
                    // if the bone is null: queue to remove
                    if (bone == null)
                    {
                        bonesToRemove.Add(bone);
                        continue;
                    }

                    // for every collider in the list
                    foreach (DynamicBoneCollider collider in colliders)
                    {
                        // if the bone doesn't have the collider
                        if (!bone.m_Colliders.Contains(collider))
                        {
                            // add the collider to the bone
                            bone.m_Colliders.Add(collider);

                            colliderCount++;
                        }

                        boneCount++;
                    }
                }

                // remove all bones which are null from original list
                foreach (DynamicBone bone in bonesToRemove)
                {
                    bones.Remove(bone);
                }

                //Log.Info($"{type}: added {colliderCount} colliders over {boneCount} bones (iterated through {bones.Count} bones and {colliders.Count} colliders)");
            }

            while (true)
            {
                // make remote collide with local
                UpdateCollisionsIfNeeded("local", m_myBones, m_targetBoneColliders);

                // make local collide with remote
                UpdateCollisionsIfNeeded("remote", m_targetBones, m_myBoneColliders);

                // wait for 5 seconds before next check
                yield return new WaitForSeconds(5f);
            }
        }

        private static void RemovePasswordedComponents(GameObject avatar)
        {
            Transform[] transforms = avatar.GetComponentsInChildren<Transform>();

            foreach (Transform trans in transforms)
            {
                if (trans == null || string.IsNullOrEmpty(trans.name))
                {
                    continue;
                }

                if (m_passwords.Contains(trans.name))
                {
                    Log.Debug($"Password removal - found password {trans.name}, removing...");

                    List<GameObject> children = new List<GameObject>();

                    foreach (Transform child in trans.GetComponentsInChildren<Transform>(true))
                    {
                        children.Add(child.gameObject);
                    }

                    foreach (GameObject child in children)
                    {
                        UnityEngine.Object.Destroy(child);
                    }

                    if (trans.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(trans.gameObject);
                    }

                    Log.Debug($"Password removal - destroyed {children.Count} passworded objects (children of {trans.name})");
                }
            }
        }

        private static void EnforceParticleLimits(GameObject avatar)
        {
            ParticleScanOptions options = Config.ParticleScanOptions;

            ParticleSystem[] systems = avatar.GetComponentsInChildren<ParticleSystem>(true);

            if (systems.Length == 0)
            {
                Log.Debug("Skipping particle limit check - no particles found on avatar.");
                return;
            }

            if (systems.Length > options.MaxSystems)
            {
                UnityEngine.Object.Destroy(avatar);
                Log.Debug($"Particle limit check - destroyed avatar {avatar.name} - too many particle systems");
                return;
            }

            for (int i = 0; i < systems.Length; i++)
            {
                void DestroyMe()
                {
                    Log.Debug($"Particle limit check - destroyed {systems[i].name}");
                    UnityEngine.Object.Destroy(systems[i].gameObject);
                    UnityEngine.Object.Destroy(systems[i]);
                }

                ParticleSystem.EmissionModule emission = systems[i].emission;
                ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
                ParticleSystem.CollisionModule collisionMod = systems[i].collision;
                ParticleSystem.LightsModule lightsMod = systems[i].lights;

                if (emission.rateOverTime.constantMax > options.Emission || emission.rateOverTime.constantMax < 0)
                {
                    Log.Debug($"Particle limit check - destroying particle system for {emission.rateOverTime.constantMax} rateOverTime emission");
                    DestroyMe();
                }
                else if (emission.rateOverDistance.constantMax > options.Emission || emission.rateOverDistance.constantMax < 0)
                {
                    Log.Debug($"Particle limit check - destroying particle system for {emission.rateOverDistance.constantMax} rateOverDistance emission");
                    DestroyMe();
                }
                else if (lightsMod.maxLights > options.MaxLights)
                {
                    Log.Debug($"Particle limit check - destroying particle system for {lightsMod.maxLights} lights maxcount");
                    DestroyMe();
                }
                else if (emission.burstCount != 0)
                {
                    emission.GetBursts(bursts);

                    for (int x = 0; x < bursts.Length; x++)
                    {
                        if (systems[i] == null)
                        {
                            continue;
                        }

                        short count = bursts[x].maxCount;

                        if (count > options.Bursts || count < 0)
                        {
                            Log.Debug($"Particle limit check - destroying particle system for {count} burst maxCount");
                            DestroyMe();
                        }
                    }
                }

                if (!options.Collision && collisionMod.collidesWith.value == 1024)
                {
                    Log.Debug("Particle limit check - disabling PlayerLocal collision");
                    collisionMod.enabled = false;
                }
                if (collisionMod.maxKillSpeed > options.MaxKill || collisionMod.maxKillSpeed < options.MinKill)
                {
                    Log.Debug($"Particle limit check - disabling collision for {collisionMod.maxKillSpeed} maxKillSpeed");
                    collisionMod.enabled = false;
                }

                if (options.LightSizeConstraint)
                {
                    Log.Debug($"Particle limit check - lights on particle system have had their size constrained");
                    lightsMod.sizeAffectsRange = false;
                }

                if (collisionMod.maxCollisionShapes > 256)
                {
                    Log.Debug($"Particle limit check - destroyed particle system for {collisionMod.maxCollisionShapes} maxCollisionShapes");
                    UnityEngine.Object.Destroy(systems[i]);
                }
            }
        }

        public static bool IsShaderBlocked(string name)
        {
            if (m_shaderList.Any(s => s.Name == name))
            {
                ShaderInfo shader = m_shaderList.First(s => s.Name == name);
                return shader.Blocked;
            }

            return false;
        }

        public static void BlockShader(string name)
        {
            ShaderInfo shader = new ShaderInfo()
            {
                Name = name,
                Blocked = true
            };

            if (m_shaderList.Any(s => s.Name == name))
            {
                shader = m_shaderList.First(s => s.Name == name);
                shader.Blocked = true;
            }

            SaveShaderList();
        }

        public static void UnblockShader(string name)
        {
            ShaderInfo shader = new ShaderInfo()
            {
                Name = name,
                Blocked = false
            };

            if (m_shaderList.Any(s => s.Name == name))
            {
                shader = m_shaderList.First(s => s.Name == name);
                shader.Blocked = false;
            }

            SaveShaderList();
        }

        private static Shader m_poiShader = null;

        private static void EnforceShaderBlacklist(GameObject avatar)
        {
            ComponentBlacklist components = Config.ComponentBlacklist;

            Shader standard = Shader.Find("Standard");

            foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.materials;

                Material poiMat = materials.FirstOrDefault(m => m.shader.name.ToLowerInvariant().Contains(".poiyomi/master/opaque"));

                if (poiMat != null && m_poiShader == null)
                {
                    m_poiShader = poiMat.shader;
                }

                if (components.MaterialsEnabled)
                {
                    for (int x = 0; x < materials.Length; x++)
                    {
                        ShaderInfo shader = new ShaderInfo()
                        {
                            Name = materials[x].shader.name.ToLowerInvariant(),
                            Blocked = false
                        };

                        if (!m_shaderList.Any(s => s.Name == shader.Name))
                        {
                            m_shaderList.Add(shader);
                        }

                        if (!components.ShadersEnabled)
                        {
                            UnityEngine.Object.Destroy(materials[x].shader);
                        }
                        else
                        {
                            ShaderInfo target = m_shaderList.First(s => s.Name == shader.Name);

                            if (materials[x].shader.name.ToLowerInvariant().Contains("cubedparadox/flat lit toon") && components.ReplaceWithPoiyomi)
                            {
                                Material mat = materials[x];
                                Texture mainTex = mat.GetTexture("_MainTex");
                                Texture bumpTex = mat.GetTexture("_BumpMap");
                                Color col = mat.GetColor("_Color");
                                float cutoff = mat.GetFloat("_Cutoff");

                                if (m_poiShader != null)
                                {
                                    Material newMat = new Material(m_poiShader); // use reference to poi's master
                                    newMat.SetTexture("_MainTex", mainTex);
                                    newMat.SetTexture("_NormalMap", bumpTex);
                                    newMat.SetColor("_Color", col);
                                    newMat.SetFloat("_Lit", 1.0f);
                                    newMat.SetFloat("_Clip", cutoff);

                                    Log.Debug($"Replaced {mat.name} ({mat.shader.name}) with poiyomi's master shader");

                                    materials[x].shader = m_poiShader;
                                    materials[x] = newMat;
                                }
                                else
                                {
                                    Log.Debug("poi's shader is null, skipping replacement");
                                }
                            }
                            else if (target.Blocked)
                            {
                                if (!components.ReplaceInsteadDelete)
                                {
                                    Log.Debug($"Shader check - destroying an object for shader {shader.Name}");
                                    UnityEngine.Object.Destroy(renderer);

                                    break;
                                }
                                else
                                {
                                    Log.Debug($"Shader check - replacing shader {shader.Name} on object {renderer.name}");
                                    materials[x].shader = standard;
                                }
                            }
                        }

                        if (materials[x].shader != null && shader.Name == "standard" && components.ReplaceStandard)
                        {
                            materials[x].shader = standard;
                        }
                    }

                    new Thread(() =>
                    {
                        SaveShaderList();
                    }).Start();
                }
                else
                {
                    foreach (Material mat in materials)
                    {
                        UnityEngine.Object.Destroy(mat);
                    }
                }
            }
        }

        private static void SaveShaderList()
        {
            m_fileSystem.SaveJson("Shaders.json", m_shaderList);
        }

        public static void LoadShaderList()
        {
            m_shaderList = m_fileSystem.LoadJson<List<ShaderInfo>>("Shaders.json");
        }

        private static void RemoveGeneralComponents(GameObject avatar, bool local, bool friend)
        {
            ComponentBlacklist blacklist = Config.ComponentBlacklist;

            if (blacklist.IgnoreFriends && friend)
            {
                Log.Debug("Component limit - skipping scan because user is a friend");
                return;
            }

            if (blacklist.IgnoreSelf && local)
            {
                Log.Debug("Component limit - skipping scan because user is local");
                return;
            }

            void DestroyAll(string type, Component[] components)
            {
                Log.Debug($"Component limit - destroyed {components.Length} {type} objects");

                for (int i = 0; i < components.Length; i++)
                {
                    UnityEngine.Object.Destroy(components[i]);
                }
            }

            if (!blacklist.AudioSourcesEnabled)
            {
                DestroyAll("AudioSource", avatar.GetComponentsInChildren<AudioSource>(true));
            }

            if (!blacklist.ParticleSystemsEnabled)
            {
                DestroyAll("ParticleSystem", avatar.GetComponentsInChildren<ParticleSystem>(true));
            }

            if (!blacklist.StandardAnimatorEnabled)
            {
                DestroyAll("Animator", avatar.GetComponentsInChildren<Animator>(true));
            }

            if (!blacklist.LegacyAnimatorEnabled)
            {
                DestroyAll("Animation", avatar.GetComponentsInChildren<Animation>(true));
            }

            if (!blacklist.MeshFiltersEnabled)
            {
                DestroyAll("MeshFilters", avatar.GetComponentsInChildren<MeshFilter>(true));
            }

            if (!blacklist.SpringJointEnabled)
            {
                DestroyAll("SpringJoint", avatar.GetComponentsInChildren<SpringJoint>(true));
            }

            if (!blacklist.ClothEnabled)
            {
                DestroyAll("Cloth", avatar.GetComponentsInChildren<Cloth>(true));
            }

            if (!blacklist.DynamicBonesEnabled)
            {
                DestroyAll("DynamicBones", avatar.GetComponentsInChildren<DynamicBone>(true));
            }
        }

        private List<DynamicBone> m_myBones = new List<DynamicBone>();
        private List<DynamicBoneCollider> m_myBoneColliders = new List<DynamicBoneCollider>();
        private List<DynamicBone> m_targetBones = new List<DynamicBone>();
        private List<DynamicBoneCollider> m_targetBoneColliders = new List<DynamicBoneCollider>();
        private GameObject m_myAvatar;

        private static bool collideBones = false;

        private static void CollideBones(GameObject currentAvatar, bool local, bool friend)
        {
            if ((Instance.m_myAvatar == null || Instance.m_myAvatar != currentAvatar) && local && currentAvatar.name != "_AvatarMirrorClone" && currentAvatar.name != "_AvatarShadowClone")
            {
                Instance.m_myBones.Clear();
                Instance.m_myBoneColliders.Clear();

                Instance.m_myAvatar = currentAvatar;
            }

            if (local)
            {
                Instance.m_myBones.AddRange(currentAvatar.GetComponentsInChildren<DynamicBone>(true));
                Instance.m_myBoneColliders.AddRange(currentAvatar.GetComponentsInChildren<DynamicBoneCollider>(true));
                Log.Info($"Added {Instance.m_myBones.Count} local bones and {Instance.m_myBoneColliders.Count} local colliders");
            }
            else
            {
                Instance.m_targetBones.AddRange(currentAvatar.GetComponentsInChildren<DynamicBone>(true));
                Instance.m_targetBoneColliders.AddRange(currentAvatar.GetComponentsInChildren<DynamicBoneCollider>(true));
                Log.Info($"Added {Instance.m_targetBones.Count} remote bones and {Instance.m_targetBoneColliders.Count} remote colliders");
            }            
        }

        public static void ScanAvatar(GameObject currentAvatar, bool local, bool friend)
        {
            if (currentAvatar == null)
            {
                return;
            }

            Log.Debug($"Starting scan for {currentAvatar.name} [Me: {local}, Friend: {friend}]");

            if (collideBones)
            {
                CollideBones(currentAvatar, local, friend);
            }

            if (local && currentAvatar.name != "_AvatarMirrorClone" && currentAvatar.name != "_AvatarShadowClone")
            {
                ScanAvatar(GameObject.Find("_AvatarMirrorClone"), true, false);
                ScanAvatar(GameObject.Find("_AvatarShadowClone"), true, false);
            }

            RemovePasswordedComponents(currentAvatar);
            EnforceParticleLimits(currentAvatar);
            EnforceShaderBlacklist(currentAvatar);
            RemoveGeneralComponents(currentAvatar, local, friend);

            Log.Debug($"Scan complete.");
        }
    }
}