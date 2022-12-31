using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzLegacyPack
{
    class MeetingHudPatch
    {
        public static List<byte> dictatedVotes = new List<byte>();

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        class MeetingHudPatch_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                dictatedVotes.Clear();
                if (DillyzUtil.InFreeplay())
                    __instance.discussionTimer = 0f;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
        class MeetingHudPatch_BloopAVoteIcon
        {
            public static void Postfix(MeetingHud __instance, GameData.PlayerInfo voterPlayer, int index, Transform parent) {
                if (dictatedVotes.Contains(voterPlayer.PlayerId)) {
                    for (int i = 0; i < DillyzLegacyPackMain.dictatingPower - 1; i++) {
                        SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);
                        //if (GameManager.Instance.LogicOptions.GetAnonymousVotes())
                        PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
                        //  else  PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                        spriteRenderer.transform.SetParent(parent);
                        spriteRenderer.transform.localScale = Vector3.zero;
                        __instance.StartCoroutine(Effects.Bloop((float)index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
                        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CalculateVotes))]
        class MeetingHudPatch_CalculateVotes
        {
            public static void Postfix(MeetingHud __instance, ref Il2CppSystem.Collections.Generic.Dictionary<byte, int> __result) {
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (dictatedVotes.Contains(playerVoteArea.TargetPlayerId) && playerVoteArea.VotedFor != 252 && playerVoteArea.VotedFor != 255 && playerVoteArea.VotedFor != 254)
                    {
                        int num;
                        if (__result.TryGetValue(playerVoteArea.VotedFor, out num))
                            __result[playerVoteArea.VotedFor] = num + DillyzLegacyPackMain.dictatingPower - 1;
                        else
                            __result[playerVoteArea.VotedFor] = DillyzLegacyPackMain.dictatingPower;
                    }
                }
            }
        }
    }
}
