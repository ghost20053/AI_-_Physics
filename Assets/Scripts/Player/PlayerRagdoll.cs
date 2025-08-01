using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRagdoll : MonoBehaviour
{
    // Ragdoll values
    public float ragdollDuration = 2f;
    private Rigidbody[] ragdollBodies;
    private CharacterController controller;
    private bool isRagdoll = false;

    public bool IsRagdoll => isRagdoll;

    // Stores the player's rotation and Driection before entering ragdoll
    private Quaternion originalRotation;
    private Vector3 originalUpDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        // Disable ragdoll on start
        SetRagdoll(false);
    }

    // Called externally when the player is hit (e.g. by explosion or projectile)
    public void EnterRagdoll(Vector3 force)
    {
        if (isRagdoll)
        {
            return;
        }

        // Save current upright rotation
        originalRotation = transform.rotation;
        originalUpDirection = transform.up;

        // Enable ragdoll mode
        SetRagdoll(true);

        // Apply force to each ragdoll bone to simulate being hit
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }

        // Schedule exit from ragdoll after specified duration
        Invoke(nameof(ExitRagdoll), ragdollDuration);
    }

    // Called after ragdollDuration to exit ragdoll state
    private void ExitRagdoll()
    {
        // Reload Scene
        SceneManager.LoadScene("SampleScene");
    }

    // Toggles between ragdoll and normal mode
    private void SetRagdoll(bool state)
    {
        isRagdoll = state;

        // Enable/disable the CharacterController based on ragdoll state
        if (controller)
        {
            controller.enabled = !state;
        }

        // Enable/disable physics on all ragdoll rigidbodies
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
            rb.detectCollisions = state;
        }
    }
}

