using System;

namespace NekoClient.UI.Decorators
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class EntryArrayAttribute : Attribute
    {
        public MenuId Menu { get; set; }

        public EntryArrayAttribute(MenuId menu)
        {
            Menu = menu;
        }
    }
}