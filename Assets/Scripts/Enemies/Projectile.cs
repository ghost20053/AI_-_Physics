using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public GameObject explosionEffect;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.attachedRigidbody;
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            RagdollHandler ragdoll = nearby.GetComponentInParent<RagdollHandler>();
            if (ragdoll != null && ragdoll.ragdollType == RagdollType.Player)
            {
                Vector3 forceDir = (nearby.transform.position - transform.position).normalized;
                ragdoll.EnterRagdoll(forceDir * explosionForce);
            }
        }

        Destroy(gameObject);
    }
}
