using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    // Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    // Stats
    [Range(0f, 1f)] public float bounciness = 0.1f;
    public bool useGravity = true;
    public float shootForce = 50f; // NEW: initial velocity

    // Damage
    public int explosionDamage = 20;
    public float explosionRange = 5f;
    public float explosionForce = 500f;

    // Lifetime
    public int maxCollisions = 1;
    public float maxLifetime = 5f;
    public bool explodeOnTouch = true;

    private int collisions;
    private PhysicsMaterial physicsMat;

    private void Start()
    {
        Setup();

        // ✅ Ensure forward velocity on spawn
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * shootForce;
        }
    }

    private void Update()
    {
        if (collisions > maxCollisions) Explode();

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        // Spawn explosion VFX
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        // Check for enemies in range
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        foreach (Collider hit in enemies)
        {
            // Try RagdollHandler (preferred)
            RagdollHandler ragdoll = hit.GetComponentInParent<RagdollHandler>();
            if (ragdoll != null && ragdoll.ragdollType == RagdollType.Enemy)
            {
                Vector3 forceDir = (hit.transform.position - transform.position).normalized;
                ragdoll.EnterRagdoll(forceDir * explosionForce);
            }

            // Apply explosion force
            Rigidbody rbHit = hit.GetComponent<Rigidbody>();
            if (rbHit != null)
                rbHit.AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        Invoke(nameof(DestroyBullet), 0.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet")) return;

        collisions++;

        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
            Explode();
    }

    private void Setup()
    {
        physicsMat = new PhysicsMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicsMaterialCombine.Minimum,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };

        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
            col.material = physicsMat;

        if (rb != null)
            rb.useGravity = useGravity;
    }

    private void DestroyBullet() => Destroy(gameObject);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
