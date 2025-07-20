using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyLama
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour, IMovementController
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        public float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;

        [Header("References")]
        [SerializeField] private Camera playerCamera;

        public bool CanMove { get; set; } = true;
        public GameObject GameObject => gameObject;

        public CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0;

        public static FPSController Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            Debug.Log($"[FPSController] Baþlatýldý. CanMove: {CanMove}");
        }

        void Update()
        {
           
            if (!CanMove)
            {

                if (!characterController.isGrounded)
                {
                    moveDirection.y -= gravity * Time.deltaTime;
                    characterController.Move(new Vector3(0, moveDirection.y, 0) * Time.deltaTime);
                }
                return;
            }

            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            bool isGrounded = characterController.isGrounded;

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical");
            float curSpeedY = (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal");

            float movementDirectionY = moveDirection.y;
            moveDirection = (transform.forward * curSpeedX) + (transform.right * curSpeedY);

            if (Input.GetButton("Jump") && isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);
        }

        private void HandleRotation()
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}