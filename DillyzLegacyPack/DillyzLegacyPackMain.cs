using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.IL2CPP;
using DillyzRoleApi_Rewritten;
using HarmonyLib;
using Hazel;
using UnityEngine;

namespace DillyzLegacyPack
{
    [BepInPlugin(DillyzLegacyPackMain.MOD_ID, DillyzLegacyPackMain.MOD_NAME, DillyzLegacyPackMain.MOD_VERSION)]
    [BepInDependency("com.github.dillyzthe1.dillyzroleapi")]
    class DillyzLegacyPackMain : BasePlugin
    {
        public const string MOD_NAME = "DillyzLegacyPack", MOD_ID = "com.github.dillyzthe1.dillyzroleapi.packages.legacy", MOD_VERSION = "0.1.0-dev";
        public static Harmony harmony = new Harmony(DillyzLegacyPackMain.MOD_ID);

        public static DillyzLegacyPackMain Instance;
        public static Assembly assembly;

        public static CustomRole phoenixzero;
        public static bool senseiSwordOut = false;

        public override void Load()
        {
            Instance = this;

            Log.LogInfo(DillyzLegacyPackMain.MOD_NAME + " v" + DillyzLegacyPackMain.MOD_VERSION + " loaded. Hooray!");
            harmony.PatchAll();

            assembly = Assembly.GetExecutingAssembly();

            string[] empty = new string[] { };

            #region phoenix
            // normal Phoenix
            CustomRole phoenix = DillyzUtil.createRole("Phoenix", "Reveal your hidden power.", true, false, new Color32(225, 65, 25, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            phoenix.a_or_an = "a";
            phoenix.SetSprite(assembly, "DillyzLegacyPack.Assets.dillyzthe1.png");
            phoenix.roletoGhostInto = "Phoenix's Ghost";

            CustomButton advice = DillyzUtil.addButton(assembly, "Take Advice", "DillyzLegacyPack.Assets.dillyzthe1.png", 0f, false, new string[] { "Phoenix" }, empty, 
                delegate(KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;
                    
                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                }
            );
            advice.textOutlineColor = phoenix.roleColor;


            // ghost Phoenix
            CustomRole phoenixghost = DillyzUtil.createRole("Phoenix's Ghost", "Come back to play.", true, false, new Color32(245, 100, 85, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            phoenixghost.a_or_an = "a";
            phoenixghost.roleSeleciton = false;
            phoenixghost.hasSettings = false;
            phoenixghost.ghostRole = true;

            CustomButton secondchance = DillyzUtil.addButton(assembly, "2nd Chance", "DillyzLegacyPack.Assets.dillyzthe1.png", 20f, false, new string[] { "Phoenix's Ghost" }, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    SecondChance(PlayerControl.LocalPlayer, true);
                    DillyzUtil.RpcSetRole(PlayerControl.LocalPlayer, "Phoenix Zero");
                }
            );
            secondchance.textOutlineColor = phoenixghost.roleColor;

            // afterlife Phoenix
            phoenixzero = DillyzUtil.createRole("Phoenix Zero", "Use the power of the afterlife.", true, false, new Color32(240, 85, 40, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            phoenixzero.a_or_an = "a";
            phoenixzero.roleSeleciton = false;
            phoenixzero.hasSettings = false;

            string[] access_phoenixzero = new string[] { phoenixzero.name };

            CustomButton wrath = DillyzUtil.addButton(assembly, "Phoenix Wrath", "DillyzLegacyPack.Assets.dillyzthe1.png", 15f, true, access_phoenixzero, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.killButton.currentTarget);
                }
            );
            wrath.buttonText = "Wrath";
            wrath.textOutlineColor = phoenix.roleColor;

            CustomButton communicate = DillyzUtil.addButton(assembly, "Phoenix Communicate", "DillyzLegacyPack.Assets.dillyzthe1.png", 35f, true, access_phoenixzero, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    SecondChance(button.killButton.currentTarget, true);
                }
            );
            communicate.buttonText = "Communicate";
            communicate.textOutlineColor = phoenix.roleColor;
            communicate.buttonTargetsGhosts = true;

            CustomButton reveal = DillyzUtil.addButton(assembly, "Phoenix Reveal", "DillyzLegacyPack.Assets.dillyzthe1.png", 60f, false, access_phoenixzero, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    phoenixzero.nameColorPublic = true;
                   // PlayerControl.LocalPlayer.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 2f);
                    //PlayerControl.LocalPlayer.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", DillyzUtil.color32ToColor(phoenixzero.roleColor));
                    DillyzUtil.InvokeRPCCall("RevealPhoenixZero", delegate (MessageWriter writer) {
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(true);
                    });
                }
            );
            reveal.buttonText = "Reveal";
            reveal.textOutlineColor = phoenix.roleColor;
            reveal.SetUseTimeButton(7.5f, delegate(KillButtonCustomData button, bool interrupted) {
                phoenixzero.nameColorPublic = false;
               // PlayerControl.LocalPlayer.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 2f);
               // PlayerControl.LocalPlayer.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", Color.clear);
                DillyzUtil.InvokeRPCCall("RevealPhoenixZero", delegate (MessageWriter writer) {
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                });
            });
            #endregion

            #region sensei
            CustomRole sensei = DillyzUtil.createRole("Sensei", "Slice through suspects to victory.", true, false, new Color32(125, 45, 200, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            sensei.a_or_an = "a";
            sensei.SetSprite(assembly, "DillyzLegacyPack.Assets.dillyzthe1.png");

            CustomButton sword = null;
            sword = DillyzUtil.addButton(assembly, "Sensei Sword", "DillyzLegacyPack.Assets.dillyzthe1.png", 2.5f, false, new string[] { "Sensei" }, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    senseiSwordOut = !senseiSwordOut;
                    sword.buttonText = senseiSwordOut ? "Stash" : "Reveal";
                }
            );
            sword.buttonText = "Reveal";
            sword.textOutlineColor = phoenix.roleColor;
            #endregion

            #region time freeze button
            // todo
            #endregion

            #region rpc
            DillyzUtil.AddRpcCall("SecondChance", delegate(MessageReader reader) {
                SecondChance(DillyzUtil.findPlayerControl(reader.ReadByte()), false);
            });
            DillyzUtil.AddRpcCall("RevealPhoenixZero", delegate (MessageReader reader) {
                PlayerControl targetPhoenix = DillyzUtil.findPlayerControl(reader.ReadByte());

                phoenixzero.nameColorPublic = reader.ReadBoolean();
              //  targetPhoenix.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", phoenixzero.nameColorPublic ? 2f : 0f);
             //  targetPhoenix.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", DillyzUtil.color32ToColor(phoenixzero.roleColor));
            });
            #endregion
        }

        public static void SecondChance(PlayerControl player, bool rpc) {
            player.Revive();
            byte randomness = 0;

            switch (ShipStatus.Instance.name.ToLower().Replace("(clone)", ""))
            {
                case "skeldship":
                    player.transform.position = new Vector3(-7.25f, -4.85f, 0f);
                    break;
                case "miraship":
                    player.transform.position = new Vector3(16.25f, 0.5f, 0f);
                    break;
                case "polusship":
                    player.transform.position = new Vector3(40.375f, -6.75f, 0f);
                    break;
                case "airship":
                    randomness = (byte)UnityEngine.Random.Range(0, 3);
                    switch (randomness)
                    {
                        case (byte)0:
                            player.transform.position = new Vector3(29.25f, 7.25f, 0f);
                            break;
                        case (byte)1:
                            player.transform.position = new Vector3(30.8f, 7.25f, 0f);
                            break;
                        case (byte)2:
                            player.transform.position = new Vector3(32.35f, 7.25f, 0f);
                            break;
                        case (byte)3:
                            player.transform.position = new Vector3(33.75f, 7.25f, 0f);
                            break;
                    }
                    break;
            }

            if (!rpc)
                return;
            DillyzUtil.RpcSetRole(player, "");
            DillyzUtil.InvokeRPCCall("SecondChance", delegate (MessageWriter writer) {
                writer.Write(player.PlayerId);
                writer.Write(randomness);
            });
        }
    }
}
