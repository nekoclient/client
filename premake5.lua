newoption
{
	trigger = "game",
	value = "TYPE",
	description = "Specify which reference set to use",
	allowed = {
		{ "steam", "Steam references" },
		{ "oculus", "Oculus references" }
	}
}

if not _OPTIONS["game"] then
	print("no game specified, please specify game with --game argument! (steam, oculus)")
	return
end

solution "NekoClient"
configurations { "Steam", "Oculus" }
platforms { "x86" }

function make_configurations(additional_dir, special)
	local target_dir = "Build/NekoClient-%s"

	if additional_dir ~= "" then
		target_dir = "Build/NekoClient-%s/" .. additional_dir
	end

	if special == 1 then
		target_dir = "Build/"
	end

	flags { "NoCopyLocal" }

	symbols "Off"

	configuration "Steam"
		defines { "TRACE", "STEAM" }
		optimize "On"
		targetdir(string.format(target_dir, "Steam"))

	configuration "Oculus"
		defines { "TRACE", "OCULUS" }
		optimize "On"
		targetdir(string.format(target_dir, "Oculus"))
end

function make_project(project_name, project_type, extra_links, special)
	project(project_name) -- project name

	local project_dir = ""

	if project_type == "Base" then
		project_dir = project_name .. "/"
	else
		project_dir = project_type .. "/" .. project_name .. "/"
	end

	-- symbolspath(project_dir .. "/Symbols/" .. project_name .. ".Plugin.pdb")

	location(project_dir) -- output directory
	language "C#"

	if special ~= nil then
		if special == 1 then
			dotnetframework "4.5.2"
			kind "ConsoleApp" -- exe
		elseif special == 2 then
			dotnetframework "4.7.2"
			kind "WindowedApp" -- exe
		end
	else
		dotnetframework "3.5"
		kind "SharedLib" -- dll
	end

	files { project_dir .. "**.cs" }

	if project_type == "Plugins" then
		links({ "PluginBase" }) -- if its a plugin add pluginbase

		targetname(project_name .. ".Plugin") -- output file name

		make_configurations("Plugins")
	else
		targetname(project_name) -- output file name

		make_configurations("", special)
	end

	configuration {}

	local game_type = _OPTIONS["game"]

	local default_links =
	{
		"System",
		"System.Core",
		"System.Xml.Linq",
		"System.Data.DataSetExtensions",
		"System.Data",
		"System.Xml",
		"Build/References/" .. game_type .. "/Assembly-CSharp.dll",
		"Build/References/" .. game_type .. "/Assembly-CSharp-firstpass.dll",
		--"Build/References/" .. game_type .. "/Newtonsoft.Json.dll",
		"Build/References/" .. game_type .. "/Photon3Unity3D.dll",
		"Build/References/" .. game_type .. "/UnityEngine.Analytics.dll",
		"Build/References/" .. game_type .. "/UnityEngine.AnimationModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.AudioModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.ClothModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.CoreModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.IMGUIModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.ImageConversionModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.ParticleSystemModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.PhysicsModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.TextRenderingModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.UI.dll",
		"Build/References/" .. game_type .. "/UnityEngine.UnityAnalyticsModule.dll",
		"Build/References/" .. game_type .. "/UnityEngine.dll",
		"Build/References/" .. game_type .. "/VRCCore-Standalone.dll",
		"Build/References/" .. game_type .. "/VRCSDK2.dll"
	}

	links(default_links)

	nuget { "Newtonsoft.Json:12.0.1", "Lib.Harmony:1.2.0.1" }

	if extra_links ~= nil then
		links(extra_links)
	end
end

group "Framework"

--make_project("Exploits", "Framework", { "PluginBase", "Wrappers" })
make_project("Helpers", "Framework")
make_project("Logging", "Framework")
make_project("PluginBase", "Framework", { "Logging" })
make_project("UI", "Framework", { "Helpers", "Logging", "PluginBase" })
make_project("Wrappers", "Framework", { "Logging" })

group "Plugins"

make_project("AvatarAnti", "Plugins", { "Helpers", "Logging" })
make_project("ESP", "Plugins", { "Wrappers" })
--make_project("KOS", "Plugins", { "Exploits", "Helpers", "Logging", "Wrappers" })
--make_project("LoglessGen2", "Plugins", { "Exploits", "Logging", "UI", "Wrappers" })
make_project("NoClip", "Plugins", { "Wrappers", "UI" })
make_project("PluginReloadTester", "Plugins", { "UI", "Loader", "Logging", "Wrappers" })
make_project("QuickExit", "Plugins", nil)
make_project("SendInvite", "Plugins", { "Logging", "Wrappers" })
make_project("TestPlugin", "Plugins", { "UI", "Wrappers", "Logging", "Helpers" })
make_project("TrackUser", "Plugins", { "Wrappers", "UI" })
make_project("VoiceQuality", "Plugins", { "Wrappers", "UI" })
make_project("WorldTracking", "Plugins", { "Wrappers", "Logging" })
make_project("FavouriteOverriding", "Plugins", { "Wrappers", "Helpers", "Logging" })
make_project("PlayerLog", "Plugins", { "Wrappers", "Logging", "UI", "Helpers" })

if _OPTIONS["game"] == "oculus" then
	make_project("PlayspaceMover.Oculus", "Plugins", { "Logging" })
end

make_project("Patches", "Plugins", { "Logging", "AvatarAnti", "Wrappers", "WorldTracking", "FavouriteOverriding" })
make_project("DynamicBones", "Plugins", { "Wrappers", "Logging", "Helpers" })

group "Base"

--make_project("Launcher", "Tools", { "System.Windows.Forms", "System.Net", "System.Threading" }, 2)
make_project("Loader", "Base", { "Logging", "PluginBase", "Patches", "Wrappers" })
make_project("LocalReferences", "Tools", { "System.Windows.Forms" }, 1)

