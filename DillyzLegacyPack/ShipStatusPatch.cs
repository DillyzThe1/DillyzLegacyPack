using HarmonyLib;

namespace DillyzLegacyPack
{
    class ShipStatusPatch
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        class ShipStatus_Begin
        {
            public static void Postfix(ShipStatus __instance)
            {
                PlayerControlPatch.resetstuffs();
            }
        }
    }
}
