using HarmonyLib;
using Il2CppScheduleOne.Trash;
using MelonLoader;
using UnityEngine;
using System.Collections;

[assembly: MelonInfo(typeof(S1_TrashLifetime.Core), "TrashLifetime", "1.2.0", "Dom3005", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace S1_TrashLifetime
{
    public class Core : MelonMod
    {
        public static MelonPreferences_Category menuConfig;
        public static MelonPreferences_Entry<int> timerEntry;
        public static MelonPreferences_Entry<bool> onlyDeleteTrashbagsEntry;
        public static MelonPreferences_Entry<bool> dontDeleteTrashbagsEntry;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized!");
            InitializeConfig();
        }

        public void InitializeConfig()
        {
            menuConfig = MelonPreferences.CreateCategory("TrashLifetime", "Trash Lifetime");
            timerEntry = menuConfig.CreateEntry("TrashLifetime", 60, display_name:"Trash Lifetime", description: "Trash Lifetime (seconds), default is 60");
            onlyDeleteTrashbagsEntry = menuConfig.CreateEntry("OnlyDeleteTrashbags", false, display_name: "Only Delete Trashbags", description: "Only affect trash bags (Makes this mod redundant if combined with DontDeleteTrashpacks option). Default: False");
            dontDeleteTrashbagsEntry = menuConfig.CreateEntry("DontDeleteTrashbags", false, display_name:"Dont delete Trashbags", description: "Do not affect trash bags (Makes this mod redundant if combined with OnlyDeleteTrashpacks option). Default: False");
            menuConfig.SaveToFile();
        }
    }

    [HarmonyPatch(typeof(TrashItem), "Start")]
    public class TrashItemPatch
    {
        private static void Postfix(TrashItem __instance)
        {
            bool isTrashbag = __instance.GetScriptClassName() == "TrashBag";
            MelonLogger.Msg($"TrashItem detected: {__instance.name}, Trashbag?{isTrashbag}");
            if (Core.onlyDeleteTrashbagsEntry.Value && !isTrashbag) return;
            if (Core.dontDeleteTrashbagsEntry.Value && isTrashbag) return;

            MelonCoroutines.Start(DeleteTimer(__instance));
        }

        private static IEnumerator DeleteTimer(TrashItem item)
        {
            yield return new WaitForSeconds(Core.timerEntry.Value);
            if (item == null) yield break;
            item.DestroyTrash();
        }
    }
}