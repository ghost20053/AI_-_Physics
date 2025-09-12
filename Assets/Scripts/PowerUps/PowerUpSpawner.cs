using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    //Spawning Objects
    public GameObject PowerUps;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    private void Start()
    {
        InvokeRepeating("SpawnPowerUps", spawnTime, spawnDelay);
    }

    
    public void SpawnObject()
    {
        Instantiate(PowerUps, transform.position, transform.rotation);
        if(stopSpawning)
        {
            CancelInvoke("SpawnPowerUps");
        }
    }
}
