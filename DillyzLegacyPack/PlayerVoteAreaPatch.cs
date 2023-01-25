using DillyzRoleApi_Rewritten;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace DillyzLegacyPack
{
    class PlayerVoteAreaPatch
    {
        public static Dictionary<byte, UiElement> DictateButtons = new Dictionary<byte, UiElement>();

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

                DictateButtons[__instance.TargetPlayerId] = DictateButton;
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.OnDestroy))]
        class PlayerVoteAreaPatch_OnDestroy
        {
            public static void Prefix(PlayerVoteArea __instance)
            {
                if (DictateButtons == null)
                    return;
                if (DictateButtons.values != null)
                    foreach (UiElement button in DictateButtons.values)
                        if (button != null)
                            GameObject.Destroy(button.gameObject);
                DictateButtons.Clear();
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
        class PlayerVoteAreaPatch_Select
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                UiElement DictateButton = DictateButtons[__instance.TargetPlayerId];
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
                        //DillyzLegacyPackMain.Instance.Log.LogInfo("hey wait " + (DictateButton == null) + " or " + (DictateButton.gameObject == null) + " or " + (DictateSpr == null) + " but " + DictateButton.transform.parent.parent.gameObject.name);
                        DictateButton.transform.parent.parent.position += new Vector3(0.1f, 0f, 0f);
                        __instance.Buttons.SetActive(true);
                        DictateSpr.enabled = true;
                        DictateButton.transform.Find("ControllerHighlight").gameObject.GetComponent<SpriteRenderer>().enabled = true;
                        float startPos = __instance.AnimateButtonsFromLeft ? 0.2f : 1.95f;
                        DictateButton.transform.localPosition = new Vector3(startPos, 0f, 0f);
                        __instance.StartCoroutine(Effects.All(new IEnumerator[]
                        {
                            Effects.Lerp(0.25f, (Il2CppSystem.Action<float>)delegate(float t)
                            {
                                __instance.CancelButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 1.3f, Effects.ExpOut(t));
                            }),
                            Effects.Lerp(0.35f, (Il2CppSystem.Action<float>)delegate(float t)
                            {
                                __instance.ConfirmButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 0.65f, Effects.ExpOut(t));
                            }),
                            Effects.Lerp(0.45f, (Il2CppSystem.Action<float>)delegate(float t)
                            {
                                DictateButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.zero, Effects.ExpOut(t));
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
