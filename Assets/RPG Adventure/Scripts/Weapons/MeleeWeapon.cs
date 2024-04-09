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

        public int damage = 10;

        public AttackPoint[] attackPoints = new AttackPoint[0];

        public void BeginAttack()
        {
            Debug.Log("Shwing");
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
