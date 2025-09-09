using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    // Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    // Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

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
    }

    private void Update()
    {
        // Destroy after too many collisions
        if (collisions > maxCollisions)
        {
            Explode();
        }

        // Destroy after lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Spawn explosion VFX
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        // Damage and knockback
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        foreach (Collider enemy in enemies)
        {
            // Check if in RagDoll State
            Prospector_AI ai = enemy.GetComponent<Prospector_AI>();
            if (ai != null)
            {
                ai.EnableRagdoll(); // Kill enemy immediately
            }

            // Apply knockback if they have Rigidbody
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
            // Trigger ragdoll if enemy has one
            Prospector_AI ragdoll = enemy.GetComponent<Prospector_AI>();
            if (ragdoll != null)
            {
                ragdoll.EnableRagdoll();
            }
        }

        // Small delay before destroying bullet
        Invoke(nameof(DestroyBullet), 0.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore other bullets
        if (collision.collider.CompareTag("Bullet"))
        { 
            return; 
        }

        collisions++;

        // Explode immediately on enemy hit
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
        }
    }

    private void Setup()
    {
        // Physics material
        physicsMat = new PhysicsMaterial();
        physicsMat.bounciness = bounciness;
        physicsMat.frictionCombine = PhysicsMaterialCombine.Minimum;
        physicsMat.bounceCombine = PhysicsMaterialCombine.Maximum;

        // Assign to collider
        GetComponent<SphereCollider>().material = physicsMat;

        // Gravity
        rb.useGravity = useGravity;
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
