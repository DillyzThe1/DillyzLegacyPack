using DillyzRoleApi_Rewritten;
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

        void Update()
        {
            if (!setup)
                return;

            this.rend.enabled = DillyzLegacyPackMain.swordsOut.Contains(pc.PlayerId);

            this.transform.position = PlayerControl.LocalPlayer.transform.position;

            if (this.pc.PlayerId != PlayerControl.LocalPlayer.PlayerId || !rend.enabled)
                return;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.pc.transform.position;
            float angle = Mathf.Atan2(pos.y, pos.x) * (180f/ (float)Math.PI);
            rend.flipX = !(angle < -90 || angle > 90);
            Quaternion rot = Quaternion.Euler(0f, 0f, angle - 90);
            this.transform.localRotation = rot;
        }

        public void Setup(PlayerControl pc) {
            this.pc = pc;
            //this.pc.SetPlayerMaterialColors(rend);
            this.transform.parent = this.pc.transform;

            this.gameObject.name = "katana";
            this.rend = this.gameObject.AddComponent<SpriteRenderer>();
            //this.rend.material = new Material(Shader.Find("Unlit/PlayerShader"));
            this.rend.sprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "DillyzLegacyPack.Assets.katana kitchen knife.png");

            this.col2d = this.gameObject.AddComponent<BoxCollider2D>();
            this.col2d.isTrigger = true;
            this.col2d.size = new Vector2(this.col2d.size.x * 0.15f, this.col2d.size.y * 0.85f);
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

            if (target != null && !target.Data.IsDead && !target.inVent && target.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                DillyzUtil.RpcCommitAssassination(pc, target, false);
        }
    }
}
