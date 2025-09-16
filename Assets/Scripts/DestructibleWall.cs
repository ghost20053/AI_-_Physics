using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject fracturedPrefab;   // Broken version of wall
    public float destroyDelay = 5f;      // How long before chunks are cleaned up

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Called by bullets when hit
    public void TakeDamage(int damage, Vector3 hitPoint, Vector3 hitForce)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Break(hitPoint, hitForce);
        }
    }

    private void Break(Vector3 hitPoint, Vector3 hitForce)
    {
        // Spawn fractured wall
        if (fracturedPrefab != null)
        {
            GameObject fractured = Instantiate(fracturedPrefab, transform.position, transform.rotation);

            // Apply force to chunks
            foreach (Rigidbody rb in fractured.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(hitForce.magnitude * 2f, hitPoint, 5f);
            }

            Destroy(fractured, destroyDelay);
        }

        // Destroy the original wall
        Destroy(gameObject);
    }
}
