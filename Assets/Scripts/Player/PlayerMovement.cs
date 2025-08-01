using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movement speed settings
    [Header("Player Speed Settings")]
    public int playerWalkSpeed = 5;
    public int playerSprintSpeed = 8;
    public int playerCrouchSpeed = 2;

    // Gravity and Jump settings
    [Header("Gravity & Jump")]
    public int playerGravity = 20;
    public int playerJumpForce = 8;

    // Sound detection radius for enemies
    [Header("Sound Settings")]
    public float soundRadius = 10f;

    // Components
    [Header("Components")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Camera playerFirstPersonCam;

    // Input system
    private PlayerInput playerInput;
    private InputAction actionMovement;
    private InputAction actionJump;
    private InputAction actionSprint;
    private InputAction actionCrouch;
    private InputAction actionInteract;

    private Vector3 moveDirection;

    private bool playerIsMoving = false;  // Add this at class level
    private float footstepTimer = 0f;     // Add this at class level
    private float footstepInterval = 0.5f; // Adjust this value as needed

    private PlayerRagdoll playerRagdoll;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Link actions from Input System
        InputAction inputAction = playerInput.actions["Move"];
        actionMovement = inputAction;
        actionJump = playerInput.actions["Jump"];
        actionSprint = playerInput.actions["Sprint"];
        actionCrouch = playerInput.actions["Crouch"];
        actionInteract = playerInput.actions["Interact"];

        playerRagdoll = GetComponent<PlayerRagdoll>();

    }

    void Update()
    {
        // Prevent movement if in ragdoll
        if (playerRagdoll != null && playerRagdoll.IsRagdoll)
        {
            return;
        }
        // Determine player movement input and update playerIsMoving
        Vector2 input = actionMovement.ReadValue<Vector2>();
        playerIsMoving = input.magnitude > 0.1f;

        // Handle movement and gravity (your existing methods)
        HandleMovement();
        ApplyGravity();

        // Move the player controller
        playerController.Move(moveDirection * Time.deltaTime);

        // Footstep noise generation when player is moving on the ground
        if (playerController.isGrounded && playerIsMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                MakeNoise(); // Notify enemies about footstep sound
                footstepTimer = 0f;
            }
        }
    }


    // Handles walking, sprinting, crouching and jumping.
    private void HandleMovement()
    {
        Vector2 input = actionMovement.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        // Determine speed based on sprint/crouch input
        float speed = playerWalkSpeed;
        if (actionSprint.IsPressed())
        {
            speed = playerSprintSpeed;
            MakeNoise(); // Sprinting makes noise
        }
        else if (actionCrouch.IsPressed())
        {
            speed = playerCrouchSpeed;
        }

        moveDirection.x = move.x * speed;
        moveDirection.z = move.z * speed;

        // Handle jumping
        if (actionJump.WasPressedThisFrame() && playerController.isGrounded)
        {
            moveDirection.y = playerJumpForce;
            MakeNoise(); // Jumping makes noise
        }
    }

    // Applies gravity to the movement direction.
    private void ApplyGravity()
    {
        if (!playerController.isGrounded)
        {
            moveDirection.y -= playerGravity * Time.deltaTime;
        }
        else if (moveDirection.y < 0)
        {
            moveDirection.y = -1f; // Keeps player grounded
        }
    }

    // Notifies nearby enemies that a sound was made.
    private void MakeNoise()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (Collider hit in hits)
        {
            Prospector_AI enemy = hit.GetComponent<Prospector_AI>();
            if (enemy != null)
            {
                enemy.HearSound(transform.position);
            }
        }
    }

    // Draws the sound radius in the editor.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }

}
