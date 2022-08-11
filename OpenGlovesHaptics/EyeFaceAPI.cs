using HarmonyLib;
using NeosModLoader;
using FrooxEngine;

namespace OpenGlovesHaptics
{
    public class OpenGlovesHaptics : NeosMod
	{
		public override string Name => "OpenGlovesHaptics";
		public override string Author => "dfgHiatus, eia485";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/dfgHiatus/NeosOpenGlovesHaptics/";
		public override void OnEngineInit()
		{
			new Harmony("net.dfgHiatus.OpenGlovesHaptics").PatchAll();
        }
	}
}
