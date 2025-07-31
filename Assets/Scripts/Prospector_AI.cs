using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prospector_AI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f; // How far the enemy will patrol
    public float patrolWaitTime = 2f; // Wait time before choosing new patrol point

    [Header("Sound Reaction")]
    public float hearingRange = 15f; // Max distance enemy can hear
    public GameObject projectilePrefab; // Prefab of the explosive projectile
    public Transform projectileSpawnPoint; // Where the projectile is thrown from
    public float projectileForce = 20f;

    [Header("References")]
    private NavMeshAgent agent;
    private bool isHearingSound = false;
    private Vector3 soundPosition;

    private float waitTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChooseNewPatrolPoint(); // Start patrol immediately
    }

    void Update()
    {
        // If enemy is reacting to a sound, stop patrolling
        if (isHearingSound)
            return;

        // Patrol logic
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                ChooseNewPatrolPoint();
                waitTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Picks a random point within patrol radius and moves there
    /// </summary>
    void ChooseNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    /// <summary>
    /// Called externally when the enemy hears a sound
    /// </summary>
    /// <param name="soundPos">The world position of the sound</param>
    public void HearSound(Vector3 soundPos)
    {
        if (Vector3.Distance(transform.position, soundPos) <= hearingRange)
        {
            StartCoroutine(ReactToSound(soundPos));
        }
    }

    /// <summary>
    /// Coroutine that stops the enemy, faces the sound, and throws a projectile
    /// </summary>
    IEnumerator ReactToSound(Vector3 soundPos)
    {
        isHearingSound = true;
        agent.isStopped = true;

        // Rotate towards sound
        Vector3 direction = (soundPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        float elapsed = 0f;

        while (elapsed < 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Throw projectile
        ThrowProjectile(soundPos);

        yield return new WaitForSeconds(1.5f); // Wait before returning to patrol

        agent.isStopped = false;
        isHearingSound = false;
    }

    /// <summary>
    /// Spawns and launches a projectile toward the sound position
    /// </summary>
    void ThrowProjectile(Vector3 target)
    {
        if (projectilePrefab && projectileSpawnPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb)
            {
                Vector3 direction = (target - projectileSpawnPoint.position).normalized;
                rb.AddForce(direction * projectileForce, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Choose any color you like
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }

}
