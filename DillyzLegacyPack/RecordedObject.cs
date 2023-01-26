using AmongUs.GameOptions;
using DillyzRoleApi_Rewritten;
using PowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DillyzLegacyPack
{
    // attach this to any object you want affected by time reversal
    [Il2CppItem]
    class RecordedObject : MonoBehaviour
    {
        public bool canVanish = false;
        DateTime creation = DateTime.MinValue;
        DateTime lowest = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public Rigidbody2D rb2d = null;
        public SpriteRenderer spr = null;
        public Animator anim = null;

        public PlayerControl pc;

        List<TimeStore> timeStores = new List<TimeStore>();

        void Start() {
            canVanish = false;
            creation = DateTime.UtcNow;
            TimeStore funnyStore = new TimeStore(this.transform.position, this.transform.rotation, Vector2.zero, false, null);
            funnyStore.savepoint = DateTime.MaxValue;
            timeStores.Add(funnyStore);
        }

        void Update() {
            if (DillyzLegacyPackMain.reversingTime) {
                RewindFrame();
                return;
            }
            if (timeStores.Count > Mathf.Round((DillyzLegacyPackMain.timeReversed*2f) / Time.fixedDeltaTime))
                timeStores.RemoveAt(0);
            timeStores.Add(new TimeStore(this.transform.position, this.transform.rotation, ((rb2d != null) ? rb2d.velocity : Vector2.zero), (spr != null) ? spr.flipX : false, this.pc));

            if (rb2d != null)
                rb2d.isKinematic = false;
        }

        void RewindFrame() {
            if (timeStores.Count <= 0)
                return;

            if (rb2d != null)
                rb2d.isKinematic = true;

            TimeStore storedTime = timeStores[timeStores.Count - 1];
            TimeSpan timestart = storedTime.savepoint - DillyzLegacyPackMain.timeReversedOn;

            if (timestart.TotalMilliseconds > 0)
                return;

            this.transform.position = storedTime.position;
            this.transform.rotation = storedTime.rotation;
            if (rb2d != null)
                this.rb2d.velocity = Vector2.zero;
            TimeSpan timeDiff = storedTime.savepoint - creation;
            if (spr != null)
                spr.flipX = storedTime.flipX;
            timeStores.RemoveAt(timeStores.Count - 1);

            if (canVanish)
                DillyzLegacyPackMain.reversetime.buttonText = timeDiff.TotalMilliseconds + "s";

            if (canVanish && timeDiff.TotalMilliseconds < 0)
            {
                DillyzLegacyPackMain.reversetime.buttonText = "I SHOULD BE DYING";
                GameObject.Destroy(this.gameObject);
                return;
            }

            if (this.pc != null && storedTime.hadPC) {
                if (this.pc.inVent != storedTime.inVent)
                {

                    Vent closestVent = Vent.currentVent;
                    double lastdist = Double.MaxValue;

                    foreach (Vent vent in ShipStatus.Instance.AllVents) {
                        double dist = DillyzUtil.getDist(new Vector2(vent.transform.position.x, vent.transform.position.y), this.pc.GetTruePosition());
                        if (dist < lastdist) {
                            lastdist = dist;
                            closestVent = vent;
                        }
                    }

                    this.pc.walkingToVent = false;
                    if (closestVent != null)
                        if (storedTime.inVent)
                            closestVent.EnterVent(this.pc);
                        else
                            closestVent.ExitVent(this.pc);
                    if (this.pc == PlayerControl.LocalPlayer)
                        closestVent.SetButtons(storedTime.inVent);

                    this.pc.inVent = storedTime.inVent;
                    this.pc.Visible = !storedTime.inVent;
                }
                if (this.pc.PlayerId == PlayerControl.LocalPlayer.PlayerId && DillyzUtil.getRoleName(this.pc) != storedTime.role)
                {
                    CustomRole rollleeeeeeeeee = CustomRole.getByName(storedTime.role);
                    bool beimp = (storedTime.role == "Impostor" || storedTime.role == "ShapeShifter" || (rollleeeeeeeeee != null && rollleeeeeeeeee.switchToImpostor));
                    if (beimp)
                        this.pc.RpcSetRole(RoleTypes.Impostor);
                    else
                        this.pc.RpcSetRole(RoleTypes.Crewmate);

                    this.pc.SetTasks(this.pc.Data.Tasks);
                    DillyzLegacyPackMain.Instance.Log.LogInfo(storedTime.role + " is a" + (beimp ? "n Impostor" : " Crewmate") + " role.");
                    DillyzUtil.RpcSetRole(this.pc, storedTime.role);
                }
                if (this.pc.Data.IsDead != storedTime.isDead && !DillyzLegacyPackMain.lastKilledByTiMEpostor.Contains(this.pc.PlayerId))
                {
                    if (storedTime.isDead)
                    {
                        this.pc.Die(DeathReason.Kill, false);
                        DeadBody deadBody = UnityEngine.Object.Instantiate(this.pc.KillAnimations[0].bodyPrefab);
                        deadBody.enabled = true;
                        deadBody.ParentId = this.pc.PlayerId;
                        this.pc.SetPlayerMaterialColors(deadBody.bodyRenderer);
                        this.pc.SetPlayerMaterialColors(deadBody.bloodSplatter);
                        Vector3 vector = this.pc.transform.position + this.pc.KillAnimations[0].BodyOffset;
                        vector.z = vector.y / 1000f;
                        deadBody.transform.position = vector;
                    }
                    else
                    {
                        this.pc.Revive();
                        List<DeadBody> bodies = FindObjectsOfType<DeadBody>().ToArray().ToList();
                        foreach (DeadBody body in bodies)
                            if (body.ParentId == this.pc.PlayerId)
                            {
                                DillyzLegacyPackMain.Instance.Log.LogInfo(body.gameObject.name + " you're");
                                Destroy(body.gameObject);
                            }
                        CustomRole.setRoleName(this.pc.PlayerId, storedTime.role);
                    }
                }

                SpriteAnim animtorrrr = this.pc.GetComponent<PlayerPhysics>().Animations.Animator;
                if (storedTime.clip != null && animtorrrr.m_currAnim.name != storedTime.clip.name)
                    animtorrrr.Play(storedTime.clip);
                if (this.pc.cosmetics.skin != null && storedTime.clip_skin != null && this.pc.cosmetics.skin.animator.m_currAnim.name != storedTime.clip_skin.name)
                    this.pc.cosmetics.skin.animator.Play(storedTime.clip_skin);
            }
        }
    }

    class TimeStore {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 velocity;
        public bool flipX = false;
        public DateTime savepoint;

        public bool hadPC = false;
        public bool inVent = false;
        public string role = "";
        public bool isDead = false;
        public AnimationClip clip = null;
        public AnimationClip clip_skin = null;

        public TimeStore(Vector3 position, Quaternion rotation, Vector2 velocity, bool flipX, PlayerControl player)
        {
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
            this.savepoint = DateTime.UtcNow;
            this.flipX = flipX;

            if (player != null)
            {
                this.hadPC = true;
                this.inVent = player.inVent;
                this.role = DillyzUtil.getRoleName(player);
                this.isDead = player.Data.IsDead;
                this.clip = player.GetComponent<PlayerPhysics>().Animations.Animator.m_currAnim;
                if (player.cosmetics.skin != null)
                    this.clip_skin = player.cosmetics.skin.animator.m_currAnim;
            }
        }
    }
}
