using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DillyzLegacyPack
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(5)] // low
    class HudManagerPatch
    {
        public static void Postfix(HudManager __instance) {
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
}
