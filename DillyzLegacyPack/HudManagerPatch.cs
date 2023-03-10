using DillyzRoleApi_Rewritten;
using HarmonyLib;
using TMPro;

namespace DillyzLegacyPack
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.LowerThanNormal)] // low
    class HudManagerPatch
    {
        public static void Postfix(HudManager __instance) {

            if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor")
            {
                __instance.SabotageButton.gameObject.SetActive(false);
                __instance.SabotageButton.SetDisabled();
                __instance.SabotageButton.transform.parent = __instance.KillButton.transform.parent.parent;
            }
            else if (__instance.SabotageButton.transform.parent != __instance.KillButton.transform.parent)
                __instance.SabotageButton.transform.parent = __instance.KillButton.transform.parent;

            if (DillyzLegacyPackMain.timeFrozen)
            {
                __instance.ReportButton?.SetDisabled();
                __instance.PetButton?.SetDisabled();
                __instance.UseButton?.SetDisabled();
                __instance.UseButton?.SetTarget(null);
                __instance.AdminButton?.SetDisabled();

                __instance.PetButton?.gameObject.SetActive(false);
                __instance.UseButton?.gameObject.SetActive(true);

                if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) != "TiMEpostor" || DillyzLegacyPackMain.reversingTime)
                {
                    __instance.AbilityButton?.SetDisabled();
                    __instance.KillButton?.SetDisabled();
                    __instance.KillButton?.SetTarget(null);
                    __instance.ImpostorVentButton?.SetDisabled();
                    __instance.ImpostorVentButton?.SetTarget(null);
                }
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls) {
                if (!DillyzLegacyPackMain.namesPublic.Contains(player.PlayerId))
                    continue;

                string rnnnn = DillyzUtil.getRoleName(player);
                CustomRole theRole = (rnnnn != null && rnnnn != "") ? CustomRole.getByName(rnnnn) : null;

                string hex = DillyzUtil.colorToHex(theRole.roleColor);
                TextMeshPro tmp = player.gameObject.transform.Find("Names").Find("NameText_TMP").GetComponent<TextMeshPro>();
                tmp.text = $"<{hex}>{player.name}</color>";

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                        if (pva.TargetPlayerId == player.PlayerId)
                        {
                            pva.NameText.text = $"<{hex}>{player.name}</color>";
                            return;
                        }

                player.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 2f);
                player.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", DillyzUtil.color32ToColor(theRole.roleColor));
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    class HudManagerPatch_Start {

        public static void Postfix(HudManager __instance) {
            DillyzLegacyPackMain.ssreveal = DillyzUtil.getSprite(DillyzLegacyPackMain.assembly, "DillyzLegacyPack.Assets.reveal2.png");
            DillyzLegacyPackMain.sshide = DillyzUtil.getSprite(DillyzLegacyPackMain.assembly, "DillyzLegacyPack.Assets.hide.png");
        }
    }
}
