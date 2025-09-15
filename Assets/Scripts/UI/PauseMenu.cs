using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenu;
    public static bool isPaused = false;

    private void Start()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        isPaused = false;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ToggleUI(false);
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ToggleUI(true);
        }
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
