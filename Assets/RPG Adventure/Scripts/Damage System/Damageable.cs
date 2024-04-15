using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public partial class Damageable : MonoBehaviour
    {
        
        [Range(0, 360)]
        public float hitAngle = 270f;
        public int maxHealth = 100;
        public int CurrentHitpoints { get; private set; }

        private void Awake()
        {
            CurrentHitpoints = maxHealth;
        }


        public void ApplyDamage(DamageMessage data)
        {
            if (CurrentHitpoints <= 0)
            {
                return;
            }

            Vector3 positionToDamager = data.damageSource - transform.position;
            positionToDamager.y = 0;

            if(Vector3.Angle(transform.forward, positionToDamager) > hitAngle * 0.5) 
            {
                Debug.Log("not hitting");
            } else
            {
                Debug.Log("hit");
            }

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
