using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace DillyzLegacyPack
{
    [HarmonyPatch(typeof(DillyzUtil), nameof(DillyzUtil.commitAssassination))]
    class DillyzUtilPatch
    {
        public static byte nextPheonixGhostId = 255;
        public static void Postfix(DillyzUtil __instance, PlayerControl assassinator, PlayerControl target) {
            if (target.PlayerId == nextPheonixGhostId)
            {
                CustomRole.setRoleName(nextPheonixGhostId, "Phoenix's Ghost");
                nextPheonixGhostId = 255;
            }
        }
    }
}
