using DillyzRoleApi_Rewritten;
using Il2CppSystem.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzLegacyPack
{
    // attach this to any object you want affected by time reversal
    [Il2CppItem]
    class RecordedObject : MonoBehaviour
    {
        public bool canVanish = false;
        DateTime creation = DateTime.MinValue;

        public Rigidbody2D rb2d;

        List<TimeStore> timeStores = new List<TimeStore>();

        void Start() {
            canVanish = false;
            creation = DateTime.UtcNow;
        }

        void Update() {
            if (DillyzLegacyPackMain.reversingTime) {
                RewindFrame();
                return;
            }
            if (timeStores.Count > Mathf.Round(DillyzLegacyPackMain.timeReversed / Time.fixedDeltaTime))
                timeStores.RemoveAt(0);
            timeStores.Add(new TimeStore(this.transform.position, this.transform.rotation, (rb2d != null) ? rb2d.velocity : Vector2.zero));

            if (rb2d != null)
                rb2d.isKinematic = false;
        }

        void RewindFrame() {
            if (timeStores.Count <= 0)
                return;

            if (rb2d != null)
                rb2d.isKinematic = true;

            TimeStore storedTime = timeStores[timeStores.Count - 1];
            this.transform.position = storedTime.position;
            this.transform.rotation = storedTime.rotation;
            if (rb2d != null && storedTime.velocity != null)
                this.rb2d.velocity = storedTime.velocity;
            TimeSpan timeDiff = storedTime.savepoint - creation;
            timeStores.RemoveAt(timeStores.Count - 1);

            if (canVanish && timeDiff.TotalMilliseconds < 0)
            {
                GameObject.Destroy(this);
                return;
            }
        }
    }

    class TimeStore {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 velocity;
        public DateTime savepoint;

        public TimeStore(Vector3 position, Quaternion rotation, Vector2 velocity)
        {
            this.position = position;
            this.rotation = rotation;
            this.velocity = velocity;
            this.savepoint = DateTime.UtcNow;
        }
    }
}
