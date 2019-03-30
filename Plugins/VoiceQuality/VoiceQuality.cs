using NekoClient;
using NekoClient.UI;
using NekoClient.Wrappers.Reflection;
using System;
using System.Collections;
using UnityEngine;
using VRC;

namespace VoiceQuality
{
    public class VoiceQuality : PluginBase
    {
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(10);

            InstantiateButtons();
        }

        public VoiceQuality()
        {
            Load += VoiceQuality_Load;
            Unload += VoiceQuality_Unload;
        }

        private void VoiceQuality_Load()
        {
            QueueCoroutine(DelayedStart());
        }

        private BitRate m_originalBitrate;
        private BandMode m_originalBandMode;
        private bool m_bitrateSwitched = false;
        private bool m_gainSwitched = false;

        private GameObject m_uiBitrate;
        private GameObject m_uiMicGain;

        private void InstantiateButtons()
        {
            m_uiBitrate = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetUIElementsBase(), VrcOverrideUI.GetUIElementsBase().transform.parent.gameObject, "BITRATE:\n<color=red>OFF</color>", () =>
            {
                m_bitrateSwitched = !m_bitrateSwitched;

                FuckupBitrate(m_bitrateSwitched);

                if (m_bitrateSwitched)
                {
                    VrcOverrideUI.ChangeUIButtonText(m_uiBitrate, "BITRATE:\n<color=green>ON</color>");
                }
                else
                {
                    VrcOverrideUI.ChangeUIButtonText(m_uiBitrate, "BITRATE:\n<color=red>OFF</color>");
                }

            }, new[] { Vector2.up, Vector2.left });

            m_uiMicGain = VrcOverrideUI.InstantiateUIButton(VrcOverrideUI.GetUIElementsBase(), VrcOverrideUI.GetUIElementsBase().transform.parent.gameObject, "GAIN:\n<color=red>OFF</color>", () =>
            {
                m_gainSwitched = !m_gainSwitched;

                FuckupGain(m_gainSwitched);

                if (m_gainSwitched)
                {
                    VrcOverrideUI.ChangeUIButtonText(m_uiMicGain, "GAIN:\n<color=green>ON</color>");
                }
                else
                {
                    VrcOverrideUI.ChangeUIButtonText(m_uiMicGain, "GAIN:\n<color=red>OFF</color>");
                }

            }, new[] { Vector2.up });
        }

        private void VoiceQuality_Unload()
        {
            UnityEngine.Object.DestroyImmediate(m_uiBitrate);
            UnityEngine.Object.DestroyImmediate(m_uiMicGain);
        }

        private void FuckupBitrate(bool enable)
        {
            Player p = PlayerWrappers.GetLocalPlayer();

            if (enable)
            {
                m_originalBitrate = p.vrcPlayer.USpeaker().Bitrate;
                p.vrcPlayer.USpeaker().Bitrate = BitRate.BitRate_8K;
            }
            else
            {
                p.vrcPlayer.USpeaker().Bitrate = m_originalBitrate;
            }
        }

        private void FuckupGain(bool enable)
        {
            Player p = PlayerWrappers.GetLocalPlayer();

            if (enable)
            {
                USpeaker.LocalGain = float.MaxValue;
                USpeaker.RemoteGain = float.MaxValue;

                m_originalBandMode = p.vrcPlayer.USpeaker().BandwidthMode;
                p.vrcPlayer.USpeaker().BandwidthMode = BandMode.Narrow;
            }
            else
            {
                USpeaker.LocalGain = 1.0f;
                USpeaker.RemoteGain = 1.0f;
                p.vrcPlayer.USpeaker().BandwidthMode = m_originalBandMode;
            }
        }
    }
}