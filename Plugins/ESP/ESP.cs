using NekoClient;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using NekoClient.Wrappers.Reflection;

namespace ESP
{
    public class ESP : PluginBase
    {
        public ESP()
        {
            Tick += ESP_Tick;
            Gui += ESP_Gui;
            RenderObject += ESP_RenderObject;
        }

        private float m_lastTime = 0;
        private bool m_enabled = false;
        private bool m_vrEnabled = false;

        private GUIStyle m_style;

        private List<Player> m_players = new List<Player>();

        private float m_lastPlayerUpdate = 0;

        private void UpdatePlayers()
        {
            if (Time.time - m_lastPlayerUpdate > 1f)
            {
                m_players = PlayerManager.GetAllPlayers().ToList();

                m_players.Remove(m_players.First(p => p.name.Contains("VRCPlayer[Local]")));

                m_lastPlayerUpdate = Time.time;
            }
        }

        private void ESP_Tick()
        {
            if (!RoomManagerBaseWrappers.InRoom)
            {
                return;
            }

            if (Input.GetKey(KeyCode.Tab) && Time.time - m_lastTime > 0.2f)
            {
                m_enabled = !m_enabled;

                m_lastTime = Time.time;
            }

            // QuickMenu.Instance._useLeftHand.held
            // found above `vrcuiCursor = VRCUiCursorManager.GetInstance().handLeftCursor;` in QuickMenu
            // private by default
            if (Input.GetAxis("Joy1 Axis 9") > 0.05f && VRCTrackingManager.IsInVRMode() || Input.GetKey(KeyCode.Alpha9))
            {
                if (!m_vrEnabled)
                {
                    m_vrEnabled = true;
                }
            }
            else
            {
                if (m_vrEnabled)
                {
                    //m_vrJustDisabled = true;
                    m_vrEnabled = false;
                }
            }

            if (m_enabled || m_vrEnabled)
            {
                UpdatePlayers();
            }
        }

        private Texture2D m_texture;

        private Dictionary<string, GameObject> m_names = new Dictionary<string, GameObject>();

        private void SetupStyle()
        {
            m_style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Normal
            };
            m_style.normal.textColor = new Color32(255, 255, 255, 255);

            m_texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            m_texture.SetPixel(0, 0, new Color32(0, 0, 0, 200));
            m_texture.Apply();

            m_mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            m_mat.SetInt("_SrcBlend", 5);
            m_mat.SetInt("_DstBlend", 10);
            m_mat.SetInt("_Cull", 0);
            m_mat.SetInt("_ZWrite", 0);
        }

        private float InvertY(float y)
        {
            return VRCVrCamera.GetInstance().screenCamera.pixelHeight - y;
        }

        private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        private Color32 GetGroupColour(Player p)
        {
            string id = p.ApiUser().id;

            if (p.IsFriend())
            {
                return Color.cyan;
            }

            return new Color32(255, 255, 255, 255);
        }

        private bool m_styleSetup = false;

        private Material m_mat = new Material(Shader.Find("Particles/Alpha Blended"));

