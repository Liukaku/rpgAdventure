using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance { get { return s_Instance; } }

        private static PlayerInput s_Instance;
        private Vector3 m_Movement;
        private bool m_Attack;
        public bool m_AttackTwo;
        private bool m_Q;
        private Collider[] m_InteractTarget;

        public bool isPlayerInputBlocked;
        public float distanceToInteract = 5.0f;
        public bool canCombo = false;
        public float comboTimer = 0.0f;
        public float comboSpace = 0.7f;

        public Collider[] interactTarget
        {
            get { return m_InteractTarget; }
        }

        public bool IsMoveInput
        {
            get { return !Mathf.Approximately(MoveInput.magnitude, 0); }
        }
        public Vector3 MoveInput
        {
            get {
                if (isPlayerInputBlocked) 
                {
                    return Vector3.zero;
                }
                return m_Movement; 
            }
        }

        public bool IsAttack
        {

            get { return !isPlayerInputBlocked && m_Attack; }
        }

        public bool IsAttackTwo
        {

            get { return !isPlayerInputBlocked && m_AttackTwo; }
        }

        private void Awake()
        {
            s_Instance = this;
        }

        void Update()
        {
            m_Movement.Set(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));

            bool isLeftMouseClick = Input.GetMouseButtonDown(0);
            bool isRightMouseClick = Input.GetMouseButtonDown(1);
            if (Input.GetKeyDown(KeyCode.Q) && m_Q == false)
            {
                m_Q = true;
            }
            if (Input.GetKeyUp(KeyCode.Q) && m_Q == true)
            {
                m_Q = false;
            }
            if (m_Q == true)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleEInteract();
            }
            if (canCombo)
            {
                comboTimer += Time.deltaTime;
                if(comboTimer >= comboSpace)
                {
                    comboTimer = 0.0f;
                    canCombo = false;
                }
            }

            if (isLeftMouseClick && !m_Q)
            {
                Debug.Log("left");
                
                if (canCombo && !m_AttackTwo && comboTimer <= comboSpace)
                {

                    StartCoroutine(TriggerCombo());

                } else
                {
                    HandleLeftMouseButtonDown();
                }

            }
            if (isRightMouseClick)
            {
                //HandleRightMouseButtonDown();
            }
        }

        private void HandleLeftMouseButtonDown()
        {
            if (!m_Attack)
            {
                StartCoroutine(TriggerAttack());
            }
        }

        private void HandleRightMouseButtonDown()
        {
            Debug.Log("right mouse click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit && hit.collider.CompareTag("QuestGiver"))
            {
                Debug.Log("has hit: " + hit.collider.name);
                var distanceToTarget = (transform.position - hit.transform.position).magnitude;
                if (distanceToTarget <= distanceToInteract)
                {
                    Debug.Log("hello there");
                    //m_isTalking = true;
                }
            }
        }

        private void HandleEInteract()
        {
            Debug.Log("E");
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, distanceToInteract);
            StartCoroutine(TriggerInteract(colliderArray));

            //foreach (Collider collider in colliderArray)
            //{
            //    StartCoroutine(TriggerInteract(collider));
            //    if (collider.CompareTag("QuestGiver"))
            //    {
            //        Debug.Log("Hello there: " + collider.name);

            //    }
            //}
        }

        private IEnumerator TriggerInteract(Collider[] interactables)
        {
            m_InteractTarget = interactables;

            yield return new WaitForSeconds(0.3f);

            m_InteractTarget = null;
        }

        private IEnumerator TriggerAttack()
        {
            m_Attack = true;

            yield return new WaitForSeconds(0.3f);

            canCombo = true;
            m_Attack = false;
        }

        private IEnumerator TriggerCombo()
        {
            Debug.Log("combo");
            m_AttackTwo = true;

            yield return new WaitForSeconds(0.3f);

            comboTimer = 0.0f;
            canCombo = false;
            m_AttackTwo = false;
        }
    }
}

