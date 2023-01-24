using DillyzRoleApi_Rewritten;
using HarmonyLib;
using UnityEngine;

namespace DillyzLegacyPack
{
    class ShipStatusPatch
    {
        public static FlashOverlay frozenOverlay;
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        class ShipStatus_Begin
        {
            public static void Postfix(ShipStatus __instance)
            {
                PlayerControlPatch.resetstuffs();
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
        class ShipStatusPatch_OnEnable
        {
            public static void Postfix(ShipStatus __instance)
            {
                GameObject frozenOverlayy = new GameObject();
                frozenOverlayy.transform.parent = HudManager.Instance.transform;
                frozenOverlayy.name = "ice";
                frozenOverlayy.layer = LayerMask.NameToLayer("UICollide");
                frozenOverlayy.transform.position = new Vector3(0.125f, 0.25f, -250f);
                SpriteRenderer freezeRend = frozenOverlayy.AddComponent<SpriteRenderer>();
                freezeRend.sprite = DillyzUtil.getSprite(System.Reflection.Assembly.GetExecutingAssembly(), "DillyzLegacyPack.Assets.ice.png");
                frozenOverlay = frozenOverlayy.AddComponent<FlashOverlay>();
                frozenOverlay.sprrend = freezeRend;
                ShipStatusPatch.frozenOverlay.SetStaticColor(new Color(1f, 1f, 1f, 0f));
            }
        }
    }
}
