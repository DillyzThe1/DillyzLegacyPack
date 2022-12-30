using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzLegacyPack
{
    class DeadBodyPatch
    {
        [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
        class DeadBodyPatch_OnClick
        {
            public static bool Prefix(PlayerControl __instance)
            {
                return !DillyzLegacyPackMain.timeFrozen && !DillyzLegacyPackMain.reversingTime;
            }
        }
    }
}
