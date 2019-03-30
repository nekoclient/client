using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LocalReferences
{
    internal class Program
    {
        private class UserConfig
        {
            public string PcUsername { get; set; }
            public string GameType { get; set; }
            public string GamePath { get; set; }
        }

        private class Configuration
        {
            public List<UserConfig> UserConfigs { get; set; }
            public List<string> RequiredLibraries { get; set; }
        }

        private static Configuration Config;

        private static void ReadConfiguration()
        {
            void GenerateNew()
            {
                Console.WriteLine("ERROR READING CONFIG. GENERATING NEW.");
                Config = new Configuration()
                {
                    UserConfigs = new List<UserConfig>()
                    {
                        new UserConfig()
                        {
                            PcUsername = "REIMU",
                            GameType = "Steam",
                            GamePath = "G:\\Games\\SteamLibrary\\steamapps\\common\\VRChat"
                        },
                        new UserConfig()
                        {
                            PcUsername = "REIMU",
                            GameType = "Oculus",
                            GamePath = "G:\\Oculus\\Software\\Software\\vrchat-vrchat"
                        }
                    },
                    RequiredLibraries = new List<string>()
                    {
                        "Assembly-CSharp.dll",
                        "Photon3Unity3D.dll",
                        "UnityEngine.dll",
                        "UnityEngine.CoreModule.dll",
                        "Newtonsoft.Json.dll",
                        "VRCCore-Standalone.dll",
                        "UnityEngine.ImageConversionModule.dll",
                        "UnityEngine.IMGUIModule.dll",
                        "UnityEngine.TextRenderingModule.dll",
                        "VRCSDK2.dll",
                        "UnityEngine.Analytics.dll",
                        "UnityEngine.AnimationModule.dll",
                        "UnityEngine.AudioModule.dll",
                        "UnityEngine.ClothModule.dll",
                        "UnityEngine.ParticleSystemModule.dll",
                        "UnityEngine.PhysicsModule.dll",
                        "UnityEngine.UI.dll",
                    }
                };
            }

            if (File.Exists("Config.json"))
            {
                try
                {
                    Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("Config.json"));
                }
                catch
                {
                    GenerateNew();
                }
            }
            else
            {
                GenerateNew();
            }
        }

        private static void SaveConfiguration()
        {
            File.WriteAllText($"Config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
        }

        private static void PromptForGameDirIfNotExists()
        {
            string PromptForGameDir()
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "VRChat folder";

                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        string[] files = Directory.GetFiles(fbd.SelectedPath);

                        if (!files.Any(f => f.ToLowerInvariant() == "vrchat.exe"))
                        {
                            return PromptForGameDir();
                        }
                        else
                        {
                            return fbd.SelectedPath;
                        }
                    }

                    return "";
                }
            }

            if (!Config.UserConfigs.Any(c => c.PcUsername == Environment.UserName))
            {
                MessageBox.Show("Game path for your username was not found, a file dialog will open, please select your vrchat installation folder.");

                string gameDir = PromptForGameDir();

                UserConfig user = new UserConfig()
                {
                    PcUsername = Environment.UserName,
                    GamePath = gameDir
                };
            }
        }

        private static void CopyReferences()
        {
            IEnumerable<UserConfig> configs = Config.UserConfigs.Where(u => u.PcUsername == Environment.UserName);

            if (!Directory.Exists("References"))
            {
                Directory.CreateDirectory("References");
            }

            foreach (UserConfig config in configs)
            {
                string targetDir = Path.Combine("References", config.GameType);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                foreach (string file in Config.RequiredLibraries)
                {
                    string filePath = Path.Combine(config.GamePath, "VRChat_Data", "Managed", file);
                    string destFilePath = Path.Combine(targetDir, file);

                    if (File.Exists(filePath))
                    {
                        Console.WriteLine($"Copying file {file}");

                        if (File.Exists(destFilePath))
                        {
                            File.Delete(destFilePath);
                        }

                        File.Copy(filePath, destFilePath);
                    }
                    else
                    {
                        Console.WriteLine($"Didn't find file {file}");
                    }
                }
            }
        }

        private static void Main(string[] args)
        {
            ReadConfiguration();
            PromptForGameDirIfNotExists();
            CopyReferences();
            SaveConfiguration();
        }
    }
}