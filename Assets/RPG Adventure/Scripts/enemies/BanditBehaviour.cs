using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RpgAdventure { 
    public class BanditBehaviour : MonoBehaviour
    {

        public float detectionRange = 10.0f;
        public float detectionAngle = 90.0f;

        private PlayerController m_Player;
        private NavMeshAgent m_NavMeshAgent;

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            m_Player = DetectPlayer();

            if (!m_Player) { return; }

           Vector3 targetPosition = m_Player.transform.position;

           m_NavMeshAgent.SetDestination(targetPosition);
        }

        private PlayerController DetectPlayer()
        {
            if(PlayerController.Instance == null) {
                return null;
            }

            Vector3 enemyPosition = transform.position;
            Vector3 disToPlayer = PlayerController.Instance.transform.position - enemyPosition;
            disToPlayer.y = 0; // to make debugging easier 

            // check if the player is within a general range
            if(disToPlayer.magnitude <= detectionRange)
            {
                // check if the player is within the angle specified in decectionAngle
                if(Vector3.Dot(disToPlayer.normalized, transform.forward) > 
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad ))
                {
                    Debug.Log("player detected");
                    return PlayerController.Instance;
                }
            } 
            return null;
        }

        # if UNITY_EDITOR
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
