using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public partial class Damageable : MonoBehaviour
    {
        public int maxHealth = 100;
        [Range(0, 360)]
        public float hitAngle = 270f;

        public void ApplyDamage(DamageMessage data)
        {
            Debug.Log("Applying Damage");
            Debug.Log(data.amount);
            Debug.Log(data.damageSource);
            Debug.Log(data.damager);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.white;

            Vector3 rotatedForward = Quaternion.Euler(
                                        0,
                                        -hitAngle * 0.5f,
                                        0)
                                        * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                hitAngle,
                1.0f
                );
        }
#endif
    }

}
