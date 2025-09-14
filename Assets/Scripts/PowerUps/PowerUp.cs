using UnityEngine;

public enum PowerUpType { MoreAmmo, InfiniteAmmo, DoubleShot, TripleShot }

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public int ammoAmount = 30;       // for MoreAmmo
    public float duration = 10f;      // for timed powerups
    public Sprite icon;               // icon for UI

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.ActivatePowerUp(this);
            Destroy(gameObject);
        }
    }
}
