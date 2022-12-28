﻿using System;
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

        public override void Load()
        {
            Instance = this;

            Log.LogInfo(DillyzLegacyPackMain.MOD_NAME + " v" + DillyzLegacyPackMain.MOD_VERSION + " loaded. Hooray!");
            harmony.PatchAll();

            assembly = Assembly.GetExecutingAssembly();

            #region phoenix
            // normal Phoenix
            CustomRole phoenix = DillyzUtil.createRole("Phoenix", "Reveal your hidden power.", true, false, new Color32(225, 65, 25, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            phoenix.a_or_an = "a";
            phoenix.SetSprite(assembly, "DillyzLegacyPack.Assets.dillyzthe1.png");
            phoenix.roletoGhostInto = "Phoenix's Ghost";

            CustomButton advice = DillyzUtil.addButton(assembly, "Take Advice", "DillyzLegacyPack.Assets.dillyzthe1.png", 0.1f, false, new string[] { "Phoenix" }, new string[] { }, 
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

            CustomButton secondchance = DillyzUtil.addButton(assembly, "Second Chance", "DillyzLegacyPack.Assets.dillyzthe1.png", 20f, false, new string[] { "Phoenix's Ghost" }, new string[] { },
                delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    PlayerControl.LocalPlayer.Revive();
                    DillyzUtil.RpcSetRole(PlayerControl.LocalPlayer, "Phoenix Zero");
                    byte randomness = 0;

                    switch (ShipStatus.Instance.name.ToLower().Replace("(clone)","")) {
                        case "skeldship":
                            PlayerControl.LocalPlayer.transform.position = new Vector3(-7.25f, -4.85f, 0f);
                            break;
                        case "miraship":
                            PlayerControl.LocalPlayer.transform.position = new Vector3(-16.25f, -0.5f, 0f);
                            break;
                        case "polusship":
                            PlayerControl.LocalPlayer.transform.position = new Vector3(40.375f, -6.75f, 0f);
                            break;
                        case "airship":
                            randomness = (byte)UnityEngine.Random.Range(0, 3);
                            switch (randomness) {
                                case (byte)0:
                                    PlayerControl.LocalPlayer.transform.position = new Vector3(29.25f, 7.25f, 0f);
                                    break;
                                case (byte)1:
                                    PlayerControl.LocalPlayer.transform.position = new Vector3(30.8f, 7.25f, 0f);
                                    break;
                                case (byte)2:
                                    PlayerControl.LocalPlayer.transform.position = new Vector3(32.35f, 7.25f, 0f);
                                    break;
                                case (byte)3:
                                    PlayerControl.LocalPlayer.transform.position = new Vector3(33.75f, 7.25f, 0f);
                                    break;
                            }
                            break;
                    }

                    DillyzUtil.InvokeRPCCall("SecondChance", delegate (MessageWriter writer) {
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(randomness);
                    });
                }
            );
            secondchance.textOutlineColor = phoenixghost.roleColor;

            // afterlife Phoenix
            CustomRole phoenixzero = DillyzUtil.createRole("Phoenix Zero", "Use the power of the afterlife.", true, false, new Color32(240, 85, 40, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            phoenixzero.a_or_an = "a";
            phoenixzero.roleSeleciton = false;
            phoenixzero.hasSettings = false;
            #endregion

            #region sensei
            CustomRole sensei = DillyzUtil.createRole("Sensei", "Slice through suspects to victory.", true, false, new Color32(125, 45, 200, 255), false,
                CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
            sensei.a_or_an = "a";
            sensei.SetSprite(assembly, "DillyzLegacyPack.Assets.dillyzthe1.png");
            #endregion

            #region time freeze button
            // todo
            #endregion

            #region rpc
            DillyzUtil.AddRpcCall("SecondChance", delegate(MessageReader reader) {
                DillyzUtil.findPlayerControl(reader.ReadByte()).Revive();
                int randomness = reader.ReadByte();

                switch (ShipStatus.Instance.name.ToLower().Replace("(clone)", ""))
                {
                    case "skeldship":
                        PlayerControl.LocalPlayer.transform.position = new Vector3(-7.25f, -4.85f, 0f);
                        break;
                    case "miraship":
                        PlayerControl.LocalPlayer.transform.position = new Vector3(-16.25f, -0.5f, 0f);
                        break;
                    case "polusship":
                        PlayerControl.LocalPlayer.transform.position = new Vector3(40.375f, -6.75f, 0f);
                        break;
                    case "airship":
                        randomness = UnityEngine.Random.Range(0, 3);
                        switch (randomness)
                        {
                            case (byte)0:
                                PlayerControl.LocalPlayer.transform.position = new Vector3(29.25f, 7.25f, 0f);
                                break;
                            case (byte)1:
                                PlayerControl.LocalPlayer.transform.position = new Vector3(30.8f, 7.25f, 0f);
                                break;
                            case (byte)2:
                                PlayerControl.LocalPlayer.transform.position = new Vector3(32.35f, 7.25f, 0f);
                                break;
                            case (byte)3:
                                PlayerControl.LocalPlayer.transform.position = new Vector3(33.75f, 7.25f, 0f);
                                break;
                        }
                        break;
                }
            });
            #endregion
        }
    }
}
