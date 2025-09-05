using UnityEngine;
using TMPro;

public class PlayerGun : MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //Bullet Force
    public float shootForce, upwardForce;

    //Gun Stats
    public float TimeBetweenShooting, Spread, ReloadTime, TimeBetweenShots;
    public int MagazineSize, BulletsPerTap;
    public bool AllowButtonHold;

    int BulletsLeft, BulletShot;

    //Recoil
    public Rigidbody playerRb;
    public float recoilForce;

    //Bools
    bool shooting, ReadyToShoot, Reloading;

    //Reference
    public Camera fpsCam;
    public Transform AttackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //Bug Fixing
    public bool AllowInvoke = true;

    public void Awake()
    {
        //Make sure magazine is full
        BulletsLeft = MagazineSize;
        ReadyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        //Set ammo display, if it exists
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(BulletsLeft / BulletsPerTap + " / " + MagazineSize / BulletsPerTap);
        }
    }

    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (AllowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        //Reloading
        if (Input.GetKeyDown(KeyCode.R) && BulletsLeft < MagazineSize && !Reloading)
        {
            Reload();
        }
        //Reload automatically when trying to shoot without ammo
        if (ReadyToShoot && shooting && !Reloading && BulletsLeft > 0)
        {
            Reload();
        }

        //Shooting
        if (ReadyToShoot && shooting && !Reloading && BulletsLeft > 0)
        {
            //Set bullets shot to 0
            BulletShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        ReadyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your screen
        RaycastHit hit;

        //Check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        //Calculate direction from AttackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - AttackPoint.position;

        //Calculate Spread
        float x = Random.Range(-Spread, Spread);
        float y = Random.Range(-Spread, Spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, AttackPoint.position, Quaternion.identity); //Store instatiated bullet/projectile
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, AttackPoint.position, Quaternion.identity);
        }

        BulletsLeft--;
        BulletShot++;

        //Invoke ResetShot function (if not already invoked)
        if (AllowInvoke)
        {
            Invoke("ResetShot", TimeBetweenShooting);
            AllowInvoke = false;

            //Add recoil to player
            playerRb.AddForce(directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //If more than one bulletsPerTap make sure to repeat shoot function
        if (BulletShot < BulletsPerTap && BulletsLeft > 0)
        {
            Invoke("Shoot", TimeBetweenShots);
        }

    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        ReadyToShoot = true;
        AllowInvoke = true;
    }

    private void Reload()
    {
        //Check if Gun is needs to relode
        Reloading = true;
        Invoke("ReloadFinished", ReloadTime);
    }

    private void ReloadFinished()
    {
        //Check if Gun is reloded
        BulletsLeft = MagazineSize;
        Reloading = false;
    }
}
