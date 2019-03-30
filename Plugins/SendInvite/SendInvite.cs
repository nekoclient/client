using NekoClient;
using NekoClient.Logging;
using NekoClient.Wrappers.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI;

namespace SendInvite
{
    public class SendInvite : PluginBase
    {
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(15);

            Tick += SendInvite_Tick;
        }

        public SendInvite()
        {
            Load += SendInvite_Load;
        }

        private void SendInvite_Load()
        {
            QueueCoroutine(DelayedStart());
        }

        private Button m_inviteButton;

        Button GetInviteButton(PageUserInfo pageUserInfo)
        {
            if (m_inviteButton == null)
            {
                List<VRCUiButton> componentsInChildren = pageUserInfo.GetComponentsInChildren<VRCUiButton>().ToList();

                VRCUiButton vrcuiButton = componentsInChildren.Where((button, val) =>
                    button.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text == "Invite" ||
                    button.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text == "Invite Sent"
                ).FirstOrDefault<VRCUiButton>();

                m_inviteButton = vrcuiButton.GetComponent<Button>();
            }

            return m_inviteButton;
        }

        private void SendInvite_Tick()
        {
            PageUserInfo pageUserInfo = (PageUserInfo)VRCUiManagerWrappers.Instance.GetPage("UserInterface/MenuContent/Screens/UserInfo");

            if (pageUserInfo != null && pageUserInfo.GetShown())
            {
                Button button = GetInviteButton(pageUserInfo);

                if (button != null)
                {
                    button.interactable = true;
                }
            }
        }
    }
}