using HarmonyLib;

namespace DillyzLegacyPack
{
    class ChatControllerPatch
    {
        public static ChatController Instance;

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
        class ChatControllerPatch_Awake
        {
            public static void Postfix(ChatController __instance) {
                ChatControllerPatch.Instance = __instance;
            }
        } 
    }
}
