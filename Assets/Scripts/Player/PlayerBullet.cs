using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage = 20;
    public float lifeTime = 3f;
    public float hitForce = 200f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we hit an enemy with ragdoll handler
        RagdollHandler ragdoll = collision.collider.GetComponentInParent<RagdollHandler>();
        if (ragdoll != null && ragdoll.ragdollType == RagdollType.Enemy)
        {
            Vector3 hitDir = (collision.transform.position - transform.position).normalized;
            ragdoll.EnterRagdoll(hitDir * hitForce);
        }

        Destroy(gameObject);
    }
}
