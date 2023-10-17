using Unity.VisualScripting;
using UnityEngine;

namespace RpgAdventure { 
    public class BanditBehaviour : MonoBehaviour
    {

        public float detectionRange = 10.0f;
        private void Start()
        {
            Debug.Log(PlayerController.Instance);
        }

        private void Update()
        {
            DetectPlayer();
        }

        private PlayerController DetectPlayer()
        {
            if(PlayerController.Instance == null) {
                return null;
            }

            Vector3 enemyPosition = transform.position;
            Vector3 disToPlayer = PlayerController.Instance.transform.position - enemyPosition;
            disToPlayer.y = 0; // to make debugging easier 

            if(disToPlayer.magnitude <= detectionRange)
            {
                Debug.Log("detecting the player ");
            } else
            {
                Debug.Log("where are you");
            }
            return null;
        }
    }
}
