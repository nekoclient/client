using NekoClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NekoClient.UI
{
    public class Drawing : MonoBehaviour
    {
        private class TextStorage
        {
            public string Text { get; set; }
            public GUIStyle Style { get; set; }
            public Color Color { get; set; }
            public Rect Position { get; set; }
            public int Scale { get; set; }
        }

        private class KeyCodeList
        {
            public KeyCode OpenMenu { get; set; }
            public KeyCode Accept { get; set; }
            public KeyCode Back { get; set; }
            public KeyCode Up { get; set; }
            public KeyCode Down { get; set; }
            public KeyCode Left { get; set; }
            public KeyCode Right { get; set; }
        }

        private static int m_width = 420;
        private static int m_leftPad = 24;

        private static Texture2D m_staticRectTexture;
        private static GUIStyle m_staticRectStyle;
        private static Texture2D m_staticMenuHeader;

        public static Dictionary<MenuId, SubMenu> TrackedMenus = new Dictionary<MenuId, SubMenu>();

        public static MenuId m_subMenu = MenuId.NotOpen;
        public static MenuId m_previousSubMenu = MenuId.NotOpen;

        private static int m_subMenuLevel = 0;
        private static MenuId[] m_lastSubMenu = new MenuId[20];
        private static int[] m_lastOption = new int[20];
        private static int m_maxOptionCount = 36;

        private static float m_activeY = 185;

        private static KeyCodeList m_keyCodes = new KeyCodeList();
        private static Dictionary<string, TextStorage> m_textStorage = new Dictionary<string, TextStorage>();

        private static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }

        private static void DrawRectangle(Rect position, Color color)
        {
            if (m_staticRectTexture == null)
            {
                m_staticRectTexture = new Texture2D(1, 1);
            }

            if (m_staticRectStyle == null)
            {
                m_staticRectStyle = new GUIStyle();
            }

            m_staticRectTexture.SetPixel(0, 0, color);
            m_staticRectTexture.Apply();

            m_staticRectStyle.normal.background = m_staticRectTexture;

            GUI.Box(position, GUIContent.none, m_staticRectStyle);
        }

        private static void DrawText(Rect pos, string text, int scale, Color color, bool center)
        {
            string identifier = $"{pos.y}_{scale}_{text}_{center}_{color.r}";

            TextStorage currentItem;

            if (!m_textStorage.Keys.Contains(identifier))
            {
                GUIStyle newStyle = new GUIStyle()
                {
                    alignment = (center ? TextAnchor.MiddleCenter : TextAnchor.MiddleLeft),
                    fontSize = scale,
                    fontStyle = FontStyle.Normal
                };

                newStyle.richText = true;
                newStyle.normal.textColor = color;

                currentItem = new TextStorage()
                {
                    Text = text,
                    Style = newStyle,
                    Position = pos,
                };

                m_textStorage.Add(identifier, currentItem);
            }
            else
            {
                currentItem = m_textStorage[identifier];
            }

            GUI.Label(currentItem.Position, currentItem.Text, currentItem.Style);
        }

        public static void SetMenuTriggers(KeyCode open, KeyCode accept, KeyCode back, KeyCode up, KeyCode down, KeyCode left, KeyCode right)
        {
            m_keyCodes.OpenMenu = open;
            m_keyCodes.Accept = accept;
            m_keyCodes.Back = back;
            m_keyCodes.Up = up;
            m_keyCodes.Down = down;
            m_keyCodes.Left = left;
            m_keyCodes.Right = right;
        }

        public static void ChangeSubmenu(MenuId submenu)
        {
            m_lastSubMenu[m_subMenuLevel] = m_subMenu;
            m_lastOption[m_subMenuLevel] = m_currentOption;
            m_currentOption = 1;
            m_subMenu = submenu;
            m_subMenuLevel++;
        }

        public static int m_currentOption = 0;
        public static bool m_optionPress = false;
        public static bool m_leftPress = false;
        public static bool m_rightPress = false;
        public static int m_optionCount = 0;

        private static float m_delay = 0;

        private static bool GetDPadButton(string button)
        {
            if (button == "right") return Input.GetAxis("Joy1 Axis 6") == 1;
            if (button == "left") return Input.GetAxis("Joy1 Axis 6") == -1;
            if (button == "up") return Input.GetAxis("Joy1 Axis 7") == 1;
            if (button == "down") return Input.GetAxis("Joy1 Axis 7") == -1;
            return false;
        }

        public static void HandleInput()
        {
            if (Time.time - m_delay > 0.1f)
            {
                if (m_subMenu == MenuId.NotOpen)
                {
                    if (Input.GetKey(m_keyCodes.OpenMenu) || Input.GetKey(KeyCode.JoystickButton4))
                    {
                        m_subMenu = MenuId.Main;
                        m_subMenuLevel = 0;
                        m_currentOption = 1;

                        m_delay = Time.time;
                    }
                }
                else
                {
                    if (Input.GetKey(m_keyCodes.Back) || Input.GetKey(KeyCode.JoystickButton1))
                    {
                        if (m_subMenu == MenuId.Main)
                        {
                            m_subMenu = MenuId.NotOpen;
                        }
                        else
                        {
                            m_subMenu = m_lastSubMenu[m_subMenuLevel - 1];
                            m_currentOption = m_lastOption[m_subMenuLevel - 1];
                            m_subMenuLevel--;
                        }

                        m_delay = Time.time;
                    }
                    else if (Input.GetKey(m_keyCodes.Accept) || Input.GetKey(KeyCode.JoystickButton0))
                    {
                        m_optionPress = true;

                        m_delay = Time.time;
                    }
                    else if (Input.GetKey(m_keyCodes.Up) || GetDPadButton("up"))
                    {
                        m_currentOption--;
                        m_currentOption = m_currentOption < 1 ? m_optionCount : m_currentOption;

                        m_delay = Time.time;
                    }
                    else if (Input.GetKey(m_keyCodes.Down) || GetDPadButton("down"))
                    {
                        m_currentOption++;
                        m_currentOption = m_currentOption > m_optionCount ? 1 : m_currentOption;

                        m_delay = Time.time;
                    }
                    else if (Input.GetKey(m_keyCodes.Left) || GetDPadButton("left"))
                    {
                        m_leftPress = true;

                        m_delay = Time.time;
                    }
                    else if (Input.GetKey(m_keyCodes.Right) || GetDPadButton("right"))
                    {
                        m_rightPress = true;

                        m_delay = Time.time;
                    }
                }
            }
        }

        private static float DiffTrack(float tgt, float cur, float rate, float deltaTime)
        {
            float diff = tgt - cur;
            float step = (diff * rate * deltaTime);

            if (Math.Abs(diff) <= 0.00001f)
            {
                return cur;
            }

            if (Math.Abs(step) > Math.Abs(diff))
            {
                return cur;
            }

            return cur + step;
        }

        private static Color32 m_backgroundColour = new Color32(45, 45, 47, 200);

        public static void StyleMenu()
        {
            int totalRemainingHeight = Screen.height;
            int fromTop = 180;

            int CalculateRemainder(ref int target, int amount)
            {
                target -= amount;

                return amount;
            }

            DrawRectangle(new Rect(0, 0, m_width, CalculateRemainder(ref totalRemainingHeight, fromTop)), m_backgroundColour);

            if (m_staticMenuHeader == null)
            {
                m_staticMenuHeader = LoadPNG(new FileSystem("NekoClient\\Assets").GetFilePath("Neko.png"));
            }

            GUI.DrawTexture(new Rect(0, 0, m_staticMenuHeader.width, m_staticMenuHeader.height), m_staticMenuHeader);

            DrawRectangle(new Rect(0, fromTop, m_width, totalRemainingHeight), m_backgroundColour);

            // TODO: some sort of graphic for when there's too many options to display on same screen

            if (m_currentOption <= m_maxOptionCount)
            {
                m_activeY = DiffTrack(((m_currentOption * 20) + (185 + 24)), m_activeY, 15.0f, Time.deltaTime);
            }
            else
            {
                m_activeY = ((m_maxOptionCount * 20) + (185 + 24));
            }

            DrawRectangle(new Rect(0, m_activeY, 12, 20), new Color32(120, 112, 193, 255));
        }

        public static void SetMenuTitle(string title)
        {
            DrawText(new Rect(0, 185, 420, 24), title, 24, Color.white, true);
        }

        public static bool IsEntryPressed(int entry)
        {
            return m_currentOption == entry && m_optionPress;
        }

        public static bool IsCurrentEntry(int entry)
        {
            return m_currentOption == entry;
        }

        public static int AddMenuEntry(string option, bool isTextField = false)
        {
            m_optionCount++;

            if (m_currentOption <= m_maxOptionCount && m_optionCount <= m_maxOptionCount)
            {
                DrawText(new Rect(m_leftPad, m_optionCount * 20 + (185 + 24), m_width, 20), option, 18, new Color32(213, 213, 213, 255), false);
            }
            else if (m_optionCount > (m_currentOption - m_maxOptionCount) && m_optionCount <= m_currentOption)
            {
                DrawText(new Rect(m_leftPad, (m_optionCount - (m_currentOption - m_maxOptionCount)) * 20 + (185 + 24), m_width, 20), option, 18, new Color32(213, 213, 213, 255), false);
            }

            return m_optionCount;
        }

        public static void SetTextFieldFocus(string name)
        {
            GUI.FocusControl(name);
        }

        public static void TextFieldUnfocus()
        {
            GUIUtility.keyboardControl = 0;
        }

        public static bool TextFieldEnterPressed()
        {
            return (Event.current.type == EventType.KeyDown && Event.current.character == '\n');
        }

        public static bool IsTextFieldFocused(string controlName)
        {
            return GUI.GetNameOfFocusedControl() == controlName;
        }

        public static int AddTextField(string option, ref string value, string controlName)
        {
            m_optionCount++;

            if (m_currentOption <= m_maxOptionCount && m_optionCount <= m_maxOptionCount)
            {
                Rect pos = new Rect(m_leftPad, m_optionCount * 20 + (185 + 24), m_width, 20);
                DrawText(pos, option, 18, new Color32(213, 213, 213, 255), false);
                pos.x += 80;
                pos.width -= 120;
                GUI.SetNextControlName(controlName);
                value = GUI.TextField(pos, value);
            }
            else if (m_optionCount > (m_currentOption - m_maxOptionCount) && m_optionCount <= m_currentOption)
            {
                Rect pos = new Rect(m_leftPad, (m_optionCount - (m_currentOption - m_maxOptionCount)) * 20 + (185 + 24), m_width, 20);
                DrawText(pos, option, 18, new Color32(213, 213, 213, 255), false);
                pos.x += 80;
                pos.width -= 120;
                GUI.SetNextControlName(controlName);
                value = GUI.TextField(pos, value);
            }

            return m_optionCount;
        }

        public static void AddMenuOption(string option, MenuId targetMenu)
        {
            AddMenuEntry(option);

            if (m_currentOption == m_optionCount && m_optionPress)
            {
                ChangeSubmenu(targetMenu);
            }
        }

        public static int AddInt(string option, ref int value, int min, int max, int step = 1)
        {
            int count = AddMenuEntry($"{option}: <color=#ff0000><</color> <color=#ffffff><b><i>{value}</i></b></color> <color=#ff0000>></color>");

            if (m_currentOption == m_optionCount)
            {
                if (m_rightPress)
                {
                    if (value >= max)
                    {
                        value = min;
                    }
                    else
                    {
                        value = value + step;
                    }

                    m_rightPress = false;
                }
                else if (m_leftPress)
                {
                    if (value <= min)
                    {
                        value = max;
                    }
                    else
                    {
                        value = value - step;
                    }

                    m_leftPress = false;
                }
            }

            return count;
        }

        public static int AddBool(string option, ref bool value)
        {
            int count = AddMenuEntry($"{option}: <color=#ff0000><</color> <color=#ffffff><b><i>{value}</i></b></color> <color=#ff0000>></color>");

            if (m_currentOption == m_optionCount)
            {
                if (m_rightPress || m_leftPress)
                {
                    value = !value;
                }
            }

            return count;
        }

        public static int AddFloat(string option, ref float value, float min, float max, float step = 1.0f)
        {
            int count = AddMenuEntry($"{option}: <color=#ff0000><</color> <color=#ffffff><b><i>{value}</i></b></color> <color=#ff0000>></color>");

            if (m_currentOption == m_optionCount)
            {
                if (m_rightPress)
                {
                    if (value >= max)
                    {
                        value = min;
                    }
                    else
                    {
                        value = value + step;
                    }

                    m_rightPress = false;
                }
                else if (m_leftPress)
                {
                    if (value <= min)
                    {
                        value = max;
                    }
                    else
                    {
                        value = value - step;
                    }

                    m_leftPress = false;
                }
            }

            return count;
        }

        public static int AddArray(string option, ref int value, string[] names, int size)
        {
            int count = AddMenuEntry($"{option}: <color=#ff0000><</color> <color=#ffffff><b><i>{names[value]}</i></b></color> <color=#ff0000>></color>");

            if (m_currentOption == m_optionCount)
            {
                if (m_rightPress)
                {
                    if (value >= (size - 1))
                    {
                        value = 0;
                    }
                    else
                    {
                        value = value + 1;
                    }

                    m_rightPress = false;
                }
                else if (m_leftPress)
                {
                    if (value <= 0)
                    {
                        value = size - 1;
                    }
                    else
                    {
                        value = value - 1;
                    }

                    m_leftPress = false;
                }
            }

            return count;
        }
    }
}