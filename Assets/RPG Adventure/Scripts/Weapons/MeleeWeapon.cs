using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class MeleeWeapon : MonoBehaviour
    {

        [System.Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform rootTransform;

        }

        public LayerMask targetLayer;

        public int damage = 10;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        public RandomAudioPlayer SwingAudio;
        public RandomAudioPlayer impactAudio;

        private bool m_IsAttack = false;
        private Vector3[] m_OriginAttackPos;
        private RaycastHit[] m_RaycastHitCache = new RaycastHit[32];
        private GameObject m_Owner;

        public void FixedUpdate()
        {
            if (m_IsAttack)
            {
                for(int i = 0; i < attackPoints.Length; i++)
                {
                    AttackPoint ap = attackPoints[i];
                    Vector3 worldPos = ap.rootTransform.position + ap.rootTransform.TransformVector(ap.offset);
                    Vector3 attackVector = (worldPos - m_OriginAttackPos[i]).normalized;

                    Ray r = new Ray(worldPos, attackVector);
                    Debug.DrawRay(worldPos, attackVector, Color.red, 4.0f);

                    int conacts = Physics.SphereCastNonAlloc(
                        r, 
                        ap.radius, 
                        m_RaycastHitCache, 
                        attackVector.magnitude, 
                        ~0, 
                        QueryTriggerInteraction.Ignore);

                    for(int j = 0; j < conacts; j++)
                    {
                        Collider collider = m_RaycastHitCache[j].collider;
                        
                        if(collider != null)
                        {

                            CheckDamage(collider, ap);
                        }
                    }

                    m_OriginAttackPos[0] = worldPos;

                }
            }
        }

        public void SetOwner(GameObject owner)
        {
            m_Owner = owner;
        }

        private void CheckDamage(Collider otherColl, AttackPoint ap)
        {

            if ((targetLayer.value & (1 << otherColl.gameObject.layer)) == 0)
            {
                return;
            }

            Damageable damageable = otherColl.GetComponent<Damageable>();

            if (damageable != null)
            {
                Damageable.DamageMessage data;
                data.amount = damage;
                data.damager = this;
                data.damageSource = m_Owner;
                if (impactAudio != null)
                {
                    impactAudio.PlayRandomClip();
                }
                damageable.ApplyDamage(data);
            }
        }

        public void SetTargetLayer(LayerMask targetMask)
        {
            targetLayer = targetMask;
        }

        public void BeginAttack()
        {
            SwingAudio.PlayRandomClip();
            m_IsAttack = true;
            m_OriginAttackPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; i++)
            {
                AttackPoint ap = attackPoints[i];
                m_OriginAttackPos[i] = ap.rootTransform.position + ap.rootTransform.TransformDirection(ap.offset);
            }
            
        }

        public void EndAttack()
        {
            m_IsAttack = false;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (AttackPoint attackPoint in attackPoints)
            {
                if(attackPoint.rootTransform != null)
                {
                    Vector3 worldPosition = attackPoint.rootTransform.TransformVector(attackPoint.offset);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(attackPoint.rootTransform.position + worldPosition, attackPoint.radius);

                }
            }
        }

#endif
    }

}
