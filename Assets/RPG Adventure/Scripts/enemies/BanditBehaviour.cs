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

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
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

                m_NavMeshAgent.SetDestination(targetPosition);
            }
            else
            {
                if(m_TimeSinceLostPlayer >= timeToStopFollowing)
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

            if (transform.position.x == banditOriginPosition.x && transform.position.z == banditOriginPosition.z)
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
