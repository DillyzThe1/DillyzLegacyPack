using DillyzRoleApi_Rewritten;
using HarmonyLib;

namespace DillyzLegacyPack
{
    [HarmonyPatch(typeof(DillyzUtil), nameof(DillyzUtil.commitAssassination))]
    class DillyzUtilPatch
    {
        public static byte nextPheonixGhostId = 255;
        public static void Postfix(DillyzUtil __instance, PlayerControl assassinator, PlayerControl target, bool tp) {
            if (target.PlayerId == nextPheonixGhostId)
            {
                CustomRole.setRoleName(nextPheonixGhostId, "Phoenix's Ghost");
                nextPheonixGhostId = 255;
            }

            if (tp)
                KillAnimation.SetMovement(assassinator, !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(assassinator) != "TiMEpostor"));
            KillAnimation.SetMovement(target, !(DillyzLegacyPackMain.timeFrozen && DillyzUtil.getRoleName(target) != "TiMEpostor"));

            if (assassinator.PlayerId == PlayerControl.LocalPlayer.PlayerId && DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "Phoenix Zero" && DillyzLegacyPackMain.wrath.GameInstance != null)
                switch (DillyzLegacyPackMain.wrathDisables) {
                    case "Any Kill":
                        DillyzLegacyPackMain.wrath.GameInstance.blockingButton = true;
                        DillyzLegacyPackMain.wrath.GameInstance.showIconOnBlocked = true;
                        break;
                    case "Impostor Kill":
                        if (DillyzUtil.roleSide(target) == CustomRoleSide.Impostor)
                        {
                            DillyzLegacyPackMain.wrath.GameInstance.blockingButton = true;
                            DillyzLegacyPackMain.wrath.GameInstance.showIconOnBlocked = true;
                        }
                        break;
                    case "Crewmate Kill":
                        if (DillyzUtil.roleSide(target) == CustomRoleSide.Crewmate)
                        {
                            DillyzLegacyPackMain.wrath.GameInstance.blockingButton = true;
                            DillyzLegacyPackMain.wrath.GameInstance.showIconOnBlocked = true;
                        }
                        break;
                    case "Other Kill":
                        if (DillyzUtil.roleSide(target) == CustomRoleSide.Independent || DillyzUtil.roleSide(target) == CustomRoleSide.LoneWolf)
                        {
                            DillyzLegacyPackMain.wrath.GameInstance.blockingButton = true;
                            DillyzLegacyPackMain.wrath.GameInstance.showIconOnBlocked = true;
                        }
                        break;
                    case "Non-Crew Kill":
                        if (DillyzUtil.roleSide(target) != CustomRoleSide.Crewmate)
                        {
                            DillyzLegacyPackMain.wrath.GameInstance.blockingButton = true;
                            DillyzLegacyPackMain.wrath.GameInstance.showIconOnBlocked = true;
                        }
                        break;
                }

            if (DillyzUtil.getRoleName(assassinator) == "TiMEpostor")
                DillyzLegacyPackMain.lastKilledByTiMEpostor.Add(target.PlayerId);
        }
    }
}
