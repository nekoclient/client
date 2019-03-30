using NekoClient.Logging;
using NekoClient.UI.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NekoClient.UI
{
    public static class MenuProcessor
    {
        private static Dictionary<MenuId, List<EntryCommand>> Menus = new Dictionary<MenuId, List<EntryCommand>>();

        public static void Initialize()
        {
            Drawing.SetMenuTriggers(KeyCode.M, KeyCode.Return, KeyCode.Backspace, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        }

        public static void AddCommands(List<PluginBase> bases)
        {
            foreach (PluginBase menuBase in bases)
            {
                Log.Debug($"Going through menu base {menuBase.GetType().ToString()}");
                Dictionary<MenuId, EntryCommand> entries = new Dictionary<MenuId, EntryCommand>();

                foreach (MethodInfo methodInfo in menuBase.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    try
                    {
                        object[] attributes = methodInfo.GetCustomAttributes(typeof(EntryAttribute), true);

                        if (attributes.Any())
                        {
                            if (attributes[0].GetType() == typeof(EntryAttribute))
                            {
                                EntryAttribute commandAttribute = (EntryAttribute)(attributes[0]);

                                if (commandAttribute != null)
                                {
                                    entries.Add(commandAttribute.Menu, new EntryCommand(methodInfo, commandAttribute.Menu, commandAttribute.Label));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                foreach (KeyValuePair<MenuId, EntryCommand> entry in entries)
                {
                    Log.Debug($"Adding entry {entry.Value.Label} to menu {entry.Key.ToString()}");

                    if (!Menus.ContainsKey(entry.Key))
                    {
                        Menus[entry.Key] = new List<EntryCommand>()
                        {
                            entry.Value
                        };
                    }
                    else
                    {
                        Menus[entry.Key].Add(entry.Value);
                    }

                    Log.Debug($"Added entry {entry.Value.Label} to menu {entry.Key.ToString()}");
                }
            }
        }

        public static void RunMenu()
        {
            Drawing.HandleInput();
            Drawing.m_optionCount = 0;

            if (Drawing.m_subMenu != MenuId.NotOpen)
            {
                Drawing.StyleMenu();
                Drawing.SetMenuTitle(Drawing.m_subMenu.ToString());
            }

            RunAll();

            Drawing.m_leftPress = false;
            Drawing.m_rightPress = false;
            Drawing.m_optionPress = false;
        }

        private static void RunAll()
        {
            foreach (KeyValuePair<MenuId, List<EntryCommand>> commands in Menus)
            {
                if (Drawing.m_subMenu == commands.Key)
                {
                    foreach (EntryCommand command in commands.Value)
                    {
                        int entry = Drawing.AddMenuEntry(command.Label);

                        if (Drawing.IsEntryPressed(entry))
                        {
                            command.Run();
                        }
                    }
                }
            }
        }
    }
}