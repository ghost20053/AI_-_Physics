using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("Wall Settings")]
    public GameObject fracturedPrefab;   // assign fractured wall prefab

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }


    // Called when the wall takes damage (from bullets, explosions, etc.)

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
        if (fracturedPrefab != null)
        {
            GameObject fractured = Instantiate(fracturedPrefab, transform.position, transform.rotation);
            fractured.transform.localScale = transform.localScale;

            // 🔹 Force all children into "Debris" layer
            foreach (Transform child in fractured.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Debris");
            }

            // Apply physics forces
            Rigidbody[] chunks = fractured.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in chunks)
            {
                rb.AddExplosionForce(hitForce.magnitude, hitPoint, 5f);
            }
        }

        Destroy(gameObject);
    }
}
