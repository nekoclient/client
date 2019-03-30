using NekoClient;
using NekoClient.Logging;
using System;
using System.Linq;

namespace Loader
{
    internal static class PluginProcessor
    {
        public static void RunAll(Action<PluginBase> cb)
        {
            PluginBase[] plugins = PluginLoader.LoadedPlugins.Values.ToArray();

            foreach (PluginBase plugin in plugins)
            {
                try
                {
                    cb(plugin);
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception during RunAll call: {0}", ex.ToString());
                }
            }
        }
    }
}