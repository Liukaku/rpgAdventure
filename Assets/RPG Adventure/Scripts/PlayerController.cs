using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace RpgAdventure
{
    public class PlayerController : MonoBehaviour
    {
        public Quaternion PlayerRotation;
        private Camera followCamera;
        public float maxForwardSpeed = 0.4f;
        //private Vector3 m_CameraPosition;

        private PlayerInput m_playerInput;
        private CharacterController m_ChController;
        private Animator m_Animator;
        public float speed;
        private float defaultSpeed;
        public float speedModifier = 1;
        public float rotationSpeed;
        float forwardSpeed;

        private readonly int m_hashForwardSpeed = Animator.StringToHash("ForwardSpeed");

        float mDesiredRotation = 0f;

        public void Awake()
        {
            followCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            m_ChController = GetComponent<CharacterController>();
            m_playerInput = GetComponent<PlayerInput>();
            m_Animator = GetComponent<Animator>();
            defaultSpeed = speed;
        }

        void FixedUpdate()
        {
            HandleSprintAnimation();

            Vector3 movement = m_playerInput.MoveInput;

            Quaternion camRotation = followCamera.transform.rotation;

            

            if (movement.y != 0f && movement.x != 0f)
            {
                movement = Vector3.ClampMagnitude(movement, 1);
            }

            Vector3 rotatedMovement = Quaternion.Euler(0, camRotation.eulerAngles.y, 0) * movement;
            Vector3 targetDirection = camRotation * movement;
            targetDirection.y = 0;

            mDesiredRotation = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;

            float desiredSpeed = movement.normalized.magnitude * maxForwardSpeed;

            forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, Time.fixedDeltaTime);

            float acceleration = forwardSpeed;

            m_Animator.SetFloat(m_hashForwardSpeed, acceleration);

            m_ChController.Move(speed * speedModifier * Time.fixedDeltaTime * targetDirection);

            HandleRotation();

        }

        void HandleRotation()
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, mDesiredRotation, 0);

            if (m_playerInput.IsMoveInput)
            {
                m_ChController.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, 10 * Time.fixedDeltaTime);

            }
        }

        void HandleSprintAnimation()
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 20f;
                maxForwardSpeed = Mathf.MoveTowards(maxForwardSpeed, 0.6f, Time.fixedDeltaTime);

            } else
            {
                speed = defaultSpeed;
                maxForwardSpeed = Mathf.MoveTowards(maxForwardSpeed, 0.4f, Time.fixedDeltaTime);

            }
        }


    }

}

