using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float explosionRadius = 5f;
    public float explosionForce = 20f;
    public float lifetime = 5f;

    [Header("Damage Settings")]
    public int damage = 20; // if you add HP later

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Detect everything in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            // ✅ Affect Player
            if (hit.CompareTag("Player"))
            {
                PlayerRagdoll ragdoll = hit.GetComponentInParent<PlayerRagdoll>();
                if (ragdoll != null)
                {
                    Vector3 forceDir = (hit.transform.position - transform.position).normalized * explosionForce;
                    ragdoll.EnterRagdoll(forceDir);
                }
            }

            // ✅ Add force to rigidbodies (props, etc.)
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce * 50f, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }
}
