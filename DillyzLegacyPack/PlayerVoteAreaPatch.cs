using DillyzRoleApi_Rewritten;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DillyzLegacyPack
{
    class PlayerVoteAreaPatch
    {
        public static bool IsDictator() {
            return DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == DillyzLegacyPackMain.dictator.name;
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Start))]
        class PlayerVoteAreaPatch_Start {
            public static void Postfix(PlayerVoteArea __instance)
            {
                if (__instance.TargetPlayerId == 253)
                    DillyzLegacyPackMain.Instance.Log.LogInfo("Skipping gets a button.");
                else
                    DillyzLegacyPackMain.Instance.Log.LogInfo(DillyzUtil.findPlayerControl(__instance.TargetPlayerId).name + " gets a button.");
                UiElement DictateButton = GameObject.Instantiate(__instance.ConfirmButton);
                SpriteRenderer DictateSpr = DictateButton.gameObject.GetComponent<SpriteRenderer>();
                DictateSpr.sprite = DillyzUtil.getSprite(DillyzLegacyPackMain.assembly, "DillyzLegacyPack.Assets.dictate.png");
                DictateSpr.enabled = true;
                DictateButton.name = "DictateButton";
                DictateButton.transform.parent = __instance.Buttons.transform;
                DictateButton.transform.localPosition = new Vector3(0f, 0f, 0f);
                DictateButton.transform.Find("ControllerHighlight").gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 175f/255f, 30f/255f, 1f);
                DictateButton.transform.Find("ControllerHighlight").gameObject.GetComponent<SpriteRenderer>().enabled = false;

                PassiveButton pb = DictateButton.GetComponent<PassiveButton>();
                pb.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                pb.OnClick.AddListener((UnityAction)dictate);
                void dictate() {
                    if (!IsDictator() || DillyzLegacyPackMain.dictationsDone >= DillyzLegacyPackMain.maxDictations)
                        return;
                    SoundManager.Instance.PlaySound(MeetingHud.Instance.VoteSound, false, 1);

                    DillyzLegacyPackMain.Instance.Log.LogInfo("dictateded");
                    DillyzLegacyPackMain.dictationsDone++;
                    MeetingHudPatch.dictatedVotes.Add(PlayerControl.LocalPlayer.PlayerId);
                    DillyzUtil.InvokeRPCCall("dictate_vote", delegate(MessageWriter writer) {
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    });

                    __instance.ConfirmButton.gameObject.GetComponent<PassiveButton>().OnClick.Invoke();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
        class PlayerVoteAreaPatch_Select
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                UiElement DictateButton = null;

                foreach (UiElement button in __instance.Buttons.GetComponentsInChildren<UiElement>())
                    if (button.gameObject.name == "DictateButton")
                    {
                        DictateButton = button;
                        break;
                    }

                if (DictateButton == null)
                    return true;

                SpriteRenderer DictateSpr = DictateButton.gameObject.GetComponent<SpriteRenderer>();

                DictateSpr.enabled = false;
                DictateButton.transform.Find("ControllerHighlight").gameObject.GetComponent<SpriteRenderer>().enabled = false;
                if (IsDictator() && DillyzLegacyPackMain.maxDictations > DillyzLegacyPackMain.dictationsDone) {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                        return false;
                    if (__instance.AmDead)
                        return false;
                    if (!__instance.Parent)
                        return false;

                    if (__instance.TargetPlayerId == 253)
                        DillyzLegacyPackMain.Instance.Log.LogInfo("Skipping highlighted.");
                    else
                        DillyzLegacyPackMain.Instance.Log.LogInfo(DillyzUtil.findPlayerControl(__instance.TargetPlayerId).name + " highlighted.");

                    if (!__instance.voteComplete && __instance.Parent.Select((int)__instance.TargetPlayerId) && !MeetingHud.Instance.DidVote(PlayerControl.LocalPlayer.PlayerId))
                    {
                        __instance.Buttons.SetActive(true);
                        DictateSpr.enabled = true;
                        DictateButton.transform.Find("ControllerHighlight").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        float startPos = __instance.AnimateButtonsFromLeft ? 0.25f : 1.95f;
                        float endOffset = __instance.AnimateButtonsFromLeft ? 0.4f : 0f;
                        DictateButton.transform.localPosition = new Vector3(startPos, 0f, 0f);
                        __instance.StartCoroutine(Effects.All(new IEnumerator[]
                        {
                            Effects.Lerp(0.25f, (Il2CppSystem.Action<float>)delegate(float t)
                            {
                                __instance.CancelButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, (Vector2.right * 1.3f) + new Vector2(endOffset, 0f), Effects.ExpOut(t));
                            }),
                            Effects.Lerp(0.35f, (Il2CppSystem.Action<float>)delegate(float t)
                            {
                                __instance.ConfirmButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, (Vector2.right * 0.65f) + new Vector2(endOffset, 0f), Effects.ExpOut(t));
                            }),
                            Effects.Lerp(0.45f, (Il2CppSystem.Action<float>)delegate(float t)
                            {
                                DictateButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, new Vector2(endOffset, 0f), Effects.ExpOut(t));
                            })
                        }));
                        List<UiElement> selectableElements = new List<UiElement>();
                        selectableElements.Add(__instance.CancelButton);
                        selectableElements.Add(__instance.ConfirmButton);
                        selectableElements.Add(DictateButton);

                        ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.CancelButton, __instance.ConfirmButton, selectableElements, false);
                    }

                    return false;
                }
                return true;
            }
        }
    }
}
