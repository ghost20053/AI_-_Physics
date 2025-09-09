using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class CamFollow : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // The player body to rotate left/right

    [Header("Settings")]
    public float sensitivity = 1.0f;        // Mouse sensitivity

    private float cameraVerticalRotation;   // Rotation on X axis (up/down)
    private PlayerInput playerInput;        // Input system reference
    private InputAction lookAction;         // "Look" action from input map

    private void Awake()
    {
        // Get PlayerInput from the same GameObject or parent
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"];
    }

    private void Update()
    {
        // Do not rotate camera if paused
        if (PauseMenu.isPaused) return;

        // Read Look input (mouse delta)
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float inputX = lookInput.x * sensitivity;
        float inputY = lookInput.y * sensitivity;

        // Vertical camera rotation (clamped so we don’t flip upside down)
        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        // Apply vertical rotation to camera
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotate player horizontally
        player.Rotate(Vector3.up * inputX);
    }
}
