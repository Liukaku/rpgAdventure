using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RpgAdventure
{
    public class BanditBehaviour : MonoBehaviour, IAttackListener, IMessageReceiver
    {
        public PlayerScanner playerScanner;
        public float timeToStopFollowing = 5.0f;
        public float rotationSpeed = 1.0f;
        public float attackDistance = 7.0f;

        private PlayerController m_Player;
        private NavMeshAgent m_NavMeshAgent;
        public float m_TimeSinceLostPlayer = 0.0f;
        private Vector3 banditRotation;
        public Vector3 banditOriginPosition;
        private EnemyController m_EnemyController;

        private readonly int m_HashInPursuitPara = Animator.StringToHash("InPersuit");
        private readonly int m_IdlePosition = Animator.StringToHash("IdlePosition");
        private readonly int m_HashAttack = Animator.StringToHash("Attack");
        private readonly int m_Hurt = Animator.StringToHash("Hurt");
        private readonly int m_Dead = Animator.StringToHash("Dead");

        private void Awake()
        {
            m_EnemyController = GetComponent<EnemyController>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            banditOriginPosition = transform.position;
            banditRotation = transform.eulerAngles;
            
        }

        private void Update()
        {
            GuardPosition();
        }

        private void GuardPosition()
        {
            var target = playerScanner.DetectPlayer(transform);

            // player within sight radius
            if (target)
            {
                AttackOrMoveToPlayer(target);
            }
            else
            {
                // player not within range
                StopPersuit();
            }
        }

        public void OnReceiveMessage(IMessageReceiver.MessageType type)
        {
            switch (type)
            {
                case IMessageReceiver.MessageType.DAMAGED:
                    OnReceiveDamage();
                    break;
                case IMessageReceiver.MessageType.DEAD:
                    onDead();
                    break;
                default:
                    break;
            }
        }

        private void onDead()
        {
            m_EnemyController.Animator.SetTrigger(m_Dead);
        }

        private void OnReceiveDamage()
        {
            m_EnemyController.Animator.SetTrigger(m_Hurt);
        }

        private void StopPersuit()
        {
            playerScanner.SetDetectionAngle(110.0f);
            m_NavMeshAgent.acceleration = 8.0f;
            m_EnemyController.Animator.SetBool(m_IdlePosition, m_NavMeshAgent.velocity.magnitude < 0.1f);
            m_EnemyController.Animator.SetBool(m_HashInPursuitPara, false);

            m_TimeSinceLostPlayer += Time.deltaTime;

            Vector3 distanceToHome = transform.position - banditOriginPosition;
            Boolean distanceX = Math.Abs(distanceToHome.x) > 5.0f;
            Boolean distanceZ = Math.Abs(distanceToHome.z) > 5.0f;

            if (m_TimeSinceLostPlayer >= timeToStopFollowing && distanceX && distanceZ)
            {
                m_Player = null;
                m_EnemyController.FollowTarget(banditOriginPosition);
                Debug.Log("returning to home");
                HandleRotation();
            }
            
        }
        

        private void AttackOrMoveToPlayer(PlayerController target)
        {
            m_TimeSinceLostPlayer = 0f;
            m_Player = target;
            Vector3 targetPosition = m_Player.transform.position - transform.position;
            playerScanner.SetDetectionAngle(350.0f);

            // check if bandit is close enough to attack
            Debug.Log(targetPosition.magnitude <= attackDistance);
            if (targetPosition.magnitude <= attackDistance)
            {
                Debug.Log("Attacking player");
                AttackTarget(targetPosition - transform.position);
            }
            else
            {
                Debug.Log("moving to player");
                // move towards player
                m_EnemyController.FollowTarget(m_Player.transform.position);

            }
        }

        private void AttackTarget(Vector3 targetPosition)
        {
            var attackRotation = Quaternion.LookRotation(targetPosition);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, attackRotation, 360 * Time.deltaTime);


            m_EnemyController.Animator.SetBool(m_HashInPursuitPara, true);
            m_NavMeshAgent.enabled = false;
            m_EnemyController.Animator.SetTrigger(m_HashAttack);
        }



        public void MeleeAttackEnd()
        {

        }

        public void MeleeAttackStart()
        {

        }

        void HandleRotation()
        {

            if ((transform.position - banditOriginPosition).magnitude < 0.5f )
            {
                Debug.Log("rotating");
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.Euler(banditRotation);
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Color c = new Color(0.0f, 0.0f, 1.0f, 0.5f);
            UnityEditor.Handles.color = c;

            Vector3 rotatedForward = Quaternion.Euler(
                    0,
                    -playerScanner.detectionAngle * 0.5f,
                    0)
                * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                playerScanner.detectionAngle,
                playerScanner.detectionRange
                );

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                360.0f,
                playerScanner.meleeDetectionRadius
                );
        }
#endif
    }
}
