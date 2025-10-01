using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject fracturedPrefab;   // assign your broken-wall prefab
    public float destroyDelay = 5f;      // how long to keep pieces alive

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }


    // Called by bullets or explosion logic when this wall should take damage.

    public void TakeDamage(int damage, Vector3 hitPoint, Vector3 hitForce)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            BreakWall(hitPoint, hitForce);
        }
    }

    private void BreakWall(Vector3 hitPoint, Vector3 hitForce)
    {
        // Spawn fractured version
        if (fracturedPrefab != null)
        {
            GameObject fractured = Instantiate(fracturedPrefab, transform.position, transform.rotation);

            // Apply explosion / hit force to chunks
            Rigidbody[] chunks = fractured.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in chunks)
            {
                rb.AddExplosionForce(hitForce.magnitude * 2f, hitPoint, 5f);
            }

            // Clean up fractured pieces after a delay
            //Destroy(fractured, destroyDelay);
        }

        // Destroy original wall
        Destroy(gameObject);
    }
}
