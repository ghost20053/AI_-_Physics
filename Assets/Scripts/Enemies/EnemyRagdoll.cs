using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    private Rigidbody[] ragdollBodies;
    private Animator animator;

    private void Awake()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        SetRagdoll(false);
    }

    /// <summary>
    /// Enable ragdoll and apply force on death.
    /// </summary>
    public void EnterRagdoll(Vector3 force)
    {
        // Disable animator so physics takes over
        if (animator != null)
            animator.enabled = false;

        SetRagdoll(true);

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Toggle ragdoll physics on/off.
    /// </summary>
    private void SetRagdoll(bool state)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
            rb.detectCollisions = state;
        }
    }
}
