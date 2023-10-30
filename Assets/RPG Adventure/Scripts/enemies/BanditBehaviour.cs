using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RpgAdventure
{
    public class BanditBehaviour : MonoBehaviour
    {
        public PlayerScanner playerScanner;
        public float timeToStopFollowing = 5.0f;
        public float rotationSpeed = 1.0f;

        private PlayerController m_Player;
        private NavMeshAgent m_NavMeshAgent;
        private float m_TimeSinceLostPlayer = 0.0f;
        private Vector3 banditRotation;
        private Vector3 banditOriginPosition;
        private Animator m_Animator;

        private readonly int m_HashInPursuitPara = Animator.StringToHash("InPersuit");
        private readonly int m_IdlePosition = Animator.StringToHash("IdlePosition");

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            banditOriginPosition = transform.position;
            banditRotation = transform.eulerAngles;
            
        }

        private void Update()
        {
            var target = playerScanner.DetectPlayer(transform);

            if (target)
            {
                m_TimeSinceLostPlayer = 0f;
                m_Player = target;
                Vector3 targetPosition = m_Player.transform.position;

                m_NavMeshAgent.speed = 8.0f;
                m_NavMeshAgent.acceleration = 14.0f;

                m_NavMeshAgent.SetDestination(targetPosition);
                m_Animator.SetBool(m_HashInPursuitPara, true);
                m_Animator.SetBool(m_IdlePosition, false);
            }
            else
            {
                m_NavMeshAgent.acceleration = 8.0f;
                m_Animator.SetBool(m_IdlePosition, m_NavMeshAgent.velocity.magnitude < 0.1f);
                m_Animator.SetBool(m_HashInPursuitPara, false);

                if (m_TimeSinceLostPlayer >= timeToStopFollowing)
                {
                    m_TimeSinceLostPlayer = timeToStopFollowing;
                } else
                {
                    m_TimeSinceLostPlayer += Time.deltaTime;
                }
                if (m_TimeSinceLostPlayer >= timeToStopFollowing)
                {
                    m_Player = null;
                    m_NavMeshAgent.SetDestination(banditOriginPosition);
                    Debug.Log("returning to home");
                }
                HandleRotation();
            }
        }

        void HandleRotation()
        {

            if ((transform.position - banditOriginPosition).magnitude < 0.5f )
            {
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
        }
#endif
    }
}
