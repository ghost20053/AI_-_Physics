using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;

    [Header("Jump & Gravity")]
    public float gravity = 20f;
    public float jumpForce = 8f;

    [Header("Camera Settings")]
    public Camera playerCamera;       // Assign your fps cam directly
    public float mouseSensitivity = 10f; // Base sensitivity
    private float verticalRotation = 0f; // Pitch (up/down)

    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform attackPoint;
    public float shootForce = 20f;
    public float upwardForce = 0f;
    public float spread = 0.05f;
    public float timeBetweenShooting = 0.2f;
    public float timeBetweenShots = 0.05f;
    public float reloadTime = 2f;
    public int magazineSize = 30;
    public int bulletsPerTap = 1;
    public bool allowButtonHold = true;

    [Header("Gun Recoil")]
    public Rigidbody playerRb;
    public float recoilForce = 2f;

    [Header("Gun Visuals")]
    public GameObject muzzleFlashPrefab;
    public TextMeshProUGUI ammoDisplay;

    [Header("Sound Settings")]
    public float soundRadius = 10f;

    // --- Private movement vars ---
    private CharacterController controller;
    private Vector3 moveDirection;
    private bool isMoving;
    private float footstepTimer;
    private float footstepInterval = 0.5f;

    // --- Private gun vars ---
    private int bulletsLeft;
    private int bulletsShot;
    private bool shooting;
    private bool readyToShoot;
    private bool reloading;
    private bool allowInvoke = true;

    // --- Input System ---
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction lookAction;
    private InputAction fireAction;
    private InputAction reloadAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Input Actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        lookAction = playerInput.actions["Look"];
        fireAction = playerInput.actions["Fire"];
        reloadAction = playerInput.actions["Reload"];

        // Gun setup
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
        controller.Move(moveDirection * Time.deltaTime);

        HandleLook();
        HandleGunInput();

        // Ammo UI
        if (ammoDisplay != null)
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);

        // Footstep noise
        if (controller.isGrounded && isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                MakeNoise();
                footstepTimer = 0f;
            }
        }
    }

    // ---------------- Movement ----------------
    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        float speed = walkSpeed;
        if (sprintAction.IsPressed()) { speed = sprintSpeed; MakeNoise(); }
        else if (crouchAction.IsPressed()) { speed = crouchSpeed; }

        moveDirection.x = move.x * speed;
        moveDirection.z = move.z * speed;

        if (jumpAction.WasPressedThisFrame() && controller.isGrounded)
        {
            moveDirection.y = jumpForce;
            MakeNoise();
        }

        isMoving = input.magnitude > 0.1f;
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        else if (moveDirection.y < 0)
            moveDirection.y = -1f;
    }

    // ---------------- Camera Look ----------------
    private void HandleLook()
    {
        if (PauseMenu.isPaused) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // Scale mouse input (0.1f factor makes it closer to old system feel)
        float mouseX = lookInput.x * mouseSensitivity * 0.1f;
        float mouseY = lookInput.y * mouseSensitivity * 0.1f;

        // Pitch (vertical rotation on camera itself)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Yaw (horizontal rotation on player root)
        transform.Rotate(Vector3.up * mouseX);
    }

    // ---------------- Gun Handling ----------------
    private void HandleGunInput()
    {
        shooting = allowButtonHold ? fireAction.IsPressed() : fireAction.WasPressedThisFrame();

        if (reloadAction.WasPressedThisFrame() && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Raycast from screen center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);

        // Spread
        Vector3 direction = targetPoint - attackPoint.position;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 directionWithSpread = direction + new Vector3(x, y, 0);

        // Bullet
        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;
        Rigidbody bulletRb = currentBullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        bulletRb.AddForce(playerCamera.transform.up * upwardForce, ForceMode.Impulse);

        // Muzzle flash
        if (muzzleFlashPrefab != null)
            Instantiate(muzzleFlashPrefab, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        // Recoil
        if (playerRb != null)
            playerRb.AddForce(-playerCamera.transform.forward * recoilForce, ForceMode.Impulse);

        if (allowInvoke)
        {
            Invoke(nameof(ResetShot), timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke(nameof(Shoot), timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    // ---------------- Noise for AI ----------------
    private void MakeNoise()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (Collider hit in hits)
        {
            Prospector_AI enemy = hit.GetComponent<Prospector_AI>();
            if (enemy != null)
                enemy.HearSound(transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}
