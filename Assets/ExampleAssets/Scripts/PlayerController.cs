using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class PlayerController : MonoBehaviour
    {
        public Quaternion PlayerRotation;
        private Camera followCamera;
        //private Vector3 m_CameraPosition;

        private CharacterController m_ChController;
        public float speed;
        public float speedModifier = 1;
        public float rotationSpeed;


        float mDesiredRotation = 0f;

        public void Awake()
        {
            followCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            //m_CameraPosition = followCamera.transform.position - transform.position;
            m_ChController = GetComponent<CharacterController>();
        }

        void FixedUpdate()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Quaternion camRotation = followCamera.transform.rotation;

            Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);


            if (horizontalInput != 0f && verticalInput != 0f)
            {
                movement = Vector3.ClampMagnitude(movement, 1);
            }

            Vector3 rotatedMovement = Quaternion.Euler(0, camRotation.eulerAngles.y, 0) * movement;
            Vector3 targetDirection = camRotation * movement;
            targetDirection.y = 0;

            mDesiredRotation = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;

            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, mDesiredRotation, 0);



            m_ChController.Move(targetDirection * speed * speedModifier * Time.fixedDeltaTime);

            if (horizontalInput + verticalInput != 0)
            {
                m_ChController.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, 10 * Time.fixedDeltaTime);
            }
            //m_rb.MoveRotation(Quaternion.Euler(0, camRotation.eulerAngles.y, 0));

        }


    }

}

