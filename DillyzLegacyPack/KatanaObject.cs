﻿using DillyzRoleApi_Rewritten;
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

        float lastangle = 0f;

        void UpdateAngle(float funnyangle) {
            lastangle = funnyangle;
            float degrees = lastangle * (180f / (float)Math.PI);
            rend.flipX = !(degrees < -90 || degrees > 90);
            Quaternion rot = Quaternion.Euler(0f, 0f, degrees - 90);
            // this code made me want to a bridge
            float bruh = lastangle;
            this.transform.position += (new Vector3(Mathf.Cos(bruh), Mathf.Sin(bruh), 0f) * distfromplayer);
            this.transform.localRotation = rot;
        }

        void Update()
        {
            if (!setup)
                return;

            this._enabled = (DillyzLegacyPackMain.swordsOut.Contains(pc.PlayerId) || DillyzUtil.InFreeplay()) && !pc.Data.IsDead;
            this.rend.enabled = this._enabled;

            this.transform.position = this.pc.transform.position;

            if (this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId && this.enabled && !DillyzLegacyPackMain.reversingTime && !DillyzLegacyPackMain.timeFrozen)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.pc.transform.position;
                UpdateAngle(Mathf.Atan2(pos.y, pos.x));
                return;
            }
            UpdateAngle(lastangle);
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
        }

        void OnTriggerEnter2D(Collider2D tag)
        {
            if (!setup)
                return;
            PlayerControl target = tag.gameObject.GetComponent<PlayerControl>();

            if (this.enabled && target != null && !target.Data.IsDead && !target.inVent && target.PlayerId != PlayerControl.LocalPlayer.PlayerId && !DillyzLegacyPackMain.reversingTime)
                DillyzUtil.RpcCommitAssassination(pc, target, false);
        }
    }
}
