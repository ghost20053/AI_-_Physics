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
        // inside Explode()
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        foreach (Collider enemy in enemies)
        {
            Prospector_AI ai = enemy.GetComponent<Prospector_AI>();
            if (ai != null)
            {
                Vector3 forceDir = (enemy.transform.position - transform.position).normalized * explosionForce;
                ai.TakeDamage(explosionDamage, forceDir);
            }
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
