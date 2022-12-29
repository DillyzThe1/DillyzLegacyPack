using DillyzRoleApi_Rewritten;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzLegacyPack
{
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameEnd))]
        class PlayerControlPatch_OnEndGame {
            public static void Postfix(PlayerControl __instance) {
                DillyzLegacyPackMain.senseiSwordOut = false;
                DillyzLegacyPackMain.phoenixzero.nameColorPublic = false;
            }
        }
    }
}
