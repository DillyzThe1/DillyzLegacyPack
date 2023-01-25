using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System.Collections.Generic;
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
            public static void Postfix(MeetingHud __instance, GameData.PlayerInfo voterPlayer, int index, Transform parent)
            {
                if (dictatedVotes.Contains(voterPlayer.PlayerId)) {
                    for (int i = 0; i < DillyzLegacyPackMain.dictatingPower - 1; i++) {
                        SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);
                        if (GameManager.Instance.LogicOptions.GetAnonymousVotes() || !DillyzLegacyPackMain.dictator.nameColorPublic)
                            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
                        else  
                            PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                        spriteRenderer.transform.SetParent(parent);
                        spriteRenderer.transform.localScale = Vector3.zero;
                        __instance.StartCoroutine(Effects.Bloop((float)index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
                        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        class MeetingHudPatch_VotingComplete
        {
            public static void Postfix(MeetingHud __instance, MeetingHud.VoterState[] states, GameData.PlayerInfo exiled, bool tie)
            {
                DillyzLegacyPackMain.Instance.Log.LogInfo("We've got " + dictatedVotes.Count + " votes to dictate.");

                // player, vote count
                Dictionary<byte, int> allVotes = new Dictionary<byte, int>();
                byte highestVoted = 0;
                int highestVotes = 0;
                bool highestAtTie = false;

                DillyzLegacyPackMain.Instance.Log.LogInfo("-- WHO VOTED FOR WHO?! --");

                for (int i = 0; i < __instance.playerStates.Count; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];

                    if (!playerVoteArea.DidVote || (playerVoteArea.VotedFor >= 250 && playerVoteArea.VotedFor != __instance.SkipVoteButton.TargetPlayerId))
                        continue;

                    int curVotes = (dictatedVotes.Contains(playerVoteArea.TargetPlayerId) ? DillyzLegacyPackMain.dictatingPower : 1);
                    if (allVotes.ContainsKey(playerVoteArea.VotedFor))
                        curVotes += allVotes[playerVoteArea.VotedFor];
                    allVotes[playerVoteArea.VotedFor] = curVotes;

                    // skip vote
                    if (playerVoteArea.VotedFor == __instance.SkipVoteButton.TargetPlayerId) 
                        DillyzLegacyPackMain.Instance.Log.LogInfo(DillyzUtil.findPlayerControl(playerVoteArea.TargetPlayerId).name + " voted to skip.");
                    else
                        DillyzLegacyPackMain.Instance.Log.LogInfo(DillyzUtil.findPlayerControl(playerVoteArea.TargetPlayerId).name + " voted for " 
                                                                                + DillyzUtil.findPlayerControl(playerVoteArea.VotedFor).name + ".");

                    if (highestVotes > curVotes)
                        continue;

                    highestVoted = playerVoteArea.VotedFor;
                    highestAtTie = (highestVotes == curVotes);
                    highestVotes = curVotes;
                }

                DillyzLegacyPackMain.Instance.Log.LogInfo("-- WHO HAS HOW MANY VOTES?! --");
                foreach (byte pid in allVotes.Keys)
                    DillyzLegacyPackMain.Instance.Log.LogInfo((pid == __instance.SkipVoteButton.TargetPlayerId ? "Skipping" : DillyzUtil.findPlayerControl(pid).name) + " has " + allVotes[pid] + " votes rn.");

                DillyzLegacyPackMain.Instance.Log.LogInfo("-- RESULT?! --");
                if (highestAtTie)
                {
                    DillyzLegacyPackMain.Instance.Log.LogInfo("The votes are tied! No exile!");
                    __instance.exiledPlayer = null;
                    __instance.wasTie = true;
                    return;
                }

                if (highestVoted == __instance.SkipVoteButton.TargetPlayerId)
                {
                    DillyzLegacyPackMain.Instance.Log.LogInfo("The voting was skipped! No exile!");
                    __instance.exiledPlayer = null;
                    __instance.wasTie = false;
                    return;
                }

                __instance.exiledPlayer = DillyzUtil.findPlayerControl(highestVoted).Data;
                __instance.wasTie = false;

                DillyzLegacyPackMain.Instance.Log.LogInfo("A judgement has been made! Let's break into " + __instance.exiledPlayer.PlayerName + "'s house and murder them!");
            }
        }
    }
}
