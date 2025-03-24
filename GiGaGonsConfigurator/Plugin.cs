using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using MonoMod.Cil;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace GiGaGonsConfigurator
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static readonly Harmony Patcher = new(PluginInfo.PLUGIN_GUID);
        private void Awake()
        {
            Logger = base.Logger;
            Patcher.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void OnDestroy()
        {
            // This is very brittle but I don't know of a better way
            // to detect if HideManagerGameObject = true
            // In spite of that I've tried to make it as robust as possible
            var path = Assembly.GetExecutingAssembly().Location;
            // Find the BepInEx folder
            while (true)
            {
                if (path == null) return;
                if (Path.GetDirectoryName(path) == "BepInEx")
                {
                    break;
                }
                path = Directory.GetParent(path).FullName;
            }
            // Get the path to the config folder
            path = Path.Combine(path, "config", "BepInEx.cfg");
            // If something goes wrong while trying to read the config,
            // it's fine to just give up and not unload everything
            string configContents;
            try
            {
                configContents = File.ReadAllText(path);
            }
            catch (System.Exception ex) when (
                ex is System.ArgumentException
                || ex is System.ArgumentNullException
                || ex is PathTooLongException
                || ex is DirectoryNotFoundException
                || ex is IOException
                || ex is System.UnauthorizedAccessException
                || ex is FileNotFoundException
                || ex is System.NotSupportedException
                || ex is System.Security.SecurityException
            )
            { return; }
            // This should be consistent since BepInEx normalizes config content on load
            if (configContents != null && configContents.Contains("HideManagerGameObject = false")) { return; }
            // The actual OnDestroy action
            Patcher.UnpatchSelf();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is unloaded!");
        }

    }

    //[HarmonyPatch(typeof(LevelMapGenerator), "ConfigSpecialNodes")]
    //[HarmonyDebug]
    //public static class Patch_LevelMapGeneration_ConfigSpecialNodes
    //{
    //    [HarmonyDebug]
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        System.Console.Write("Test2");
    //        var matcher = new CodeMatcher(instructions);
    //        matcher.MatchForward(false,
    //            new CodeMatch(OpCodes.Ldc_I4_5)
    //        );
    //        matcher.SetOpcodeAndAdvance(OpCodes.Ldc_I4_1);
    //        matcher.SearchForward(x => x.opcode == OpCodes.Ldc_R4 && (float)x.operand == 0.3f);
    //        matcher.SetOperandAndAdvance(1.0f);
    //        return matcher.InstructionEnumeration();
    //    }
    //}

}

//[HarmonyPatch(typeof(LevelAssembler), "GetLevel")]
//public static class Patch_LevelAssembler_GetLevel
//{
//    [HarmonyDebug]
//    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//    {
//        var matcher = new CodeMatcher(instructions);
//        matcher.MatchForward(true,
//            new CodeMatch(OpCodes.Ldloc_1),
//            new CodeMatch(OpCodes.Ldarg_3)
//        );
//        matcher.InsertAndAdvance(
//            new CodeInstruction(OpCodes.Ldc_I4_5),
//            new CodeInstruction(OpCodes.Mul)
//        );
//        matcher.MatchForward(true,
//            new CodeMatch(OpCodes.Ldloc_1),
//            new CodeMatch(OpCodes.Ldarg_3)
//        );
//        matcher.InsertAndAdvance(
//            new CodeInstruction(OpCodes.Ldc_I4_5),
//            new CodeInstruction(OpCodes.Mul)
//        );
//        return matcher.InstructionEnumeration();
//    }
//}

[HarmonyPatch(typeof(LevelAssembler), nameof(LevelAssembler.GetLevel))]
internal static class Patch_LevelAssembler_GetLevel
{
    internal static bool Prefix(int difficulty, GameObject biome, int length = 6, float slope = 0f)
    {
        return false;
    }
}