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
using Iced.Intel;
using Il2CppSystem.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        public static CustomButton revealbutton;
        public static CustomButton wrath;
        public static bool senseiSwordOut = false;
        public static bool timeFrozen = false;
        public static bool reversingTime = false;
        public static CustomButton freezetime;
        public static CustomButton reversetime;
        public static bool causedTimeEvent = false;
        public static DateTime timeReversedOn = DateTime.MinValue;

        public static Sprite ssreveal;
        public static Sprite sshide;

        #region settings
        public static string chanceMode = "Death";
        public static string wrathDisables = "Crewmate Kill";
        public static string communicateDisables = "Revive Any";

        public static float timeReversed = 10f;
        #endregion


        public static List<byte> namesPublic = new List<byte>();

        public override void Load()
        {
            Instance = this;

            Log.LogInfo(DillyzLegacyPackMain.MOD_NAME + " v" + DillyzLegacyPackMain.MOD_VERSION + " loaded. Hooray!");
            harmony.PatchAll();

            assembly = Assembly.GetExecutingAssembly();

            string[] empty = new string[] { };
            string[] phoenixnormal = new string[] { "Phoenix" };

            #region phoenix
            // normal Phoenix
            CustomRole phoenix = DillyzUtil.createRole("Phoenix", "Reveal your hidden power.", true, false, new Color32(225, 65, 25, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            phoenix.a_or_an = "a";
            phoenix.SetSprite(assembly, "DillyzLegacyPack.Assets.dillyzthe1.png");
            phoenix.roletoGhostInto = "Phoenix's Ghost";

            CustomButton advice = DillyzUtil.addButton(assembly, "Take Advice", "DillyzLegacyPack.Assets.dillyzthe1.png", 60f, false, empty, empty,
                delegate (KillButtonCustomData button, bool success)
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

            CustomButton secondchance = DillyzUtil.addButton(assembly, "2nd Chance", "DillyzLegacyPack.Assets.second_chance.png", 20f, false, new string[] { "Phoenix's Ghost" }, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    SecondChance(PlayerControl.LocalPlayer, true);
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

            wrath = DillyzUtil.addButton(assembly, "Phoenix Wrath", "DillyzLegacyPack.Assets.wrath.png", 15f, true, access_phoenixzero, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.killButton.currentTarget);
                }
            );
            wrath.buttonText = "Wrath";
            wrath.textOutlineColor = phoenix.roleColor;

            CustomButton communicate = null;
            communicate = DillyzUtil.addButton(assembly, "Phoenix Communicate", "DillyzLegacyPack.Assets.communicate.png", 35f, true, access_phoenixzero, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    SecondChance(button.killButton.currentTarget, true);

                    CustomRoleSide rs = DillyzUtil.roleSide(button.killButton.currentTarget);
                    if (communicate.GameInstance != null)
                        switch (communicateDisables)
                        {
                            case "Revive Any":
                                communicate.GameInstance.blockingButton = true;
                                communicate.GameInstance.showIconOnBlocked = true;
                                break;
                            case "Revive Impostor":
                                if (rs == CustomRoleSide.Impostor)
                                {
                                    communicate.GameInstance.blockingButton = true;
                                    communicate.GameInstance.showIconOnBlocked = true;
                                }
                                break;
                            case "Revive Crewmate":
                                if (rs == CustomRoleSide.Crewmate)
                                {
                                    communicate.GameInstance.blockingButton = true;
                                    communicate.GameInstance.showIconOnBlocked = true;
                                }
                                break;
                            case "Revive Other":
                                if (rs == CustomRoleSide.Independent || rs == CustomRoleSide.LoneWolf)
                                {
                                    communicate.GameInstance.blockingButton = true;
                                    communicate.GameInstance.showIconOnBlocked = true;
                                }
                                break;
                        }
                }
            );
            communicate.buttonText = "Communicate";
            communicate.textOutlineColor = phoenix.roleColor;
            communicate.buttonTargetsGhosts = true;

            revealbutton = DillyzUtil.addButton(assembly, "Phoenix Reveal", "DillyzLegacyPack.Assets.reveal.png", 60f, false, access_phoenixzero, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    namesPublic.Add(PlayerControl.LocalPlayer.PlayerId);
                    DillyzUtil.InvokeRPCCall("RevealPhoenixZero", delegate (MessageWriter writer)
                    {
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(true);
                    });

                    if (revealbutton.GameInstance == null)
                        return;

                    revealbutton.GameInstance.blockingButton = true;
                    revealbutton.GameInstance.showIconOnBlocked = true;
                }
            );
            revealbutton.buttonText = "Reveal";
            revealbutton.textOutlineColor = phoenix.roleColor;
            revealbutton.SetUseTimeButton(10f, delegate (KillButtonCustomData button, bool interrupted)
            {
                namesPublic.Remove(PlayerControl.LocalPlayer.PlayerId);
                PlayerControl.LocalPlayer.ToggleHighlight(false, RoleTeamTypes.Crewmate);
                DillyzUtil.InvokeRPCCall("RevealPhoenixZero", delegate (MessageWriter writer)
                {
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(false);
                });
            });
            #endregion

            #region sensei
            CustomRole sensei = DillyzUtil.createRole("Sensei", "Slice through suspects to victory.", true, false, new Color32(125, 45, 200, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            sensei.a_or_an = "a";
            sensei.SetSprite(assembly, "DillyzLegacyPack.Assets.sensei.png");


            ssreveal = DillyzUtil.getSprite(assembly, "DillyzLegacyPack.Assets.reveal2.png");
            sshide = DillyzUtil.getSprite(assembly, "DillyzLegacyPack.Assets.hide.png");
            CustomButton sword = null;
            sword = DillyzUtil.addButton(assembly, "Sensei Sword", "DillyzLegacyPack.Assets.reveal2.png", 2.5f, false, new string[] { "Sensei" }, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    senseiSwordOut = !senseiSwordOut;
                    sword.buttonText = senseiSwordOut ? "Stash" : "Reveal";

                    if (sword.GameInstance != null)
                        sword.GameInstance.killButton.graphic.sprite = senseiSwordOut ? sshide : ssreveal;
                }
            );
            sword.buttonText = "Reveal";
            sword.textOutlineColor = sensei.roleColor;
            #endregion

            #region timepostor
            // alt color : new Color32(100, 150, 255, 255)
            CustomRole timepostor = DillyzUtil.createRole("TiMEpostor", "Manipulate the time line.", true, false, new Color32(85, 50, 225, 255), true,
                CustomRoleSide.Impostor, VentPrivilege.Impostor, true, true);
            timepostor.a_or_an = "a";
            timepostor.SetSprite(assembly, "DillyzLegacyPack.Assets.dillyzthe1.png");

            freezetime = DillyzUtil.addButton(assembly, "Freeze Time", "DillyzLegacyPack.Assets.freeze.png", 35f, false, new string[] { "TiMEpostor" }, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    causedTimeEvent = true;
                    reversingTime = true;
                    Log.LogInfo("Freeze time.");
                    FreezeTime(true);
                    DillyzUtil.InvokeRPCCall("time_reverse", delegate (MessageWriter writer) { });
                }
            );
            freezetime.textOutlineColor = timepostor.roleColor;
            freezetime.SetUseTimeButton(17.5f, delegate (KillButtonCustomData button, bool interrupted) {
                if (causedTimeEvent)
                {
                    Log.LogInfo("Continue time.");
                    FreezeTime(false);
                }
            });

            reversetime = DillyzUtil.addButton(assembly, "Reverse Time", "DillyzLegacyPack.Assets.reverse.png", 4f, false, new string[] { "TiMEpostor" }, empty,
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    Log.LogInfo("Reverse time for " + timeReversed + "s.");
                    causedTimeEvent = true;
                    reversingTime = true;
                    FreezeTime(true);
                    timeReversedOn = DateTime.UtcNow;
                    DillyzUtil.InvokeRPCCall("time_reverse", delegate (MessageWriter writer) { writer.Write(true); });
                }
            );
            reversetime.textOutlineColor = new Color32(255, 0, 51, 255);
            reversetime.SetUseTimeButton(timeReversed, delegate (KillButtonCustomData button, bool interrupted) {
                if (causedTimeEvent)
                {
                    Log.LogInfo("Continue time.");
                    FreezeTime(false);
                    reversingTime = false;
                    DillyzUtil.InvokeRPCCall("time_reverse", delegate (MessageWriter writer) { writer.Write(false); });
                }
            });

            #endregion

            #region rpc
            DillyzUtil.AddRpcCall("SecondChance", delegate (MessageReader reader)
            {
                SecondChance(DillyzUtil.findPlayerControl(reader.ReadByte()), false);
            });
            DillyzUtil.AddRpcCall("RevealPhoenixZero", delegate (MessageReader reader)
            {
                PlayerControl p = DillyzUtil.findPlayerControl(reader.ReadByte());
                bool on = reader.ReadBoolean();

                if (on)
                    namesPublic.Add(p.PlayerId);
                else
                {
                    namesPublic.Remove(p.PlayerId);
                    p.ToggleHighlight(false, RoleTeamTypes.Crewmate);
                }

                //  targetPhoenix.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", phoenixzero.nameColorPublic ? 2f : 0f);
                //  targetPhoenix.gameObject.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", DillyzUtil.color32ToColor(phoenixzero.roleColor));
            });
            DillyzUtil.AddRpcCall("time_freeze", delegate(MessageReader reader) {
                bool active = reader.ReadBoolean();
                FreezeTime(active);

                if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor" && freezetime.GameInstance != null)
                {
                    freezetime.GameInstance.lastUse = DateTime.UtcNow;
                    freezetime.GameInstance.useTimerMode = false;
                    causedTimeEvent = false;

                    TimeSpan timeoff = new TimeSpan(0, 0, 0, -((int)Math.Floor(freezetime.useTime)), -((int)Math.Floor((freezetime.useTime * 1000) % 1000)));
                    freezetime.GameInstance.lastUse.Add(timeoff);
                }
            });
            DillyzUtil.AddRpcCall("time_reverse", delegate (MessageReader reader) {
                bool active = reader.ReadBoolean();
                FreezeTime(active);
                reversingTime = active;
                timeReversedOn = DateTime.UtcNow;

                if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "TiMEpostor" && freezetime.GameInstance != null)
                {
                    reversetime.GameInstance.lastUse = DateTime.UtcNow;
                    reversetime.GameInstance.useTimerMode = false;
                    causedTimeEvent = false;

                    TimeSpan timeoff = new TimeSpan(0, 0, 0, -((int)Math.Floor(reversetime.useTime)), -((int)Math.Floor((reversetime.useTime * 1000) % 1000)));
                    reversetime.GameInstance.lastUse.Add(timeoff);
                }
            });
            #endregion

            #region settings
            // -- Phoenix --
            //phoenix.AddAdvancedSetting_Boolean("Suicide Button", false, delegate (bool v) { advice.allowedRoles.Clear(); if (v) advice.allowedRoles.Add(phoenix.name); });
            //phoenix.AddAdvancedSetting_String("2nd Chance Allowed", chanceMode, new string[] { "Tasks Done", "Death", "Meeting Over", "Exile Only", "Kill Only" }, delegate (string v) { chanceMode = v; });
            phoenix.AddAdvancedSetting_Float("Wrath Cooldown", 15, 5, 75, 5, delegate (float v) { wrath.cooldown = v; }).suffix = "s";
            phoenix.AddAdvancedSetting_String("Wrath Disabled On", wrathDisables, new string[] { "Any Kill", "Impostor Kill", "Crewmate Kill", "Other Kill", "Non-Crew Kill", "None"}, delegate(string v) { wrathDisables = v; });
            phoenix.AddAdvancedSetting_Float("Comm. Cooldown", 35, 5, 100, 5, delegate(float v) { communicate.cooldown = v; }).suffix = "s";
            phoenix.AddAdvancedSetting_String("Comm. Disabled On", communicateDisables, new string[] { "Revive Any", "Revive Impostor", "Revive Crewmate", "Revive Other", "None"}, delegate(string v) { communicateDisables = v; });
            phoenix.AddAdvancedSetting_Float("Reveal Cooldown", 60, 10, 115, 5, delegate(float v) { revealbutton.cooldown = v; }).suffix = "s";
            phoenix.AddAdvancedSetting_Float("Reveal Timer", 10, 5, 40, 2.5f, delegate (float v) { revealbutton.useTime = v; }).suffix = "s";

            // -- TiMEpostor --
            timepostor.AddAdvancedSetting_Boolean("Time Freezing", true, delegate (bool v) { freezetime.allowedRoles.Clear(); if (v) freezetime.allowedRoles.Add(timepostor.name); });
            timepostor.AddAdvancedSetting_Float("Freezing Cooldown", 35, 5, 75, 5, delegate(float v) { freezetime.cooldown = v; }).suffix = "s";
            timepostor.AddAdvancedSetting_Float("Frozen Duration", 17.5f, 7.5f, 30f, 2.5f, delegate (float v) { freezetime.useTime = v; }).suffix = "s";
            timepostor.AddAdvancedSetting_Boolean("Time Reversal", true, delegate(bool v) { reversetime.allowedRoles.Clear(); if (v) reversetime.allowedRoles.Add(timepostor.name); });
            timepostor.AddAdvancedSetting_Float("Revering Cooldown", 40, 5, 85, 5, delegate (float v) { reversetime.cooldown = v; }).suffix = "s";
            timepostor.AddAdvancedSetting_Float("Reversal Duration", timeReversed, 5f, 25f, 2.5f, delegate (float v) { timeReversed = v; reversetime.useTime = v; }).suffix = "s";
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
            DillyzUtil.InvokeRPCCall("SecondChance", delegate (MessageWriter writer) {
                writer.Write(player.PlayerId);
                writer.Write(randomness);
            });
        }

        public static void FreezeTime(bool frozen)
        {
            timeFrozen = frozen;
            PlayerControl player = PlayerControl.LocalPlayer;
            if (DillyzUtil.getRoleName(player) != "TiMEpostor" || reversingTime)
            {
                foreach (CustomButton button in CustomButton.AllCustomButtons)
                    button.GameInstance.blockingButton = timeFrozen;

                KillAnimation.SetMovement(player, !timeFrozen);
            }
            if (reversetime.GameInstance != null)
                reversetime.GameInstance.blockingButton = timeFrozen;


        }
    }
}
