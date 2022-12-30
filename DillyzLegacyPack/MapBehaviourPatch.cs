using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzLegacyPack
{
    //[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Show))]
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    class MapBehaviourPatch
    {
        /*public static bool Prefix(MapBehaviour __instance, MapOptions opts) {
            if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor")
                opts.Mode = MapOptions.Modes.Normal;
            return true;
        }*/
        public static void Postfix(MapBehaviour __instance)
        {
            if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor" || DillyzLegacyPackMain.timeFrozen)
                __instance.infectedOverlay.gameObject.SetActive(false);
        }
    }
}
