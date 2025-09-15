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
        var enemies = Object.FindObjectsByType<RagdollHandler>(FindObjectsSortMode.None);
        foreach (var e in enemies)
            if (e.ragdollType == RagdollType.Enemy) totalEnemies++;

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
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.UpdateEnemyCounter(remaining);
    }

    private void PlayerWins()
    {
        Debug.Log("Player Wins!");
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
