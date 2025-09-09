using UnityEngine;
using UnityEngine.InputSystem;

public class CamFollow : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // Player root transform
    public Transform cameraPivot;  // The position to follow (usually head position)

    [Header("Settings")]
    public float sensitivity = 1.0f;
    public float followSmoothness = 20f; // Smooth following

    private float cameraVerticalRotation;
    private PlayerInput playerInput;
    private InputAction lookAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"];
    }

    private void LateUpdate()
    {
        if (PauseMenu.isPaused) return;

        // 1) Position: follow player smoothly
        if (cameraPivot != null)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                cameraPivot.position,
                Time.deltaTime * followSmoothness
            );
        }

        // 2) Rotation: look around
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float inputX = lookInput.x * sensitivity;
        float inputY = lookInput.y * sensitivity;

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        player.Rotate(Vector3.up * inputX);
    }
}
