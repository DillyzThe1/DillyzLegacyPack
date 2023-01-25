using HarmonyLib;

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
