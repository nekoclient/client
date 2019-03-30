using NekoClient;
using NekoClient.Logging;
using NekoClient.Wrappers;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Loader
{
    public class Loader
    {
        private static GameObject m_mainObject;

        public static void Initialize()
        {
            if (System.IO.File.Exists("NekoClient\\DEBUG"))
            {
                Log.Initialize(LogLevel.All);
            }
            else
            {
                Log.Initialize(LogLevel.Info);
            }

            Log.AddListener(new FileLogListener("NekoClient\\NekoClient.log", false));
            Log.AddListener(new GameLogListener());
            Log.AddListener(new TraceLogListener());

            try
            {
                InitializeInternal();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private static void InitializeInternal()
        {
            PluginLoader.InitializeDependencies();
            Wrappers.Initialize();

            /*MenuProcessor.Initialize();
            MenuProcessor.AddCommands(PluginLoader.GetLoadedAssemblies());*/

            new Thread(() =>
            {
                Thread.Sleep(6000);

                PluginLoader.Initialize();

                m_mainObject = new GameObject("NekoClient");
                m_mainObject.AddComponent<PluginRunner>();

                UnityEngine.Object.DontDestroyOnLoad(m_mainObject);
            }).Start();
        }
    }

    internal class PluginRunner : MonoBehaviour
    {
        private void Start()
        {
            PluginProcessor.RunAll(plugin => plugin.Initialize());
        }

        private void Update()
        {
            void StartQueuedCoroutines()
            {
                try
                {
                    if (PluginBase.QueuedCoroutines.Count != 0)
                    {
                        foreach (IEnumerator coroutine in PluginBase.QueuedCoroutines.ToArray())
                        {
                            StartCoroutine(coroutine);
                        }

                        PluginBase.QueuedCoroutines.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception while attempting to start coroutine: {0}", ex.ToString());
                }
            }

            PluginProcessor.RunAll(plugin => plugin.RunFrame());
            StartQueuedCoroutines();
        }

        private void OnGUI()
        {
            PluginProcessor.RunAll(plugin => plugin.RunGui());

            //MenuProcessor.RunMenu();
        }

        private void OnRenderObject()
        {
            PluginProcessor.RunAll(plugin => plugin.RunRenderObject());
        }
    }
}