using Cpp2IL.Core.Analysis.ResultModels;
using DillyzRoleApi_Rewritten;
using HarmonyLib;
using InnerNet;
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
        public static void resetstuffs()
        {
            DillyzLegacyPackMain.swordsOut.Clear();
            DillyzLegacyPackMain.namesPublic.Clear();
            DillyzLegacyPackMain.timeFrozen = false;
            DillyzLegacyPackMain.causedTimeEvent = false;
            DillyzLegacyPackMain.reversingTime = false;
            DillyzLegacyPackMain.dictationsDone = 0;
            DillyzLegacyPackMain.phoenixLingeringEffect.Clear();

            if (DillyzLegacyPackMain.wrath.GameInstance != null)
                DillyzLegacyPackMain.wrath.GameInstance.blockingButton = DillyzLegacyPackMain.wrath.GameInstance.showIconOnBlocked = false;
            if (DillyzLegacyPackMain.revealbutton.GameInstance != null)
                DillyzLegacyPackMain.revealbutton.GameInstance.blockingButton = DillyzLegacyPackMain.revealbutton.GameInstance.showIconOnBlocked = false;
            if (DillyzLegacyPackMain.communicate.GameInstance != null)
                DillyzLegacyPackMain.communicate.GameInstance.blockingButton = DillyzLegacyPackMain.communicate.GameInstance.showIconOnBlocked = false;

            if (ShipStatusPatch.frozenOverlay != null && ShipStatusPatch.frozenOverlay.gameObject != null && ShipStatusPatch.frozenOverlay.sprrend != null)
                ShipStatusPatch.frozenOverlay.SetStaticColor(new Color(1f, 1f, 1f, 0f));
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameEnd))]
        class PlayerControlPatch_OnEndGame
        {
            public static void Postfix(PlayerControl __instance)
            {
                resetstuffs();
            }
        }
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameStart))]
        class PlayerControlPatch_OnGameStart
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                    return;
                resetstuffs();

                if (!DillyzUtil.InGame())
                    return;
                foreach (PlayerControl pc in PlayerControl.AllPlayerControls)
                {
                    RecordedObject ro = pc.gameObject.AddComponent<RecordedObject>();
                    ro.rb2d = pc.gameObject.GetComponent<Rigidbody2D>();
                    ro.spr = pc.gameObject.transform.Find("BodyForms").transform.Find("Normal").gameObject.GetComponent<SpriteRenderer>();
                    ro.anim = pc.gameObject.GetComponent<Animator>();
                    ro.pc = pc;

                    GameObject katanaobject = new GameObject();
                    KatanaObject katana = katanaobject.AddComponent<KatanaObject>();
                    katana.Setup(pc);
                }
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
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
        class PlayerControlPatch_RpcSendChat
        {
            public static bool Prefix(PlayerControl __instance, string chatText)
            {
                if (DillyzLegacyPackMain.phoenixLingeringEffect.Contains(__instance.PlayerId))
                {
                    if (MeetingHud.Instance != null && ChatControllerPatch.Instance != null)
                        ChatControllerPatch.Instance.AddChatWarning("You've been revived by the Phoenix.\nChatting is disabled.");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
        class PlayerControlPatch_Start
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (!DillyzUtil.InFreeplay())
                    return;

                RecordedObject ro = __instance.gameObject.AddComponent<RecordedObject>();
                ro.rb2d = __instance.gameObject.GetComponent<Rigidbody2D>();
                ro.spr = __instance.gameObject.transform.Find("BodyForms").transform.Find("Normal").gameObject.GetComponent<SpriteRenderer>();
                ro.anim = __instance.gameObject.GetComponent<Animator>();
                ro.pc = __instance;

                GameObject katanaobject = new GameObject();
                KatanaObject katana = katanaobject.AddComponent<KatanaObject>();
                katana.Setup(__instance);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
        class PlayerControlPatch_OnDestroy
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
                    return;


                KatanaObject katana = __instance.gameObject.GetComponentInChildren<KatanaObject>();
                if (katana != null)
                    GameObject.Destroy(katana.gameObject);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        class PlayerControlPatch_FixedUpdate
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (!DillyzLegacyPackMain.reversingTime)
                    return;
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
