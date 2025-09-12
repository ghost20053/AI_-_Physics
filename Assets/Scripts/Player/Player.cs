using System.Collections;
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
        if (controller == null) controller = GetComponent<CharacterController>();

        // Fill mags at start
        foreach (var bt in bulletTypes)
            bt.bulletsLeft = bt.magazineSize;

        // Setup UI
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
        HandleWeaponSwitch();
        HandlePowerUps();
    }

    // ---------------- Movement ----------------
    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Keyboard.current.aKey.isPressed ? -1 :
                      Keyboard.current.dKey.isPressed ? 1 : 0;
        float moveZ = Keyboard.current.sKey.isPressed ? -1 :
                      Keyboard.current.wKey.isPressed ? 1 : 0;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
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
                    ShootBullet(bullet);

                if (!infiniteAmmoActive)
                    bullet.bulletsLeft--;

                UpdateUI();
            }
        }
    }

    private void ShootBullet(BulletType bullet)
    {
        if (bullet.prefab == null) return;
        Instantiate(bullet.prefab, shootPoint.position, playerCamera.transform.rotation);
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
            case PowerUpType.MoreAmmo:
                bulletTypes[currentBulletIndex].reserveAmmo += powerUp.ammoAmount;
                UpdateUI();
                return;
        }

        if (powerUp.type == PowerUpType.InfiniteAmmo)
            infiniteAmmoActive = true;
        else if (powerUp.type == PowerUpType.DoubleShot)
        {
            multiShotActive = true;
            extraShots = 2;
        }
        else if (powerUp.type == PowerUpType.TripleShot)
        {
            multiShotActive = true;
            extraShots = 3;
        }

        powerUpTimer = powerUp.duration;

        // Show on UI
        PowerUpUIManager.Instance.AddPowerUp(powerUp.type, powerUp.icon, powerUp.duration);
    }

    private void HandlePowerUps()
    {
        if (powerUpTimer > 0)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                if (infiniteAmmoActive) PowerUpUIManager.Instance.RemovePowerUp(PowerUpType.InfiniteAmmo);
                if (multiShotActive)
                {
                    PowerUpUIManager.Instance.RemovePowerUp(extraShots == 2 ? PowerUpType.DoubleShot : PowerUpType.TripleShot);
                }

                infiniteAmmoActive = false;
                multiShotActive = false;
                extraShots = 1;
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
