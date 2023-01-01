using DillyzRoleApi_Rewritten;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzLegacyPack
{
    [Il2CppItem]
    class KatanaObject : MonoBehaviour
    {
        public static List<KatanaObject> allObjects = new List<KatanaObject>();

        bool setup = false;
        SpriteRenderer rend;
        PlayerControl pc;
        BoxCollider2D col2d;

        RecordedObject rec;

        float distfromplayer = 1f;

        bool _enabled = false;
        public bool enabled => _enabled;

        float lastAngle = 0f;
        float curLerp = 0f;

        public static KatanaObject getByPlayerId(byte playerId) {
            foreach (KatanaObject obj in allObjects)
                if (obj.pc != null && obj.pc.PlayerId == playerId)
                    return obj;
            return null;
        }

        public void UpdateAngle(float funnyangle) {
            lastAngle = funnyangle;

            if (this.pc != null && this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId && this.enabled && !DillyzUtil.InFreeplay()) {
                DillyzUtil.InvokeRPCCall("sensei_katana_swing", delegate (MessageWriter writer) {
                    writer.Write(this.pc.PlayerId);
                    writer.Write(Mathf.RoundToInt(lastAngle * 100f));
                }); 
            }

            // DOING THIS BC LOCAL ROTATION HATES ME
            float degrees = lastAngle * (180f / (float)Math.PI);
            rend.flipX = !(degrees < -90 || degrees > 90);
            Quaternion rot = Quaternion.Euler(0f, 0f, degrees - 90);
            this.transform.localRotation = rot;
        }

        void UpdateDegrees(float rawAngle) { //, float degrees) {
            //float degrees = lastangle * (180f / (float)Math.PI);
            //rend.flipX = !(degrees < -90 || degrees > 90);
            //Quaternion rot = Quaternion.Euler(0f, 0f, degrees - 90);
            // this code made me want to a bridge 
            this.transform.position += (new Vector3(Mathf.Cos(rawAngle), Mathf.Sin(rawAngle), 0f) * distfromplayer);
            //this.transform.localRotation = rot; 
        }

        void Update() { 
            if (!setup || this.pc == null)
                return;

            this._enabled = DillyzLegacyPackMain.swordsOut.Contains(pc.PlayerId) && !pc.Data.IsDead;
            this.rend.enabled = this._enabled;

            this.transform.position = this.pc.transform.position;

            if (this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId && this.enabled && !DillyzLegacyPackMain.reversingTime && !DillyzLegacyPackMain.timeFrozen)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.pc.transform.position;
                UpdateAngle(Mathf.Atan2(pos.y, pos.x));
            }
            else
                UpdateAngle(lastAngle);

            float goober = Time.deltaTime * 16.5f;
            float bruh = (180f / (float)Math.PI);
            float lastangcalc = (lastAngle * bruh);
            float lastlerpcalc = (curLerp * bruh);
            if (lastlerpcalc > 120 && lastangcalc < -120) // up to dwn
                curLerp = lastAngle;
            else if (lastlerpcalc < -120 && lastangcalc > 120) // down to p
                curLerp = lastAngle;

            curLerp = Mathf.Lerp(curLerp, lastAngle, goober);

            //if (this.pc != null)
            //    this.pc.name = goober + " " + lastangcalc + " " + lastlerpcalc;
            UpdateDegrees(curLerp);//, curLerp * (180f / (float)Math.PI));
        }

        public void Setup(PlayerControl pc) {
            this.pc = pc;
            //this.pc.SetPlayerMaterialColors(rend);
            this.transform.parent = this.pc.transform;

            this.transform.localScale = Vector3.one * 0.375f;

            this.gameObject.name = "katana";
            this.rend = this.gameObject.AddComponent<SpriteRenderer>();
            //this.rend.material = new Material(Shader.Find("Unlit/PlayerShader"));
            this.rend.sprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "DillyzLegacyPack.Assets.katana kitchen knife.png");

            this.col2d = this.gameObject.AddComponent<BoxCollider2D>();
            this.col2d.isTrigger = true;
            this.col2d.size = new Vector2(0.025f, 0.15f);//new Vector2(this.col2d.size.x * 0.15f, this.col2d.size.y * 0.85f);
            //this.col2d.enabled = (this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId);

            this.rec = this.gameObject.AddComponent<RecordedObject>();
            this.rec.spr = this.rend;


            setup = true;

            allObjects.Add(this);
        }

        void OnDestroy() {
            allObjects.Remove(this);
        }

        void OnTriggerEnter2D(Collider2D tag)
        {
            if (!setup)
                return;
            PlayerControl target = tag.gameObject.GetComponent<PlayerControl>();

            if (this.enabled && target != null && !target.Data.IsDead && !target.inVent && target.PlayerId != PlayerControl.LocalPlayer.PlayerId
                                                && !DillyzLegacyPackMain.reversingTime && this.pc != null && this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            { 
                DillyzUtil.RpcCommitAssassination(pc, target, false);

                if (DillyzLegacyPackMain.sword.GameInstance == null)
                    return;
                DillyzLegacyPackMain.sword.GameInstance.useTimerMode = false;
                DillyzLegacyPackMain.sword.GameInstance.lastUse = DateTime.UtcNow;
                DillyzLegacyPackMain.sword.GameInstance.killButton.cooldownTimerText.color = Palette.White;
            }
        }
    }
}
