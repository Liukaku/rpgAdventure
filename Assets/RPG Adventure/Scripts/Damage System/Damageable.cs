using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RpgAdventure.IMessageReceiver;

namespace RpgAdventure
{
    public partial class Damageable : MonoBehaviour
    {
        
        [Range(0, 360)]
        public float hitAngle = 270f;
        public int maxHealth = 100;
        public int CurrentHitpoints { get; private set; }
        public int experience;

        public float invulnTime = 0.5f;

        public LayerMask playerActionReceivers;

        private bool isInvuln = false;
        public float timeSinceLastHit = 0.0f;



        public List<MonoBehaviour> onDamageMessageReceivers;

        private void Awake()
        {
            CurrentHitpoints = maxHealth;
            if (0 != (playerActionReceivers.value & 1 << gameObject.layer))
            {
            onDamageMessageReceivers.Add(FindObjectOfType<QuestManager>());
                onDamageMessageReceivers.Add(FindObjectOfType<PlayerStats>());
            }
        }

        public void Update()
        {
            if (isInvuln)
            {
                timeSinceLastHit += Time.deltaTime;

                if (timeSinceLastHit > invulnTime)
                {
                    isInvuln = false;
                    timeSinceLastHit = 0.0f;
                }
            }
        }


        public void ApplyDamage(DamageMessage data)
        {
            Debug.Log(isInvuln);
            if (CurrentHitpoints <= 0 || isInvuln)
            {
                return;
            }

            Vector3 positionToDamager = data.damageSource.transform.position - transform.position;
            positionToDamager.y = 0;

            if(Vector3.Angle(transform.forward, positionToDamager) > hitAngle * 0.5) 
            {
                return;
            }

            isInvuln = true;

            CurrentHitpoints -= data.amount;

            var messageType = CurrentHitpoints <= 0 ? MessageType.DEAD : MessageType.DAMAGED;
            
            for(int i = 0; i < onDamageMessageReceivers.Count; i++)
            {
                var receiver = onDamageMessageReceivers[i] as IMessageReceiver;

                receiver.OnReceiveMessage(messageType, this, data);
                

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
