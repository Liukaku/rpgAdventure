using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RpgAdventure
{
    public class BanditBehaviour : MonoBehaviour
    {

        public float detectionRange = 10.0f;
        public float detectionAngle = 90.0f;
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
            var target = DetectPlayer();

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

        private PlayerController DetectPlayer()
        {
            if (PlayerController.Instance == null)
            {
                return null;
            }

            Vector3 enemyPosition = transform.position;
            Vector3 disToPlayer = PlayerController.Instance.transform.position - enemyPosition;
            disToPlayer.y = 0; // to make debugging easier 

            // check if the player is within a general range
            if (disToPlayer.magnitude <= detectionRange)
            {
                // check if the player is within the angle specified in decectionAngle
                if (Vector3.Dot(disToPlayer.normalized, transform.forward) >
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                {
                    Debug.Log("player detected");
                    return PlayerController.Instance;
                }
            }
            return null;
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
                    -detectionAngle * 0.5f,
                    0)
                * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                detectionAngle,
                detectionRange
                );
        }
#endif
    }
}
