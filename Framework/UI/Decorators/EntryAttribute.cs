using System;

namespace NekoClient.UI.Decorators
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EntryAttribute : Attribute
    {
        public string Label { get; set; }
        public MenuId Menu { get; set; }

        public EntryAttribute(MenuId menu, string label)
        {
            Menu = menu;
            Label = label;
        }
    }
}