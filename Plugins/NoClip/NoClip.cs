using NekoClient;
using NekoClient.UI;
using NekoClient.Wrappers.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC;
using static NekoClient.Wrappers.Reflection.VRCInputManagerWrappers;

namespace NoClip
{
// TODO: make a working noclip for Vive users
#if OCULUS
    internal enum Oculus
    {
        AButton = KeyCode.JoystickButton0,
        BButton = KeyCode.JoystickButton1,
        XButton = KeyCode.JoystickButton2,
        YButton = KeyCode.JoystickButton3,
        Start = KeyCode.JoystickButton7,
        LeftThumbstickPress = KeyCode.JoystickButton8,
        RightThumbstickPress = KeyCode.JoystickButton9,
        LeftTrigger = KeyCode.JoystickButton14,
        RightTrigger = KeyCode.JoystickButton15,
        LeftThumbstickTouch = KeyCode.JoystickButton16,
        RightThumbstickTouch = KeyCode.JoystickButton17,
        LeftThumbRestTouch = KeyCode.JoystickButton18,
        RightThumbRestTouch = KeyCode.JoystickButton19
    }

    public class NoClip : PluginBase
    {
        private float m_lastTime = 0;
        private bool m_airbreakActive = false;
        private Vector3 m_position = new Vector3(-1, -1, -1);
        private VRC.Player m_localPlayer;
        private float m_currentSpeed = 10.0f;
        private int m_speedIndex = 3;
        private Vector3 m_originalGravity = new Vector3(-1, -1, -1);

        private List<float> m_speeds = new List<float>()
        {
            0.5f,
            1.0f,
            5.0f,
            10.0f,
            20.0f,
            50.0f,
            100.0f
        };

