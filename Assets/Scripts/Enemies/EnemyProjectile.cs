using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 10f;        // could be used later for HP system
    public float lifetime = 5f;       // destroy after this many seconds
    public float hitForce = 20f;      // knockback force applied to player

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Get PlayerRagdoll and trigger ragdoll
            PlayerRagdoll ragdoll = collision.collider.GetComponentInParent<PlayerRagdoll>();
            if (ragdoll != null)
            {
                Vector3 forceDir = (collision.contacts[0].normal * -1f) * hitForce;
                ragdoll.EnterRagdoll(forceDir);
            }

            Destroy(gameObject);
        }
        else if (!collision.collider.CompareTag("Enemy")) // avoid killing own AI
        {
            Destroy(gameObject);
        }
    }
}
