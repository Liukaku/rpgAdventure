using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static RpgAdventure.IMessageReceiver;

namespace RpgAdventure
{
    public class PlayerController : MonoBehaviour, IAttackListener, IMessageReceiver
    {

        public static PlayerController Instance
        {
            get
            {
                return s_Instance;
            }
        }

        public bool isPlayerRespawning { get { return isRespawning; } }

        public Quaternion PlayerRotation;
        public float maxForwardSpeed = 0.4f;
        public float gravity = 20.0f;
        public float speedModifier = 1;
        public float rotationSpeed;
        public float speed;
        public Transform attackHand;
        private bool isRespawning;
        public RandomAudioPlayer walkSound;
        public RandomAudioPlayer runSound;

        public MeleeWeapon meleeWeapon;

        private PlayerInput m_playerInput;
        private CharacterController m_ChController;
        private Animator m_Animator;
        private float defaultSpeed;
        private float forwardSpeed;
        private float verticalSpeed;
        private Camera followCamera;
        private HudManager m_hudManager;
        private Damageable m_damageable;

        private AnimatorStateInfo m_CurrentStateInfo;
        private AnimatorStateInfo m_NextStateInfo;
        private bool m_AnimtorIsTransitioning;

        private static PlayerController s_Instance;

        private readonly int m_hashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int m_hashAttackOne = Animator.StringToHash("AttackOne");
        private readonly int m_hashAttackTwo = Animator.StringToHash("AttackTwo");
        private readonly int m_hashDeath = Animator.StringToHash("Death");
        private readonly int m_hashFootFall = Animator.StringToHash("FootFall");

        // Animator tag hash
        private readonly int m_hashBlockInput = Animator.StringToHash("BlockInput");

        float mDesiredRotation = 0f;

        public void Awake()
        {
            followCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            m_ChController = GetComponent<CharacterController>();
            m_playerInput = GetComponent<PlayerInput>();
            m_Animator = GetComponent<Animator>();
            m_damageable = GetComponent<Damageable>();
            m_hudManager = FindObjectOfType<HudManager>();
            defaultSpeed = speed;

            s_Instance = this;
            m_hudManager.SetMaxHealth(m_damageable.maxHealth);
        }

        void FixedUpdate()
        {
            CacheAnimationState();
            UpdateInputBlocking();

            if (isRespawning)
            {
                return;
            }

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
            }
            if (m_playerInput.IsAttackTwo)
            {
                Debug.Log("isAttackTwo");
                m_Animator.SetBool(m_hashAttackTwo, true);
                StartCoroutine(ResetCombo());
            }
            //PlayWalkAudio();
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
                speed = 15f;
                maxForwardSpeed = Mathf.MoveTowards(maxForwardSpeed, 0.6f, Time.fixedDeltaTime);

            } else
            {
                speed = defaultSpeed;
                maxForwardSpeed = Mathf.MoveTowards(maxForwardSpeed, 0.4f, Time.fixedDeltaTime);

            }
        }

        // these are fired via animation events in the animator view
        public void MeleeAttackStart()
        {
            Debug.Log("melee attack start");
            if (meleeWeapon != null)
            {
                meleeWeapon.BeginAttack();
            }
        }

        public void MeleeAttackEnd()
        {
            if (meleeWeapon != null)
            {
                meleeWeapon.EndAttack();
            }
        }

        void HandleVerticalMovement()
        {
            verticalSpeed = -gravity;
            m_ChController.Move(verticalSpeed * Vector3.up * Time.fixedDeltaTime);
        }

        public void UseItemFrom(InventorySlot inventorySlot)
        {
            if (meleeWeapon != null)
            {
                if (inventorySlot.itemPrefab.name == meleeWeapon.name)
                {
                    return;
                } else
                {
                    Destroy(meleeWeapon.gameObject);
                }
                
            }
            meleeWeapon = Instantiate(inventorySlot.itemPrefab, transform).GetComponent<MeleeWeapon>();
            meleeWeapon.GetComponent<FixedUpdateFollow>().SetToFollow(attackHand);
            meleeWeapon.name = inventorySlot.itemPrefab.name;
            meleeWeapon.SetOwner(gameObject);
        }

        public void StartRespawn()
        {
            isRespawning = true;
            //transform.position = Vector3.zero;
            transform.position = new Vector3(170.4f, 6.23f, 48.4f);
            m_damageable.SetInitialHealth();
            m_hudManager.SetHealth(m_damageable.maxHealth);
        }

        public void FinishRespawn()
        {
            isRespawning = false;
        }

        public void PlayWalkAudio()
        {
            float footFallCurve = m_Animator.GetFloat(m_hashFootFall);
            //if (footFallCurve > 0.01f && !walkSound.isPlaying && walkSound.canPlay)
            if (true)
            {
                walkSound.isPlaying = true;
                walkSound.PlayRandomClip(0.1f);
                walkSound.canPlay = false;
            } else if (walkSound.isPlaying)
            {
                walkSound.isPlaying = false;
            } else if (footFallCurve < 0.01f && !walkSound.canPlay)
            {
                walkSound.canPlay = true;
            }
        }

        public void PlaySprintAudio()
        {
            runSound.PlayRandomClip(0.1f);
        }

        public void CacheAnimationState()
        {
            m_CurrentStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_NextStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
            m_AnimtorIsTransitioning = m_Animator.IsInTransition(0);


        }

        private void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_hashBlockInput && !m_AnimtorIsTransitioning;
            inputBlocked = inputBlocked | m_NextStateInfo.tagHash == m_hashBlockInput;
            m_playerInput.isPlayerInputBlocked = inputBlocked;
        }

        public void OnReceiveMessage(MessageType type, Damageable sender, Damageable.DamageMessage message)
        {
            if(type == MessageType.DAMAGED)
            {

                Debug.Log("damaged");
                m_hudManager.SetHealth(sender.CurrentHitpoints);
            }
            if(type == MessageType.DEAD)
            {
                m_Animator.SetTrigger(m_hashDeath);
                m_hudManager.SetHealth(0);
            }
        }

        private IEnumerator ResetCombo()
        {
            yield return new WaitForSeconds(0.3f);
            m_Animator.SetBool(m_hashAttackTwo, false);
        }
    }

}

