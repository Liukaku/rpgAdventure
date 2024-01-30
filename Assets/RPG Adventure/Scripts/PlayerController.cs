using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace RpgAdventure
{
    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance
        {
            get
            {
                return s_Instance;
            }
        }

        public Quaternion PlayerRotation;
        public float maxForwardSpeed = 0.4f;
        public float gravity = 20.0f;
        public float speedModifier = 1;
        public float rotationSpeed;
        public float speed;

        public MeleeWeapon meleeWeapon;

        private PlayerInput m_playerInput;
        private CharacterController m_ChController;
        private Animator m_Animator;
        private float defaultSpeed;
        private float forwardSpeed;
        private float verticalSpeed;
        private Camera followCamera;

        private static PlayerController s_Instance;

        private readonly int m_hashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int m_hashAttackOne = Animator.StringToHash("AttackOne");

        float mDesiredRotation = 0f;

        public void Awake()
        {
            followCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            m_ChController = GetComponent<CharacterController>();
            m_playerInput = GetComponent<PlayerInput>();
            m_Animator = GetComponent<Animator>();
            defaultSpeed = speed;

            s_Instance = this;
        }

        void FixedUpdate()
        {
            HandleSprintAnimation();
            HandleVerticalMovement();

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

            m_Animator.ResetTrigger(m_hashAttackOne);
            if (m_playerInput.IsAttack)
            {
                m_Animator.SetTrigger(m_hashAttackOne);
                meleeWeapon.BeginAttack();
            }

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

        void HandleVerticalMovement()
        {
            verticalSpeed = -gravity;
            m_ChController.Move(verticalSpeed * Vector3.up * Time.fixedDeltaTime);
        }


    }

}

