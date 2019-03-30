using NekoClient;
using NekoClient.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Loader
{
    public class PluginLoader
    {
        internal static Dictionary<string, PluginBase> LoadedPlugins = new Dictionary<string, PluginBase>();

        public static void Initialize()
        {
            LoadPlugins();
        }

        public static void InitializeDependencies()
        {
            LoadDependencies("NekoClient");
        }

        public static void LoadPlugins()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            LoadAssemblyInternal("Loader", Assembly.GetExecutingAssembly());
            LoadAssemblies("NekoClient\\Plugins", "*.Plugin.dll");
        }

        public static void LoadDependencies(string dir)
        {
            string[] files = Directory.GetFiles(dir);

            foreach (string file in files)
            {
                if (file == $"{dir}\\{Assembly.GetExecutingAssembly().GetName().Name}.dll" || !file.EndsWith(".dll"))
                {
                    continue;
                }

                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    Log.Write(LogLevel.Info, "Loading dependency {0} v{1}", assembly.GetName().Name, FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion);
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Error while loading {0}: {1}", file, ex.ToString());
                }
            }
        }

        public static void LoadAssemblies(string dir, string filter)
        {
            string[] files = Directory.GetFiles(dir, filter);

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.Load(File.ReadAllBytes(file));
                    LoadAssemblyInternal(file, assembly);
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Error while loading {0}: {1}", file, ex.ToString());
                }
            }
        }

        public static void LoadAssembly(string file)
        {
            Assembly assembly = Assembly.Load(File.ReadAllBytes(file));
            LoadAssemblyInternal(file, assembly);
        }

        public static void UnloadAssembly(string name)
        {
            if (LoadedPlugins.ContainsKey(name))
            {
                LoadedPlugins[name].RunUnload();

                string[] nameSplit = name.Split('\\');

                Log.Write(LogLevel.Info, "Unloading plugin {0}", nameSplit[nameSplit.Length - 1]);

                LoadedPlugins.Remove(name);
            }
        }

        public static List<string> GetLoadedAssemblyNames()
        {
            return LoadedPlugins.Keys.ToList();
        }

        public static List<PluginBase> GetLoadedAssemblies()
        {
            return LoadedPlugins.Values.ToList();
        }

        public static List<string> GetAvailableAssemblies()
        {
            return Directory.GetFiles("NekoClient\\Plugins", "*.Plugin.dll").ToList();
        }

        private static void LoadAssemblyInternal(string file, Assembly assembly)
        {
            try
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.IsPublic && !type.IsAbstract)
                    {
                        try
                        {
                            if (type.IsSubclassOf(typeof(PluginBase)))
                            {
                                Log.Write(LogLevel.Info, "Loading plugin {0} v{1}", type.Name, assembly.GetName().Version);

                                PluginBase plugin = (PluginBase)Activator.CreateInstance(type);

                                //Log.Info($"{file} loaded?");

                                LoadedPlugins.Add(file, plugin);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write(LogLevel.Error, "An error occurred during initialization of plugin {0}: {1}", type.Name, ex.ToString());
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Log.Write(LogLevel.Warning, "Assembly {0} could not be loaded because of a loader exception: {1}", assembly.GetName(), ex.LoaderExceptions[0].ToString());
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("NekoClientSHManager"))
            {
                return Assembly.GetExecutingAssembly();
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == args.Name)
                {
                    return assembly;
                }
            }

            return null;
        }
    }
}