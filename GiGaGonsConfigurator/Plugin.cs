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
        public class ModConfig
        {
            public static ConfigEntry<int> levelLengthMul;

            public static void InitConfig(ConfigFile config)
            {
                levelLengthMul = config.Bind("Configuration", "Level Length Multiplier", 1, "WARNING: The game stops working above 2\nAlso doubles level generation time");
            }
        }
        private void Awake()
        {
            Logger = base.Logger;
            Patcher.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        private void OnDestroy()
        {
            if (Chainloader.ManagerObject != null && Chainloader.ManagerObject.hideFlags != HideFlags.HideAndDontSave) return;
            Patcher.UnpatchSelf();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} was unloaded!");
        }
    }
}

[HarmonyPatch(typeof(LevelAssembler), "GetLevel")]
public static class Patch_LevelAssembler_GetLevel
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);
        CodeMatch[] opcodes_to_match = {
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Ldarg_3)
        };
        matcher.MatchForward(false, opcodes_to_match);
        matcher.Advance(opcodes_to_match.Length);
        matcher.InsertAndAdvance(
            CodeInstruction.Call(() => 1 )
        );
        matcher.MatchForward(false, opcodes_to_match);
        matcher.Advance(opcodes_to_match.Length);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldc_I4_5),
            new CodeInstruction(OpCodes.Mul)
        );
        return matcher.InstructionEnumeration();
    }
}