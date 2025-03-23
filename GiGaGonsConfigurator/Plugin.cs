using BepInEx;
using HarmonyLib;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GiGaGonsConfigurator
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static Harmony _patches;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            UnityEngine.Debug.LogWarning("Test");
            _patches = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
        private void OnDestroy()
        {
            _patches?.UnpatchSelf();
        }

    }

    [HarmonyPatch(typeof(LevelMapGenerator), "ConfigSpecialNodes")]
    public static class Patch_LevelMapGeneration_ConfigSpecialNodes
    {
        [HarmonyILManipulator]
        public static void ILManipulator(ILContext il, MethodBase original, ILLabel retLabel)
        {
            ILCursor c = new ILCursor(il);
            UnityEngine.Debug.LogWarning("Test1");
            c.GotoNext(
                x => x.MatchBrtrue(out _),
                x => x.MatchCall<UnityEngine.Random>("get_value"),
                x => x.MatchLdcR4(0.3f)
            );
            c.Prev.Operand = 1.0f;
        }
    }
}
