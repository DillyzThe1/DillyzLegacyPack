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
                DillyzLegacyPackMain.senseiSwordOut = false;
                DillyzLegacyPackMain.namesPublic.Clear();
                DillyzLegacyPackMain.timeFrozen = false;
                DillyzLegacyPackMain.causedTimeEvent = false;
                DillyzLegacyPackMain.reversingTime = false;
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
            }
        }
    }
}
