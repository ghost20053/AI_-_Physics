using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[System.Serializable]
public class BulletType
{
    public GameObject prefab;       // Bullet prefab
    public Sprite icon;             // Icon for UI
    public int magazineSize = 30;   // Bullets per magazine
    public int bulletsLeft;         // Bullets currently in mag
    public int reserveAmmo = 90;    // Extra ammo for reloading
}

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;       // Player movement speed
    public float jumpForce = 5f;       // Jump height
    public float gravity = -9.81f;     // Gravity applied to the player
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Camera")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;  // Mouse look sensitivity
    private float verticalRotation;

    [Header("Weapons & Bullets")]
    public BulletType[] bulletTypes;     // Assign multiple bullet types in Inspector
    private int currentBulletIndex = 0;  // Which bullet type is active
    public Transform attackPoint;        // Where bullets spawn
    public float shootForce = 20f;       // How fast bullets are fired

    [Header("UI")]
    public TextMeshProUGUI ammoDisplay;      // Shows ammo count
    public TextMeshProUGUI bulletTypeText;   // Shows active bullet type

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        foreach (var bt in bulletTypes)
        {
            bt.bulletsLeft = bt.magazineSize;
        }

        // Setup bullet UI at game start
        GameUIManager.Instance.SetupBulletUI(bulletTypes);
        UpdateUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleShooting();
        HandleReload();
        HandleBulletSwitching();
    }

    // ---------------- Player Movement ----------------
    private void HandleMovement()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keeps player "stuck" to the ground
        }

        // WASD input
        float moveX = Keyboard.current.aKey.isPressed ? -1 :
                      Keyboard.current.dKey.isPressed ? 1 : 0;
        float moveZ = Keyboard.current.sKey.isPressed ? -1 :
                      Keyboard.current.wKey.isPressed ? 1 : 0;

        // Move relative to player facing
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ---------------- Camera Look ----------------
    private void HandleLook()
    {
        if (PauseMenu.isPaused) 
        { 
            return; 
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Rotate camera vertically
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);
    }

    // ---------------- Shooting ----------------
    private void HandleShooting()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletTypes.Length == 0)
        { 
            return; 
        }

        BulletType current = bulletTypes[currentBulletIndex];
        if (current.bulletsLeft <= 0)
        {
            return; // Out of ammo
        }

        // Spawn bullet
        GameObject bullet = Instantiate(current.prefab, attackPoint.position, attackPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(attackPoint.forward * shootForce, ForceMode.Impulse);
        }

        current.bulletsLeft--;
        UpdateUI();
    }

    // ---------------- Reloading ----------------
    private void HandleReload()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Reload();
        }
    }

    private void Reload()
    {
        if (bulletTypes.Length == 0)
        {
            return;
        }

        BulletType current = bulletTypes[currentBulletIndex];
        int needed = current.magazineSize - current.bulletsLeft;

        if (needed > 0 && current.reserveAmmo > 0)
        {
            int loadAmount = Mathf.Min(needed, current.reserveAmmo);
            current.bulletsLeft += loadAmount;
            current.reserveAmmo -= loadAmount;
        }

        UpdateUI();
    }

    // ---------------- Bullet Switching ----------------
    private void HandleBulletSwitching()
    {
        // Number keys
        if (Keyboard.current.digit1Key.wasPressedThisFrame && bulletTypes.Length > 0)
        {
            currentBulletIndex = 0;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && bulletTypes.Length > 1)
        {
            currentBulletIndex = 1;
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && bulletTypes.Length > 2)
        {
            currentBulletIndex = 2;
        }
        /*
        if (Keyboard.current.digit4Key.wasPressedThisFrame && bulletTypes.Length > 3)
        {
            currentBulletIndex = 3;
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame && bulletTypes.Length > 4)
        {
            currentBulletIndex = 4;
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame && bulletTypes.Length > 5)
        {
            currentBulletIndex = 5;
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame && bulletTypes.Length > 6)
        {
            currentBulletIndex = 8;
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame && bulletTypes.Length > 7)
        {
            currentBulletIndex = 7;
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame && bulletTypes.Length > 8)
        {
            currentBulletIndex = 8;
        }
        */
        
        // Scroll wheel
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll > 0f)
        {
            currentBulletIndex++;
            if (currentBulletIndex >= bulletTypes.Length)
            {
                currentBulletIndex = 0;
            }
        }
        else if (scroll < 0f)
        {
            currentBulletIndex--;
            if (currentBulletIndex < 0)
            {
                currentBulletIndex = bulletTypes.Length - 1;
            }
        }

        UpdateUI();
    }

    // ---------------- UI ----------------
    private void UpdateUI()
    {
        for (int i = 0; i < bulletTypes.Length; i++)
        {
            bool isActive = (i == currentBulletIndex);
            GameUIManager.Instance.UpdateBulletUI(bulletTypes[i], i, isActive);
        }
    }
}
