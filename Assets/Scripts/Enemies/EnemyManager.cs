using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    private int totalEnemies;
    private int deadEnemies;

    [Header("UI")]
    public TextMeshProUGUI enemyCounterText;
    public GameObject winScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Count all enemies at start that have a Prospector_AI (or RagdollHandler if you want both systems unified)
        Prospector_AI[] enemies = FindObjectsOfType<Prospector_AI>();
        totalEnemies = enemies.Length;
        deadEnemies = 0;

        UpdateUI();
    }

    public void RegisterEnemy()
    {
        totalEnemies++;
        UpdateUI();
    }

    public void EnemyDied()
    {
        deadEnemies++;
        UpdateUI();

        if (deadEnemies >= totalEnemies)
            PlayerWins();
    }

    private void UpdateUI()
    {
        int remaining = totalEnemies - deadEnemies;

        // ✅ Update through GameUIManager (preferred central HUD)
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateEnemyCounter(remaining);
        }

        // ✅ Fallback: update direct TMP text if needed
        if (enemyCounterText != null)
        {
            enemyCounterText.text = "Enemies: " + remaining;
        }
    }

    private void PlayerWins()
    {
        Debug.Log("Player Wins!");
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