        private GameObject m_uiNoClip;

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            m_uiNoClip = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetUIElementsBase(), VrcOverrideUI.GetUIElementsBase().transform.parent.gameObject, "NoClip:\n<color=red>OFF</color>", () =>
            {
                if (m_airbreakActive)
                {
                    DisableAirbreak();
                }
                else
                {
                    SetupAirbreak();
                }

                m_airbreakActive = !m_airbreakActive;
            }, new[] { Vector2.left });
        }

        public NoClip()
        {
            Tick += Update;
            Gui += OnGUI;
            Unload += NoClip_Unload;

            QueueCoroutine(DelayedStart());
        }

        private void NoClip_Unload()
        {
            UnityEngine.Object.DestroyImmediate(m_uiNoClip);
        }

        private void IgnoreCollision(bool ignore)
        {
            List<Collider> colliders = GameObject.FindObjectsOfType<Collider>().ToList();

            foreach (Collider c in colliders)
            {
                Physics.IgnoreCollision(GameObject.FindWithTag("Player").transform.GetComponent<Collider>(), c, ignore);
            }
        }

        private void SetupAirbreak()
        {
            IgnoreCollision(true);
            VrcOverrideUI.ChangeUIButtonText(m_uiNoClip, "NoClip:\n<color=green>ON</color>");
            m_position = m_localPlayer.transform.position;
            m_originalGravity = Physics.gravity;
            Vector3 newGravity = m_originalGravity;
            newGravity.y = 0;
            Physics.gravity = newGravity;
        }

        private void DisableAirbreak()
        {
            IgnoreCollision(false);
            VrcOverrideUI.ChangeUIButtonText(m_uiNoClip, "NoClip:\n<color=red>OFF</color>");
            Physics.gravity = m_originalGravity;
        }

        private void Update()
        {
            if (!RoomManagerBaseWrappers.InRoom)
            {
                return;
            }

            if (m_localPlayer == null)
            {
                VRC.Player player = PlayerWrappers.GetLocalPlayer();

                if (player != null) // early init I guess
                {
                    m_localPlayer = player;
                }
            }

            if (m_localPlayer == null)
            {
                return; // wait until player is ready
            }

            bool controller = VRCInputManagerWrappers.LastInputMethod == InputMethod.Controller;
            bool oculus = VRCInputManagerWrappers.LastInputMethod == InputMethod.Oculus;
            bool desktop = (VRCInputManagerWrappers.LastInputMethod == InputMethod.Keyboard || VRCInputManagerWrappers.LastInputMethod == InputMethod.Mouse);

            bool isActiveController = controller && Input.GetKey(KeyCode.JoystickButton5);
            bool isActiveOculus = oculus && Input.GetKey((KeyCode)Oculus.LeftThumbstickPress);
            bool isActiveDesktop = desktop && (Input.GetKey(KeyCode.Mouse4) || Input.GetKey(KeyCode.RightControl));

            bool swapSpeedsController = controller && Input.GetKey(KeyCode.JoystickButton9);
            bool swapSpeedsOculus = oculus && Input.GetKey((KeyCode)Oculus.AButton);
            bool swapSpeedsKeyboard = desktop && Input.GetKey(KeyCode.LeftShift);

            bool isActive = isActiveController || isActiveOculus || isActiveDesktop;
            bool swapSpeeds = swapSpeedsKeyboard || swapSpeedsController || swapSpeedsOculus;

            if (isActive && Time.time - m_lastTime > 1f)
            {
                if (m_airbreakActive)
                {
                    DisableAirbreak();
                }
                else
                {
                    SetupAirbreak();
                }

                m_airbreakActive = !m_airbreakActive;

                m_lastTime = Time.time;
            }

            if (swapSpeeds && m_airbreakActive && Time.time - m_lastTime > 0.2f)
            {
                m_speedIndex += 1;

                if (m_speedIndex > m_speeds.Count() - 1)
                {
                    m_speedIndex = 0;
                }

                m_currentSpeed = m_speeds[m_speedIndex];
                m_lastTime = Time.time;
            }

            // get default fallback
            VRCVrCameraOculus[] ctrls = ((VRCVrCameraOculus[])UnityEngine.Object.FindObjectsOfType(typeof(VRCVrCameraOculus)));

            Transform trans = null;

            if (ctrls.Length > 0)
            {
                trans = ctrls[0].transform;
            }

            // alright so
            // let's start by getting our current vrcPlayer
            VRCPlayer vrcPlayer = PlayerWrappers.GetLocalPlayer().vrcPlayer;

            Animator animator = null;

            if (vrcPlayer == null)
            {
                animator = null;
            }
            else
            {
                // let's get our avatar manager
                VRCAvatarManager vrcAvatarManager = vrcPlayer.AvatarManager();

                if (vrcAvatarManager == null)
                {
                    animator = null;
                }
                else
                {
                    // current avatar
                    GameObject currentAvatar = vrcAvatarManager.currentAvatarObject;
                    animator = ((currentAvatar != null) ? currentAvatar.GetComponent<Animator>() : null);
                }
            }

            // if the animator is not null at this stage and airbreak is enabled
            if (animator != null)
            {
                // get the head bone
                Transform tempTrans = animator.GetBoneTransform(HumanBodyBones.Head);

                // if we're humanoid
                if (tempTrans != null)
                {
                    // use the head bone's transform instead of oculus camera
                    trans = tempTrans;
                }
            }

            if (trans == null)
            {
                return;
            }

            if (Input.GetKey(KeyCode.W))
            {
                m_position += trans.forward * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                m_position += (trans.right * -1) * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                m_position += (trans.forward * -1) * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                m_position += trans.right * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.E))
            {
                m_position += m_localPlayer.transform.up * m_currentSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                m_position += (m_localPlayer.transform.up * -1) * m_currentSpeed * Time.deltaTime;
            }

            if ((Input.GetAxis("Joy1 Axis 2") != 0))
            {
                m_position += trans.forward * m_currentSpeed * Time.deltaTime * (Input.GetAxis("Joy1 Axis 2") * -1);
            }

            if (Input.GetAxis("Joy1 Axis 1") != 0)
            {
                m_position += trans.right * m_currentSpeed * Time.deltaTime * Input.GetAxis("Joy1 Axis 1");
            }

            if (m_airbreakActive)
            {
                m_localPlayer.transform.position = m_position;
            }  
        }

        private void OnGUI()
        {
            if (m_airbreakActive)
            {
                GUI.Label(new Rect(25, 25, 320, 30), $"NOCLIP ACTIVE");
                GUI.Label(new Rect(25, 50, 320, 30), $"SPEED: {m_currentSpeed.ToString()}");
                GUI.Label(new Rect(25, 75, 320, 30), $"POSITION: {m_position.ToString()}");
            }
        }
    }
#endif
}