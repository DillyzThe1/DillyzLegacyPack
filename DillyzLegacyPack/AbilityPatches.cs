using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace DillyzLegacyPack
{
    class AbilityPatches
    {
        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class AbilityButtonDisability {
            public static bool Prefix(AbilityButton __instance) {
                return !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(PlayerControl.LocalPlayer) != "TiMEpostor") && !DillyzLegacyPackMain.reversingTime;
            }
        }
        [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class ReportButtonDisability
        {
            public static bool Prefix(ReportButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen && !DillyzLegacyPackMain.reversingTime;
            }
        }
        [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class UseButtonDisability
        {
            public static bool Prefix(UseButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen && !DillyzLegacyPackMain.reversingTime;
            }
        }
        [HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class AdminButtonDisability
        {
            public static bool Prefix(AdminButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen && !DillyzLegacyPackMain.reversingTime;
            }
        }
        [HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class PetButtonDisability
        {
            public static bool Prefix(PetButton __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen && !DillyzLegacyPackMain.reversingTime;
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class KillButtonDisability
        {
            public static bool Prefix(KillButton __instance)
            {
                return !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(PlayerControl.LocalPlayer) != "TiMEpostor") && !DillyzLegacyPackMain.reversingTime;
            }
        }
        [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
        [HarmonyPriority(Priority.HigherThanNormal)]
        class VentButtonDisability
        {
            public static bool Prefix(VentButton __instance)
            {
                return !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(PlayerControl.LocalPlayer) != "TiMEpostor") && !DillyzLegacyPackMain.reversingTime;
            }
        }
    }
}
