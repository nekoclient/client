namespace NekoClient.UI
{
    public class SubMenu
    {
        public bool FirstOpen = false;
        private MenuId MenuId;

        public SubMenu()
        {
        }

        public void Initialize(MenuId id)
        {
            MenuId = id;
        }

        public void Update()
        {
            if (Drawing.m_subMenu != Drawing.m_previousSubMenu && Drawing.m_subMenu == MenuId)
            {
                FirstOpen = true;
                Drawing.m_previousSubMenu = Drawing.m_subMenu;
            }
            else if (FirstOpen)
            {
                FirstOpen = false;
            }
        }
    }
}