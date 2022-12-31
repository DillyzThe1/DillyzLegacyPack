using DillyzRoleApi_Rewritten;
using System;
using System.Collections.Generic;
using System.Linq;
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

        void Start() {
            this.rend = this.gameObject.AddComponent<SpriteRenderer>();
            //this.rend.material = new Material(Shader.Find("Unlit/PlayerShader"));

            this.col2d = this.gameObject.AddComponent<BoxCollider2D>();
            this.col2d.isTrigger = true;
        }
        void Update() {

            rend.enabled = DillyzLegacyPackMain.swordsOut.Contains(pc.PlayerId);

            if (this.pc.PlayerId != PlayerControl.LocalPlayer.PlayerId || !rend.enabled)
                return;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.pc.transform.position;
            float angle = Mathf.Atan2(pos.y, pos.x) * (180f/ (float)Math.PI);
            rend.flipX = angle < -90 || angle > 90;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            this.transform.localRotation = rot;
        }

        public void Setup(PlayerControl pc) {
            if (!setup)
                return;
            this.pc = pc;
            //this.pc.SetPlayerMaterialColors(rend);
            this.transform.parent = this.pc.transform;

            this.col2d.enabled = (this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId);
        }

        void OnTriggerEnter2D(Collider2D tag) {
            PlayerControl target = tag.gameObject.GetComponent<PlayerControl>();

            if (target != null && !target.Data.IsDead && !target.inVent && target.PlayerId != PlayerControl.LocalPlayer.PlayerId)
                DillyzUtil.RpcCommitAssassination(pc, target, false);
        }
    }
}
