using System.Reflection;

namespace NekoClient.UI
{
    public class EntryCommand
    {
        public readonly MenuId Menu;
        public readonly string Label;

        private readonly MethodInfo m_method;

        public void Run()
        {
            m_method.Invoke(null, null);
        }

        public EntryCommand(MethodInfo methodInfo, MenuId menuId, string menuLabel)
        {
            Menu = menuId;
            Label = menuLabel;

            m_method = methodInfo;
        }
    }
}