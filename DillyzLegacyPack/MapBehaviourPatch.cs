using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace DillyzLegacyPack
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    class MapBehaviourPatch
    {
        public static void Postfix(MapBehaviour __instance)
        {
            if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor" || DillyzLegacyPackMain.timeFrozen)
                __instance.infectedOverlay.gameObject.SetActive(false);
        }
    }
}
