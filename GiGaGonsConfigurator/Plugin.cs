using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using MonoMod.Cil;
using System;
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
            if (Chainloader.ManagerObject.hideFlags != HideFlags.HideAndDontSave) return;
            Patcher.UnpatchSelf();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} was unloaded!");
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