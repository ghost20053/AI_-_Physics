using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Speed Settings")]
    public int playerWalkSpeed = 5;
    public int playerSprintSpeed = 8;
    public int playerCrouchSpeed = 2;

    [Header("Gravity & Jump")]
    public int playerGravity = 20;
    public int playerJumpForce = 8;

    [Header("Sound Settings")]
    public float soundRadius = 10f;

    [Header("Components")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Camera playerFirstPersonCam; // Shared with CamFollow + PlayerGun

    private PlayerInput playerInput;
    private InputAction actionMovement;
    private InputAction actionJump;
    private InputAction actionSprint;
    private InputAction actionCrouch;
    private InputAction actionInteract;

    private Vector3 moveDirection;
    private bool playerIsMoving = false;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.5f;

    private PlayerRagdoll playerRagdoll;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Bind actions
        actionMovement = playerInput.actions["Move"];
        actionJump = playerInput.actions["Jump"];
        actionSprint = playerInput.actions["Sprint"];
        actionCrouch = playerInput.actions["Crouch"];
        actionInteract = playerInput.actions["Interact"];

        playerRagdoll = GetComponent<PlayerRagdoll>();
    }

    void Update()
    {
        // Disable controls if ragdoll is active
        if (playerRagdoll != null && playerRagdoll.IsRagdoll)
        {
            return;
        }

        // Check if movement input exists
        Vector2 input = actionMovement.ReadValue<Vector2>();
        playerIsMoving = input.magnitude > 0.1f;

        HandleMovement();
        ApplyGravity();

        // Apply movement
        playerController.Move(moveDirection * Time.deltaTime);

        // Generate noise if moving
        if (playerController.isGrounded && playerIsMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                MakeNoise();
                footstepTimer = 0f;
            }
        }
    }

    private void HandleMovement()
    {
        Vector2 input = actionMovement.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        float speed = playerWalkSpeed;
        if (actionSprint.IsPressed())
        {
            speed = playerSprintSpeed;
            MakeNoise(); // Sprinting makes extra noise
        }
        else if (actionCrouch.IsPressed())
        {
            speed = playerCrouchSpeed;
        }

        moveDirection.x = move.x * speed;
        moveDirection.z = move.z * speed;

        if (actionJump.WasPressedThisFrame() && playerController.isGrounded)
        {
            moveDirection.y = playerJumpForce;
            MakeNoise(); // Jumping makes noise
        }
    }

    private void ApplyGravity()
    {
        if (!playerController.isGrounded)
        {
            moveDirection.y -= playerGravity * Time.deltaTime;
        }
        else if (moveDirection.y < 0)
        {
            moveDirection.y = -1f;
        }
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }

    // 👇 Gives other scripts access to the FPS camera
    public Camera GetPlayerCamera() => playerFirstPersonCam;
}
