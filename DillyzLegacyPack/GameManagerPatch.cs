using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace DillyzLegacyPack
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnPlayerDeath))]
    class GameManagerPatch
    {
        public static bool Prefix(GameManager __instance, PlayerControl player, bool assignGhostRole) {
            if (CustomRole.getRoleName(player.PlayerId) == "Phoenix")
            {
                DillyzUtilPatch.nextPheonixGhostId = player.PlayerId;
                return false;
            }
            return true;
        }
    }
}
