using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // New Input System

public class PlayerGun : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bullet;          // Bullet prefab
    public float shootForce;           // Forward force
    public float upwardForce;          // Extra upward kick

    [Header("Gun Stats")]
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;

    private int bulletsLeft;
    private int bulletsShot;

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce;

    [Header("References")]
    public Camera fpsCam;              // First-person camera
    public Transform attackPoint;      // Where bullets come from
    public GameObject muzzleFlash;     // Optional VFX
    public TextMeshProUGUI ammoDisplay;

    // Input System
    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction reloadAction;

    // State
    private bool shooting;
    private bool readyToShoot;
    private bool reloading;
    private bool allowInvoke = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
        reloadAction = playerInput.actions["Reload"];

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        HandleInput();

        // Update ammo UI
        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    private void HandleInput()
    {
        // Shooting input
        if (allowButtonHold)
        {
            shooting = fireAction.IsPressed();
        }
        else
        {
            shooting = fireAction.WasPressedThisFrame();
        }

        // Reload input
        if (reloadAction.WasPressedThisFrame() && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        // Auto reload if trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            Reload();
        }

        // Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Raycast from center of camera
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);

        // Direction without spread
        Vector3 direction = targetPoint - attackPoint.position;

        // Add spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 directionWithSpread = direction + new Vector3(x, y, 0);

        // Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add force
        Rigidbody bulletRb = currentBullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        bulletRb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        // Muzzle flash
        if (muzzleFlash != null) Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        // Recoil force
        playerRb.AddForce(-fpsCam.transform.forward * recoilForce, ForceMode.Impulse);

        // Reset shot
        if (allowInvoke)
        {
            Invoke(nameof(ResetShot), timeBetweenShooting);
            allowInvoke = false;
        }

        // Burst fire
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShots);
        }
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
}
