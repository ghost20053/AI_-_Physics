using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private int totalEnemies;
    private int deadEnemies;

    [Header("UI")]
    public TextMeshProUGUI enemyCounterText;
    public GameObject winScreen; // assign in Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // ✅ New Unity API (no warning)
        Prospector_AI[] enemies = Object.FindObjectsByType<Prospector_AI>(FindObjectsSortMode.None);
        totalEnemies = enemies.Length;
        deadEnemies = 0;
        UpdateUI();
    }

    public void EnemyDied()
    {
        deadEnemies++;
        UpdateUI();

        if (deadEnemies >= totalEnemies)
        {
            PlayerWins();
        }
    }

    private void UpdateUI()
    {
        if (enemyCounterText != null)
        {
            int remaining = totalEnemies - deadEnemies;
            enemyCounterText.text = $"Enemies Left: {remaining}";
        }
    }

    private void PlayerWins()
    {
        Debug.Log("Player Wins!");

        if (winScreen != null)
            winScreen.SetActive(true);

        Time.timeScale = 0f; // freeze
        // OR load scene: SceneManager.LoadScene("WinScene");
    }
}