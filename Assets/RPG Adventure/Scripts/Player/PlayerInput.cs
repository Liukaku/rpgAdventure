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
        private bool m_E;
        private Collider[] m_InteractTarget;

        public float distanceToInteract = 5.0f;

        public Collider[] interactTarget
        {
            get { return m_InteractTarget; }
        }

        public Vector3 MoveInput
        {
            get { return m_Movement; }
        }

        public bool IsMoveInput
        {
            get { return !Mathf.Approximately(MoveInput.magnitude, 0); }
        }

        public bool IsAttack
        {
            get { return m_Attack; }
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
            //if (Input.GetKeyDown(KeyCode.E) && m_E == false)
            //{
            //    m_E = true;
            //}
            //if (Input.GetKeyUp(KeyCode.E) && m_E == true)
            //{
            //    m_E = false;
            //}
            //if (m_E == true)
            //{
            //    Debug.Log("E");
            //    Cursor.visible = true;
            //    Cursor.lockState = CursorLockMode.None;
            //} else
            //{
            //    Debug.Log("no E");
            //    Cursor.visible = false;
            //    Cursor.lockState = CursorLockMode.Locked;
            //}

            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleEInteract();
            }

            if (isLeftMouseClick)
            {
                HandleLeftMouseButtonDown();
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

            m_Attack = false;
        }
    }
}

