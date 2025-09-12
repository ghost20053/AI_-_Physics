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

    private void Start() => Setup();

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
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        foreach (Collider enemy in enemies)
        {
            // Deal damage if AI
            Prospector_AI ai = enemy.GetComponent<Prospector_AI>();
            if (ai != null)
            {
                ai.TakeDamage(explosionDamage);
            }
            // Apply physics force
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.AddExplosionForce(explosionForce, transform.position, explosionRange);

            }
        }
        Invoke(nameof(DestroyBullet), 0.05f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            return;
        }

        collisions++;

        if (collision.collider.CompareTag("WhatIsEnemy") && explodeOnTouch)
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
