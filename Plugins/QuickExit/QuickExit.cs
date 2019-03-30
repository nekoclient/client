using NekoClient;
using System.Diagnostics;
using UnityEngine;

namespace QuickExit
{
    public class QuickExit : PluginBase
    {
        public QuickExit()
        {
            Tick += QuickExit_Tick;
        }

        private void QuickExit_Tick()
        {
            if (Input.GetKey(KeyCode.Backspace) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}