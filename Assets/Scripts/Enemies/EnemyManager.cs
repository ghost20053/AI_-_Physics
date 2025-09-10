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
        // Count enemies at start
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
            PlayerWins();
    }

    private void UpdateUI()
    {
        int remaining = totalEnemies - deadEnemies;
        GameUIManager.Instance.UpdateEnemyCounter(remaining);
    }

    private void PlayerWins()
    {
        Debug.Log("Player Wins!");

        if (winScreen != null)
            winScreen.SetActive(true);

        Time.timeScale = 0f;
    }
}
