using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TheLastStand.Controller.Panic;
using TheLastStand.Model;
using TheLastStand.Model.Unit;
using TheLastStand.Manager;

namespace MultipleGain
{
    [BepInPlugin("cn.shabywu.the_last_stand.multiple_gain", "多倍收益", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        static ConfigEntry<float> ExperiencePercentage;
        static ConfigEntry<float> DamnedSoulsPercentage;
        static ConfigEntry<float> PanicRewardGoldPercentage;
        static ConfigEntry<float> PanicRewardMaterialsPercentage;

        public delegate void Log(object data);
        public static Log LogDebug;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            LogDebug = Logger.LogDebug;

            ExperiencePercentage = Config.Bind("TheLastStand.MultipleGain",  "ExperiencePercentage", 1.5f, "经验倍率");
            DamnedSoulsPercentage = Config.Bind("TheLastStand.MultipleGain",  "DamnedSoulsPercentage", 10f, "污秽精华倍率");
            PanicRewardGoldPercentage = Config.Bind("TheLastStand.MultipleGain",  "PanicRewardGoldPercentage", 1.0f, "金币倍率");
            PanicRewardMaterialsPercentage = Config.Bind("TheLastStand.MultipleGain",  "PanicRewardMaterialsPercentage", 1.0f, "材料倍率");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(KillReportData), "TotalBaseExperience", MethodType.Getter)]
        [HarmonyPostfix]
        static void patchKillReportDataTotalBaseExperience(ref float __result)
        {
            __result *= ExperiencePercentage.Value;
            LogDebug($"patch TotalBaseExperience: {__result}");
        }

        [HarmonyPatch(typeof(TrophyManager), "AddEnemyKill")]
        [HarmonyPrefix]
        static bool patchTrophyManagerAddEnemyKill(ref int damnedSoulsEarned)
        {
            damnedSoulsEarned = Convert.ToInt32(damnedSoulsEarned * DamnedSoulsPercentage.Value);
            LogDebug($"patch damnedSoulsEarned when kill enmery: {damnedSoulsEarned}");
            return true;
        }

        [HarmonyPatch(typeof(PanicRewardController), "GetReward")]
        [HarmonyPostfix]
        static void patchGetReward(ref PanicRewardController __instance)
        {
            __instance.PanicReward.Gold = Convert.ToInt32(__instance.PanicReward.Gold * PanicRewardGoldPercentage.Value);
            __instance.PanicReward.Materials = Convert.ToInt32(__instance.PanicReward.Materials * PanicRewardMaterialsPercentage.Value);
        }
    }

}
