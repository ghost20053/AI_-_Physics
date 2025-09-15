using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerGun : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bullet;
    public float shootForce;
    public float upwardForce;

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
    private Camera fpsCam; // Fetched from PlayerMovement to ensure sync
    public Transform attackPoint;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction reloadAction;

    private bool shooting;
    private bool readyToShoot;
    private bool reloading;
    private bool allowInvoke = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
        reloadAction = playerInput.actions["Reload"];

        // Find PlayerMovement and grab its camera reference
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm != null)
        {
            fpsCam = pm.GetPlayerCamera();
        }

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        HandleInput();

        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    private void HandleInput()
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

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);

        Vector3 direction = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 directionWithSpread = direction + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        Rigidbody bulletRb = currentBullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        bulletRb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        playerRb.AddForce(-fpsCam.transform.forward * recoilForce, ForceMode.Impulse);

        if (allowInvoke)
        {
            Invoke(nameof(ResetShot), timeBetweenShooting);
            allowInvoke = false;
        }

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
