using UnityEngine;
using UnityEngine.SceneManagement;

public enum RagdollType { Player, Enemy }

public class RagdollHandler : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    public RagdollType ragdollType = RagdollType.Player;
    public float ragdollDuration = 2f;

    private Rigidbody[] ragdollBodies;
    private CharacterController controller;
    private Animator animator;
    private bool isRagdoll = false;

    public bool IsRagdoll => isRagdoll;

    [Header("Audio")]
    public AudioClip ragdollSound;
    public float soundVolume = 1.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        // Disable ragdoll on start
        SetRagdoll(false);
    }

    // Called externally when hit by projectile, bullet, or explosion
    public void EnterRagdoll(Vector3 force)
    {
        if (isRagdoll) return;

        // Play impact sound
        if (ragdollSound)
            AudioSource.PlayClipAtPoint(ragdollSound, transform.position, soundVolume);

        // Enable ragdoll mode
        SetRagdoll(true);

        // Apply force to simulate impact
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }

        // Handle different death behaviors
        if (ragdollType == RagdollType.Player)
        {
            Invoke(nameof(ReloadScene), ragdollDuration);
        }
        else if (ragdollType == RagdollType.Enemy)
        {
            Invoke(nameof(KillEnemy), ragdollDuration);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene("MAZE");
    }

    private void KillEnemy()
    {
        // Tell EnemyManager that one died
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.EnemyDied();

        Destroy(gameObject);
    }

    // Enable/disable ragdoll mode
    private void SetRagdoll(bool state)
    {
        isRagdoll = state;

        if (animator)
            animator.enabled = !state;

        if (controller)
            controller.enabled = !state;

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
            rb.detectCollisions = state;
        }
    }
}
