using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenu;
    public static bool isPaused = false;

    private bool canPause = false; // prevents auto-pausing at scene start
    private float delay = 0.2f;    // small delay before pause allowed
    private float timer = 0f;

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        isPaused = false;
        canPause = false;
        timer = 0f;
    }

    private void Update()
    {
        // Startup delay so ESC at scene load doesn’t instantly trigger pause
        if (!canPause)
        {
            timer += Time.unscaledDeltaTime; // works even when timeScale = 0
            if (timer >= delay) canPause = true;
            return;
        }

        // ESC input check
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Hide UI
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.ToggleUI(false);

        if (PowerUpUIManager.Instance != null)
            PowerUpUIManager.Instance.ToggleUI(false); // NEW
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Show UI
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.ToggleUI(true);

        if (PowerUpUIManager.Instance != null)
            PowerUpUIManager.Instance.ToggleUI(true); // NEW
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
