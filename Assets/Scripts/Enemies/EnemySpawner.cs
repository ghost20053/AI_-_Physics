using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public bool stopSpawning = false;
    public float spawnTime = 2f;
    public float spawnDelay = 5f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), spawnTime, spawnDelay);
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);

        if (EnemyManager.Instance != null)
            EnemyManager.Instance.RegisterEnemy();

        if (stopSpawning)
            CancelInvoke(nameof(SpawnEnemy));
    }
}
