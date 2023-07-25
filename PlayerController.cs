using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;

    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private CharacterController characterController;
    private Camera playerCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleJump();
        HandleSprint();
    }

    private void HandleMovement()
    {
        float moveSpeed = Input.GetButton("Sprint") ? sprintSpeed : walkSpeed;
        float forwardSpeed = Input.GetAxis("Vertical") * moveSpeed;
        float sideSpeed = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);

        // Convert local speed to global speed
        speed = transform.rotation * speed;

        if (characterController.isGrounded)
        {
            // If the player is grounded, apply normal movement
            characterController.Move(speed * Time.deltaTime);
        }
        else
        {
            // If the player is in the air, apply air control
            Vector3 airControl = new Vector3(sideSpeed * 0.1f, 0f, forwardSpeed * 0.1f);
            airControl = transform.rotation * airControl;
            characterController.Move((speed + airControl) * Time.deltaTime);
        }

        // Apply gravity
        verticalVelocity += Physics.gravity.y * Time.deltaTime;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private bool isJumping = false;

    private void HandleJump()
    {
        // Allow jumping whenever the jump button is pressed
        if (Input.GetButtonDown("Jump"))
        {
            if (characterController.isGrounded || !isJumping)
            {
                isJumping = true;
                verticalVelocity = jumpForce;
            }
        }

        // Apply gravity
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // Apply the final vertical velocity to the CharacterController
        Vector3 moveVector = new Vector3(0f, verticalVelocity, 0f);
        characterController.Move(moveVector * Time.deltaTime);

        // Reset jump state when the player is grounded
        if (characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    private void HandleSprint()
    {
        // You can customize the sprint activation based on your input settings.
        // For example, you might use the Left Shift key as the sprint button.
        // In this case, ensure that the "Sprint" axis is defined in the Input settings.
        bool isSprinting = Input.GetButton("Sprint");

        // Adjust the camera field of view while sprinting for a visual effect (optional).
        if (isSprinting)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 75f, Time.deltaTime * 8f);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60f, Time.deltaTime * 8f);
        }
    }
}
