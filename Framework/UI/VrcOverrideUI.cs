using NekoClient.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NekoClient.UI
{
    public class VrcOverrideUI : PluginBase
    {
        public VrcOverrideUI()
        {
        }

        private static List<GameObject> m_instantiatedButtons = new List<GameObject>();

        private IEnumerator DestroyButtons()
        {
            foreach (GameObject button in m_instantiatedButtons)
            {
                GameObject.DestroyImmediate(button);
            }

            m_instantiatedButtons.Clear();

            yield return new WaitForEndOfFrame();
        }

        private static string Colourise(string txt, string colour)
            => $"<color={colour}>{txt}</color>";

        private static string Blue(string txt)
            => Colourise(txt, "#348BD5");

        public static void ChangeUIButtonText(GameObject button, string text)
        {
            text = Blue(text);

            foreach (Text textObject in button.GetComponentsInChildren<Text>(true))
            {
                textObject.text = text;
                textObject.color = Color.white;
            }
        }

        private static Vector3 m_vrScale = new Vector3(0.0002f, 0.0002f, 0.0002f);
        private static Vector3 m_nonVrScale = new Vector3(0.0003f, 0.0003f, 0.0003f);


        private static IEnumerator ExpandDonger()
        {
            QuickMenu menu = GetQuickMenu();
            RectTransform trans = menu.GetComponent<RectTransform>();

            if (trans.localScale == m_vrScale && VRCTrackingManager.IsInVRMode())
            {
                trans.localScale = trans.localScale + (trans.localScale / 2);
            }
            else if (trans.localScale == m_nonVrScale && !VRCTrackingManager.IsInVRMode())
            {
                trans.localScale = trans.localScale + (trans.localScale / 2);
            }

            yield return new WaitForSeconds(0.05f);
        }


        private static bool m_backgroundExpanded = false;

        private static void ExpandBackgroundRect()
        {
            PluginBase.QueueCoroutine(ExpandDonger());
        }

        public static GameObject InstantiateUIButton(GameObject button, GameObject parent, string text, Action onClick, Vector2[] positionChanges)
        {
            if (!m_backgroundExpanded)
            {
                ExpandBackgroundRect();

                m_backgroundExpanded = true;
            }

            text = Blue(text);

            try
            {
                if (parent != null)
                {
                    button = GameObject.Instantiate(button, parent.transform);
                }

                foreach (Text textObject in button.GetComponentsInChildren<Text>(true))
                {
                    textObject.text = text;
                    textObject.color = Color.white;
                }

                foreach (Button buttonObject in button.GetComponentsInChildren<Button>(true))
                {
                    buttonObject.onClick = new Button.ButtonClickedEvent();
                }

                RectTransform buttonTransform = button.GetComponent<RectTransform>();
                positionChanges.ToList().ForEach(x => buttonTransform.anchoredPosition += x * 420);
                button.GetComponent<Button>().onClick.AddListener(new UnityAction(() => onClick()));

                m_instantiatedButtons.Add(button);

                return button;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public static QuickMenu GetQuickMenu()
        {
            return Resources.FindObjectsOfTypeAll<QuickMenu>()[0];
        }

        public static GameObject GetUIElementsBase()
        {
            QuickMenu quickMenu = GetQuickMenu();
            GameObject uiElementsMenu = quickMenu.transform.Find("UIElementsMenu").gameObject;

            return uiElementsMenu.transform.Find("BackButton").gameObject;
        }

        public static GameObject GetPlayerDetailsBase()
        {
            QuickMenu quickMenu = GetQuickMenu();
            GameObject playerSelectMenu = quickMenu.transform.Find("UserInteractMenu").gameObject;

            return playerSelectMenu.transform.Find("BackButton").gameObject;
        }
    }
}