using NekoClient.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NekoClient
{
    public abstract class PluginBase
    {
        protected event Action Tick;

        protected event Action Gui;

        protected event Action RenderObject;

        protected event Action Unload;

        protected event Action Load;

        public static List<IEnumerator> QueuedCoroutines = new List<IEnumerator>();

        public PluginBase()
        {
        }

        public void Initialize()
        {
            if (Load != null)
            {
                Load();
            }
        }

        public void RunGui()
        {
            if (Gui != null)
            {
                try
                {
                    Gui();
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception during Gui in plugin {0}: {1}", GetType().Name, ex.ToString());
                }
            }
        }

        public void RunFrame()
        {
            if (Tick != null)
            {
                try
                {
                    Tick();
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception during Tick in plugin {0}: {1}", GetType().Name, ex.ToString());
                }
            }
        }

        public void RunRenderObject()
        {
            if (RenderObject != null)
            {
                try
                {
                    RenderObject();
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception during RenderObject in plugin {0}: {1}", GetType().Name, ex.ToString());
                }
            }
        }

        public void RunUnload()
        {
            if (Unload != null)
            {
                try
                {
                    Unload();
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception during Unload in plugin {0}: {1}", GetType().Name, ex.ToString());
                }
            }
        }

        public static void QueueCoroutine(IEnumerator coroutine)
        {
            QueuedCoroutines.Add(coroutine);
        }
    }
}