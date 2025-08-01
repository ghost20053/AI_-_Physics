using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public GameObject explosionEffect;

    void OnCollisionEnter(Collision collision)
    {
        // Show effect
        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Apply force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Apply ragdoll knockback if it's the player
            PlayerRagdoll player = nearby.GetComponentInParent<PlayerRagdoll>();
            if (player != null)
            {
                Vector3 forceDir = (nearby.transform.position - transform.position).normalized;
                player.EnterRagdoll(forceDir * explosionForce);
            }
        }

        Destroy(gameObject);
    }
}
