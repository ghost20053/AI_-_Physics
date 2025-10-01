using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public int ammoAmount = 30;       // For Ammo pickup
    public float duration = 5f;       // For InfiniteAmmo
    public Sprite icon;               // UI icon
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                //player.ActivatePowerUp(this);

                if (pickupEffect != null)
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }
}
