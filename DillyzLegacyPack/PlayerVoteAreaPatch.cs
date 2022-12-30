using DillyzRoleApi_Rewritten;
using HarmonyLib;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            public static void Postfix(PlayerVoteArea __instance) {
                UiElement DictateButton = GameObject.Instantiate(__instance.ConfirmButton);
                SpriteRenderer DictateSpr = DictateButton.gameObject.GetComponent<SpriteRenderer>();
                DictateSpr.sprite = DillyzUtil.getSprite(DillyzLegacyPackMain.assembly, "DillyzLegacyPack.Assets.dictate.png");
                DictateSpr.enabled = false;
                DictateButton.name = "DictateButton";
                DictateButton.transform.parent = __instance.Buttons.transform;
                DictateButton.transform.localPosition = Vector3.zero;
                DictateButton.transform.Find("ControllerHighlight").gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 175f/255f, 30f/255f, 1f);

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
                if (IsDictator()) {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                        return false;
                    if (__instance.AmDead)
                        return false;
                    if (!__instance.Parent)
                        return false;
                    if (!__instance.voteComplete && __instance.Parent.Select((int)__instance.TargetPlayerId))
                    {
                        __instance.Buttons.SetActive(true);
                        DictateSpr.enabled = true;
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
                                DictateButton.transform.localPosition = Vector2.Lerp(Vector2.right * startPos, Vector2.right * 0f, Effects.ExpOut(t));
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
