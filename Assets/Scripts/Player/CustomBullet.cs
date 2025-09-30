using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    // Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Header("Movement")]
    public float shootForce = 20f; // how fast the bullet is fired
    public bool useGravity = true;

    // Stats
    [Range(0f, 1f)] public float bounciness = 0.1f;

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

        // Apply initial velocity forward from spawn direction
        if (rb != null)
        {
            rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
    }


    private void Update()
    {
        if (collisions > maxCollisions)
        {
            Explode();
        }

        maxLifetime -= Time.deltaTime;

        if (maxLifetime <= 0)
        {
            Explode();
        }
    }


    private void Explode()
    {
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (Collider hit in hits)
        {
            // Enemy AI damage
            Prospector_AI ai = hit.GetComponent<Prospector_AI>();
            if (ai != null)
            {
                Vector3 forceDir = (hit.transform.position - transform.position).normalized * explosionForce;
                ai.TakeDamage(explosionDamage, forceDir);
            }

            // 🔹 Destructible Wall ONLY if this is a player bullet
            if (CompareTag("PlayerBullet"))
            {
                DestructibleWall wall = hit.GetComponentInParent<DestructibleWall>();
                if (wall != null)
                {
                    Vector3 forceDir = (hit.transform.position - transform.position).normalized * explosionForce;
                    wall.TakeDamage(explosionDamage, transform.position, forceDir);
                }
            }

            // Physics pushback for rigidbodies
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        Invoke(nameof(DestroyBullet), 0.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet")) return;

        collisions++;

        // 🔹 Handle direct wall hit ONLY if this is a player bullet
        if (CompareTag("PlayerBullet"))
        {
            DestructibleWall wall = collision.collider.GetComponentInParent<DestructibleWall>();
            if (wall != null)
            {
                Vector3 forceDir = collision.contacts[0].normal * explosionForce;
                wall.TakeDamage(explosionDamage, collision.contacts[0].point, forceDir);
                Explode();
                return;
            }
        }

        // 🔹 Handle direct enemy hit
        if (collision.collider.CompareTag("Enemy") && explodeOnTouch)
        {
            Explode();
        }
    }

    private void Setup()
    {
        physicsMat = new PhysicsMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicsMaterialCombine.Minimum,
            bounceCombine = PhysicsMaterialCombine.Maximum
        };

        GetComponent<SphereCollider>().material = physicsMat;
        rb.useGravity = useGravity;
    }

    private void DestroyBullet() => Destroy(gameObject);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
