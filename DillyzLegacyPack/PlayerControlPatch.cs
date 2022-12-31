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
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameEnd))]
        class PlayerControlPatch_OnEndGame {
            public static void Postfix(PlayerControl __instance) {
                DillyzLegacyPackMain.swordsOut.Clear();
                DillyzLegacyPackMain.namesPublic.Clear();
                DillyzLegacyPackMain.timeFrozen = false;
                DillyzLegacyPackMain.causedTimeEvent = false;
                DillyzLegacyPackMain.reversingTime = false;
                DillyzLegacyPackMain.dictationsDone = 0;
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
        class PlayerControlPatch_Revive
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (AmongUsClient.Instance.AmHost && DillyzUtil.getRoleName(__instance) == "Phoenix's Ghost")
                    DillyzUtil.RpcSetRole(__instance, "Phoenix Zero");
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
        class PlayerControlPatch_Start
        {
            public static void Postfix(PlayerControl __instance)
            {
                RecordedObject ro = __instance.gameObject.AddComponent<RecordedObject>();
                ro.rb2d = __instance.gameObject.GetComponent<Rigidbody2D>();
                ro.spr = __instance.gameObject.transform.Find("BodyForms").transform.Find("Normal").gameObject.GetComponent<SpriteRenderer>();
                ro.anim = __instance.gameObject.GetComponent<Animator>();
                ro.pc = __instance;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class PlayerControlPatch_FixedUpdate
        {
            public static void Postfix(PlayerControl __instance)
            {
                try
                {
                    // cosmetic flip
                    if (__instance == null) return;
                    if (__instance.gameObject == null) return;
                    Transform cosmetics = __instance.gameObject.transform.Find("Cosmetics");
                    if (cosmetics == null) return;
                    Transform bfs = __instance.gameObject.transform.Find("BodyForms");
                    if (bfs == null) return;
                    Transform norm = bfs.transform.Find("Normal");

                    bool flipp = norm.gameObject.GetComponent<SpriteRenderer>().flipX;

                    Transform hat = cosmetics.Find("Hat");
                    if (hat != null)
                    {
                        Transform back = hat.transform.Find("Back");
                        if (back != null)
                            back.gameObject.GetComponent<SpriteRenderer>().flipX = flipp;
                        Transform front = hat.transform.Find("Front");
                        if (front != null)
                            front.gameObject.GetComponent<SpriteRenderer>().flipX = flipp;
                    }
                    Transform visor = cosmetics.Find("Visor");
                    if (visor != null)
                        visor.gameObject.GetComponent<SpriteRenderer>().flipX = flipp;
                    Transform skin = cosmetics.Find("Skin");
                    if (skin != null)
                        skin.gameObject.GetComponent<SpriteRenderer>().flipX = flipp;
                }
                catch { }
            }
        }
    }
}
