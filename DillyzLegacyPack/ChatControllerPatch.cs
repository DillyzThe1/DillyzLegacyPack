using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
