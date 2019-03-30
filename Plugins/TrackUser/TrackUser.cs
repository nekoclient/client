using NekoClient;
using NekoClient.UI;
using NekoClient.Wrappers.Reflection;
using System.Collections;
using UnityEngine;
using VRC;
using VRC.Core;

namespace TrackUser
{
    public class TrackUser : PluginBase
    {
        private GameObject m_uiTrackUser;
        private GameObject m_uiStopTracking;

        private void InstantiateStopTrackingButton()
        {
            m_uiStopTracking = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetUIElementsBase(), VrcOverrideUI.GetUIElementsBase().transform.parent.gameObject, "STOP\nTRACKING", () =>
            {
                if (trackingUser)
                {
                    trackingUser = !trackingUser;
                    trackedUser = null;
                    VrcOverrideUI.ChangeUIButtonText(m_uiTrackUser, "TRACK");
                    Object.Destroy(m_uiStopTracking);
                }

            }, new[] { Vector2.left, Vector2.left });
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            Tick += TrackUser_Tick;

            m_uiTrackUser = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetPlayerDetailsBase(), VrcOverrideUI.GetPlayerDetailsBase().transform.parent.gameObject, "TRACK", () =>
            {
                trackingUser = !trackingUser;

                if (trackingUser)
                {
                    APIUser targetUser = QuickMenuWrappers.SelectedUser;
                    trackedUser = PlayerManager.GetPlayer(targetUser.id);
                    self = PlayerWrappers.GetLocalPlayer();

                    VrcOverrideUI.ChangeUIButtonText(m_uiTrackUser, "[TRACKING]");
                    InstantiateStopTrackingButton();
                }
                else
                {
                    trackedUser = null;
                    VrcOverrideUI.ChangeUIButtonText(m_uiTrackUser, "TRACK");
                }

            }, new[] { Vector2.left });
        }

        public TrackUser()
        {
            Load += TrackUser_Load;
            Unload += TrackUser_Unload;
        }

        private void TrackUser_Load()
        {
            QueueCoroutine(DelayedStart());
        }

        private void TrackUser_Unload()
        {
            Object.DestroyImmediate(m_uiTrackUser);
            Object.DestroyImmediate(m_uiStopTracking);
        }

        private bool trackingUser = false;
        private Player trackedUser;
        private Player self;

        private void TrackUser_Tick()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                trackingUser = !trackingUser;

                if (trackingUser)
                {
                    APIUser targetUser = QuickMenuWrappers.SelectedUser;
                    trackedUser = PlayerManager.GetPlayer(targetUser.id);
                    self = PlayerWrappers.GetLocalPlayer();
                }
            }

            if (trackedUser == null && trackingUser)
            {
                trackingUser = false;
            }

            if (trackingUser)
            {
                self.transform.position = trackedUser.transform.position;
                self.transform.rotation = trackedUser.transform.rotation;
            }
        }
    }
}