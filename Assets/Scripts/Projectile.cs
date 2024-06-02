using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

namespace Assets.Scripts {
    public class Projectile : MonoBehaviour{
        public float speed = 10.0f;

        public EnemyAI Target;
        public Character Parent;
        
        private void Awake() {
            
        }
        private void Start() {
            this.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
            speed += (Parent.MergeLevel/2);
            Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        private void FixedUpdate() {
            if (Target != null) {
                if (Target.IsDead()) {
                    Destroy(gameObject);
                }
                // Move our position a step closer to the target.
                var step = speed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, step);

                // Check if the position of the cube and sphere are approximately equal.
                if (Vector3.Distance(transform.position, Target.transform.position) < 0.001f) {
                    // Swap the position of the cylinder.
                    Target.TakeDamage(Parent.Damage);
                    Destroy(gameObject);
                    return;
                }
            }
            else
                Destroy(gameObject);
        }
    }
}
