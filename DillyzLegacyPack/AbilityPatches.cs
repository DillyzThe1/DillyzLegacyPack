using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzLegacyPack
{
    class AbilityPatches
    {
        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class AbilityButtonDisability {
            public static bool Prefix(AbilityButton __instance) {
                return !DillyzLegacyPackMain.timeFrozen || DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor";
            }
        }
        [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class ReportButtonDisability
        {
            public static bool Prefix(ReportButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen;
            }
        }
        [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class UseButtonDisability
        {
            public static bool Prefix(UseButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen;
            }
        }
        [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class AdminButtonDisability
        {
            public static bool Prefix(AdminButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen;
            }
        }
        [HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class PetButtonDisability
        {
            public static bool Prefix(PetButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen;
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class KillButtonDisability
        {
            public static bool Prefix(KillButton __instance)
            {
                return !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(PlayerControl.LocalPlayer) != "TiMEpostor");
            }
        }
        [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class VentButtonDisability
        {
            public static bool Prefix(VentButton __instance)
            {
                return !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(PlayerControl.LocalPlayer) != "TiMEpostor");
            }
        }
    }
}
