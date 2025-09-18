using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class BulletType
{
    public GameObject prefab;       // Bullet prefab to shoot
    public Sprite icon;             // Icon for UI
    public int magazineSize = 30;   // How many per mag
    public int bulletsLeft;         // Bullets currently in mag
    public int reserveAmmo = 90;    // Ammo available for reloading
    public int damage = 20;         // NEW: damage this bullet does
}

public class Player : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Camera playerCamera;
    public Transform shootPoint;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded;
    private float verticalRotation;
    public float mouseSensitivity = 2f;

    [Header("Shooting Settings")]
    public BulletType[] bulletTypes;
    private int currentBulletIndex = 0;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("PowerUps")]
    private bool infiniteAmmoActive = false;
    private bool multiShotActive = false;
    private int extraShots = 1;  // 1 = normal, 2 = double, 3 = triple
    private float powerUpTimer = 0f;

    private void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        // Fill mags at start
        foreach (var bt in bulletTypes)
        {
            bt.bulletsLeft = bt.magazineSize;
        }

        // Setup UI
        GameUIManager.Instance.SetupBulletUI(bulletTypes);
        UpdateUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (PauseMenu.isPaused)
        {
            return; // ⬅️ stop everything when paused
        }

        HandleMovement();
        HandleLook();
        HandleShooting();
        HandleReload();
        HandleWeaponSwitch();
        HandlePowerUps();
    }


    // ---------------- Movement ----------------
    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Keyboard.current.aKey.isPressed ? -1 :
                      Keyboard.current.dKey.isPressed ? 1 : 0;
        float moveZ = Keyboard.current.sKey.isPressed ? -1 :
                      Keyboard.current.wKey.isPressed ? 1 : 0;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (PauseMenu.isPaused) return; // ⬅️ Stop processing look input when paused

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }


    // ---------------- Shooting ----------------
    private void HandleShooting()
    {
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            BulletType bullet = bulletTypes[currentBulletIndex];

            if (infiniteAmmoActive || bullet.bulletsLeft > 0)
            {
                nextFireTime = Time.time + fireRate;

                for (int i = 0; i < extraShots; i++)
                {
                    ShootBullet(bullet);
                }

                if (!infiniteAmmoActive)
                {
                    bullet.bulletsLeft--;
                }

                UpdateUI();
            }
        }
    }

    private void ShootBullet(BulletType bullet)
    {
        if (bullet.prefab == null) return;

        // Spawn the bullet prefab
        GameObject bulletObj = Instantiate(bullet.prefab, shootPoint.position, playerCamera.transform.rotation);

        // Get CustomBullet script
        CustomBullet cb = bulletObj.GetComponent<CustomBullet>();
        if (cb != null && cb.rb == null)
        {
            cb.rb = bulletObj.GetComponent<Rigidbody>();
        }

        // Ensure forward velocity if rb exists
        Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = playerCamera.transform.forward * cb.shootForce;
        }
    }


    private void HandleReload()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            BulletType bullet = bulletTypes[currentBulletIndex];
            int needed = bullet.magazineSize - bullet.bulletsLeft;

            if (needed > 0 && bullet.reserveAmmo > 0)
            {
                int toLoad = Mathf.Min(needed, bullet.reserveAmmo);
                bullet.reserveAmmo -= toLoad;
                bullet.bulletsLeft += toLoad;
                UpdateUI();
            }
        }
    }

    // ---------------- Weapon Switching ----------------
    private void HandleWeaponSwitch()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) currentBulletIndex = 0;
        if (Keyboard.current.digit2Key.wasPressedThisFrame && bulletTypes.Length > 1) currentBulletIndex = 1;
        if (Keyboard.current.digit3Key.wasPressedThisFrame && bulletTypes.Length > 2) currentBulletIndex = 2;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            currentBulletIndex = (currentBulletIndex + (scroll > 0 ? 1 : -1) + bulletTypes.Length) % bulletTypes.Length;
        }

        UpdateUI();
    }

    // ---------------- PowerUps ----------------
    public void ActivatePowerUp(PowerUp powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.Ammo:
                bulletTypes[currentBulletIndex].reserveAmmo += powerUp.ammoAmount;
                UpdateUI();
                break;

            case PowerUpType.InfiniteAmmo:
                infiniteAmmoActive = true;
                powerUpTimer = powerUp.duration;

                PowerUpUIManager.Instance?.AddPowerUp(PowerUpType.InfiniteAmmo, powerUp.icon, powerUp.duration);
                break;
        }
    }

    private void HandlePowerUps()
    {
        if (infiniteAmmoActive)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                infiniteAmmoActive = false;
                PowerUpUIManager.Instance?.RemovePowerUp(PowerUpType.InfiniteAmmo);
            }
        }
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