        private void ESP_RenderObject()
        {
            if (m_enabled || m_vrEnabled)
            {
                if (!m_styleSetup)
                {
                    SetupStyle();
                    m_styleSetup = !m_styleSetup;
                }

                foreach (Player p in m_players)
                {
                    VRCPlayer vrcPlayer = p.vrcPlayer;
                    Animator animator;

                    if (vrcPlayer == null)
                    {
                        animator = null;
                    }
                    else
                    {
                        VRCAvatarManager vrcAvatarManager = vrcPlayer.AvatarManager();
                        if (vrcAvatarManager == null)
                        {
                            animator = null;
                        }
                        else
                        {
                            GameObject currentAvatar = vrcAvatarManager.currentAvatarObject;
                            animator = ((currentAvatar != null) ? currentAvatar.GetComponent<Animator>() : null);
                        }
                    }

                    if (animator != null)
                    {
                        void DrawBoneEsp(HumanBodyBones bone1, HumanBodyBones bone2)
                        {
                            try
                            {
                                GL.Begin(GL.LINES);
                                m_mat.SetPass(0);
                                GL.Color(GetGroupColour(p));
                                GL.Vertex(animator.GetBoneTransform(bone1).position);
                                GL.Vertex(animator.GetBoneTransform(bone2).position/* + new Vector3(0, .5f, 0)*/);
                            }
                            catch { }
                            finally
                            {
                                GL.End();
                            }
                        }

                        /*EspType espType = ConfigFile.Config.Common.EspType;

                        if (espType == EspType.Cute)
                        {
                            for (int j = 0; j < 56; j++)
                            {
                                if (j == 0)
                                {
                                    continue;
                                }

                                DrawBoneEsp((HumanBodyBones)j - 1, (HumanBodyBones)j);
                            }
                        }
                        else if (espType == EspType.Bone)*/
                        {
                            // hips to left toes
                            DrawBoneEsp(HumanBodyBones.Hips, HumanBodyBones.LeftUpperLeg);
                            DrawBoneEsp(HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg);
                            DrawBoneEsp(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);
                            DrawBoneEsp(HumanBodyBones.LeftFoot, HumanBodyBones.LeftToes);

                            // hips to right toes
                            DrawBoneEsp(HumanBodyBones.Hips, HumanBodyBones.RightUpperLeg);
                            DrawBoneEsp(HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg);
                            DrawBoneEsp(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
                            DrawBoneEsp(HumanBodyBones.RightFoot, HumanBodyBones.RightToes);

                            // hips to head
                            DrawBoneEsp(HumanBodyBones.Hips, HumanBodyBones.Spine);
                            DrawBoneEsp(HumanBodyBones.Spine, HumanBodyBones.Chest);
                            DrawBoneEsp(HumanBodyBones.Chest, HumanBodyBones.Neck);
                            DrawBoneEsp(HumanBodyBones.Neck, HumanBodyBones.Head);

                            // chest to left hand
                            DrawBoneEsp(HumanBodyBones.Chest, HumanBodyBones.LeftShoulder);
                            DrawBoneEsp(HumanBodyBones.LeftShoulder, HumanBodyBones.LeftUpperArm);
                            DrawBoneEsp(HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm);
                            DrawBoneEsp(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);

                            // chest to right hand
                            DrawBoneEsp(HumanBodyBones.Chest, HumanBodyBones.RightShoulder);
                            DrawBoneEsp(HumanBodyBones.RightShoulder, HumanBodyBones.RightUpperArm);
                            DrawBoneEsp(HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm);
                            DrawBoneEsp(HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);

                            // right fingers
                            // thumb
                            DrawBoneEsp(HumanBodyBones.RightHand, HumanBodyBones.RightThumbProximal);
                            DrawBoneEsp(HumanBodyBones.RightThumbProximal, HumanBodyBones.RightThumbIntermediate);
                            DrawBoneEsp(HumanBodyBones.RightThumbIntermediate, HumanBodyBones.RightThumbDistal);

                            // index
                            DrawBoneEsp(HumanBodyBones.RightHand, HumanBodyBones.RightIndexProximal);
                            DrawBoneEsp(HumanBodyBones.RightIndexProximal, HumanBodyBones.RightIndexIntermediate);
                            DrawBoneEsp(HumanBodyBones.RightIndexIntermediate, HumanBodyBones.RightIndexDistal);

                            // middle
                            DrawBoneEsp(HumanBodyBones.RightHand, HumanBodyBones.RightMiddleProximal);
                            DrawBoneEsp(HumanBodyBones.RightMiddleProximal, HumanBodyBones.RightMiddleIntermediate);
                            DrawBoneEsp(HumanBodyBones.RightMiddleIntermediate, HumanBodyBones.RightMiddleDistal);

                            // ring
                            DrawBoneEsp(HumanBodyBones.RightHand, HumanBodyBones.RightRingProximal);
                            DrawBoneEsp(HumanBodyBones.RightRingProximal, HumanBodyBones.RightRingIntermediate);
                            DrawBoneEsp(HumanBodyBones.RightRingIntermediate, HumanBodyBones.RightRingDistal);

                            // little
                            DrawBoneEsp(HumanBodyBones.RightHand, HumanBodyBones.RightLittleProximal);
                            DrawBoneEsp(HumanBodyBones.RightLittleProximal, HumanBodyBones.RightLittleIntermediate);
                            DrawBoneEsp(HumanBodyBones.RightLittleIntermediate, HumanBodyBones.RightLittleDistal);

                            // left fingers
                            // thumb
                            DrawBoneEsp(HumanBodyBones.LeftHand, HumanBodyBones.LeftThumbProximal);
                            DrawBoneEsp(HumanBodyBones.LeftThumbProximal, HumanBodyBones.LeftThumbIntermediate);
                            DrawBoneEsp(HumanBodyBones.LeftThumbIntermediate, HumanBodyBones.LeftThumbDistal);

                            // index
                            DrawBoneEsp(HumanBodyBones.LeftHand, HumanBodyBones.LeftIndexProximal);
                            DrawBoneEsp(HumanBodyBones.LeftIndexProximal, HumanBodyBones.LeftIndexIntermediate);
                            DrawBoneEsp(HumanBodyBones.LeftIndexIntermediate, HumanBodyBones.LeftIndexDistal);

                            // middle
                            DrawBoneEsp(HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleProximal);
                            DrawBoneEsp(HumanBodyBones.LeftMiddleProximal, HumanBodyBones.LeftMiddleIntermediate);
                            DrawBoneEsp(HumanBodyBones.LeftMiddleIntermediate, HumanBodyBones.LeftMiddleDistal);

                            // ring
                            DrawBoneEsp(HumanBodyBones.LeftHand, HumanBodyBones.LeftRingProximal);
                            DrawBoneEsp(HumanBodyBones.LeftRingProximal, HumanBodyBones.LeftRingIntermediate);
                            DrawBoneEsp(HumanBodyBones.LeftRingIntermediate, HumanBodyBones.LeftRingDistal);

                            // little
                            DrawBoneEsp(HumanBodyBones.LeftHand, HumanBodyBones.LeftLittleProximal);
                            DrawBoneEsp(HumanBodyBones.LeftLittleProximal, HumanBodyBones.LeftLittleIntermediate);
                            DrawBoneEsp(HumanBodyBones.LeftLittleIntermediate, HumanBodyBones.LeftLittleDistal);
                        }
                    }
                }
            }

            if (m_vrEnabled)
            {
                foreach (Player p in m_players)
                {
                    if (p == null || p.ApiUser() == null)
                    {
                        continue;
                    }

                    string displayName = p.ApiUser().displayName;

                    // draw player tag
                    if (!m_names.ContainsKey(displayName))
                    {
                        GameObject obj = new GameObject();
                        UnityEngine.Object.DontDestroyOnLoad(obj);
                        Text t = obj.AddComponent<Text>();

                        t.text = displayName;
                        t.color = GetGroupColour(p);

                        m_names[displayName] = obj;
                    }
                    m_names[displayName].transform.LookAt(VRCVrCamera.GetInstance().screenCamera.transform.position);
                    m_names[displayName].transform.position = p.transform.position;

                    GameObject myAvatar = PlayerWrappers.GetLocalPlayer().vrcPlayer.AvatarManager().currentAvatarObject;
                    Animator myAnimator = ((myAvatar != null) ? myAvatar.GetComponent<Animator>() : null);
                    Vector3 myPos = PlayerWrappers.GetLocalPlayer().transform.position;

                    if (myAnimator != null)
                    {
                        myPos = myAnimator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).position;

                        if (myPos == null)
                        {
                            myPos = myAnimator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate).position;
                        }

                        if (myPos == null)
                        {
                            myPos = myAnimator.GetBoneTransform(HumanBodyBones.LeftIndexProximal).position;
                        }

                        if (myPos == null)
                        {
                            myPos = myAnimator.GetBoneTransform(HumanBodyBones.LeftHand).position;
                        }
                    }

                    GameObject targetAvatar = p.vrcPlayer.AvatarManager().currentAvatarObject;
                    Animator targetAnimator = ((targetAvatar != null) ? targetAvatar.GetComponent<Animator>() : null);
                    Vector3 targetPos = p.transform.position;

                    if (targetAnimator != null)
                    {
                        targetPos = targetAnimator.GetBoneTransform(HumanBodyBones.Head).position;
                    }

                    GL.Begin(GL.LINES);
                    m_mat.SetPass(0);
                    GL.Color(GetGroupColour(p));
                    GL.Vertex(myPos);
                    GL.Vertex(targetPos);
                    GL.End();
                }
            }
        }

        private void ESP_Gui()
        {
            if (m_enabled)
            {
                if (!m_styleSetup)
                {
                    SetupStyle();
                    m_styleSetup = !m_styleSetup;
                }

                foreach (Player p in m_players)
                {
                    if (p == null || p.ApiUser() == null)
                    {
                        continue;
                    }

                    string CleanPlayerName(string name)
                    {
                        name = name.Replace("VRCPlayer[Local]", "").Replace("VRCPlayer[Remote]", "");

                        List<string> nameParts = name.Split(' ').ToList();

                        nameParts.RemoveAt(nameParts.Count() - 1);

                        name = string.Join(" ", nameParts.ToArray());

                        return name;
                    }

                    string targetName = CleanPlayerName(p.name);

                    // draw player tag
                    Vector3 pos = p.transform.position;
                    Vector3 playerPosScreen = VRCVrCamera.GetInstance().screenCamera.WorldToScreenPoint(pos);
                    float newY = InvertY(playerPosScreen.y);

                    if (playerPosScreen.z <= 0)
                    {
                        continue;
                    }

                    Color newColour = GetGroupColour(p);

                    m_style.normal.background = m_texture;
                    m_style.CalcMinMaxWidth(new GUIContent(targetName), out float minWidth, out float maxWidth);
                    m_style.normal.textColor = newColour;

                    GUI.Label(new Rect(playerPosScreen.x - (maxWidth / 2), newY, maxWidth + 10, 30), targetName, m_style);
                }
            }
        }
    }
}