using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy AI that patrols, detects player, chases, becomes enraged, 
/// and accepts an item to calm down.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;             // Reference to the NavMeshAgent component
    public Transform player;               // Reference to the player
    public LayerMask playerLayer;          // Layer mask to detect player only
    public Animator animator;              // Animator for playing animations
    public AudioSource audioSource;        // AudioSource for sound effects

    [Header("Patrolling")]
    public float patrolRadius = 10f;       // Radius around current position to patrol
    public float patrolWaitTime = 2f;      // Time to wait at patrol point
    private bool isWaiting;

    [Header("Detection")]
    public float sightRange = 12f;         // Range within which player can be seen
    public float fieldOfView = 120f;       // FOV angle in degrees

    [Header("Chasing & Enrage")]
    public float normalSpeed = 3.5f;       // Default patrol and chase speed
    public float enragedSpeed = 6f;        // Speed when enraged
    public float timeToEnrage = 5f;        // Time after which enemy gets enraged
    private float enrageTimer = 0f;

    [Header("State Tracking")]
    private bool isPatrolling = true;
    private bool isChasingPlayer = false;
    private bool isEnraged = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        DetectPlayer();

        if (isChasingPlayer)
        {
            ChasePlayer();

            // Start enrage countdown
            if (!isEnraged)
            {
                enrageTimer += Time.deltaTime;
                if (enrageTimer >= timeToEnrage)
                {
                    BecomeEnraged();
                }
            }
        }
    }

    /// <summary>
    /// Continuously patrol random points.
    /// </summary>
    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (isPatrolling && !agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
            {
                Vector3 patrolPoint = GetRandomPatrolPoint();
                agent.SetDestination(patrolPoint);
                animator?.SetTrigger("Walk");

                isWaiting = true;
                yield return new WaitForSeconds(patrolWaitTime);
                isWaiting = false;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Finds a random point within the patrol radius.
    /// </summary>
    Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, 1))
        {
            return hit.position;
        }
        return transform.position;
    }

    /// <summary>
    /// Detects the player within FOV and line of sight.
    /// </summary>
    void DetectPlayer()
    {
        if (player == null)
            return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < sightRange)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle < fieldOfView / 2f)
            {
                // Check for line of sight
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit hit, sightRange, playerLayer))
                {
                    if (hit.transform == player)
                    {
                        if (!isChasingPlayer)
                        {
                            Debug.Log("Player detected. Start chasing.");
                            isPatrolling = false;
                            isChasingPlayer = true;
                            animator?.SetTrigger("Run");
                            PlaySound("Alert");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Continuously chase the player when detected.
    /// </summary>
    void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    /// <summary>
    /// Player can calm the enemy by giving an item.
    /// Call this externally when player uses the item.
    /// </summary>
    public void GiveItemToEnemy()
    {
        if (isChasingPlayer)
        {
            Debug.Log("Enemy received the calming item. Returning to patrol.");
            CalmDown();
        }
    }

    /// <summary>
    /// The enemy enters an enraged state: faster speed & aggressive behavior.
    /// </summary>
    private void BecomeEnraged()
    {
        isEnraged = true;
        agent.speed = enragedSpeed;
        animator?.SetTrigger("Enraged");
        PlaySound("Enrage");
        Debug.Log("Enemy is enraged! Speed increased.");
    }

    /// <summary>
    /// Resets the enemy to normal patrol state.
    /// </summary>
    private void CalmDown()
    {
        isPatrolling = true;
        isChasingPlayer = false;
        isEnraged = false;
        enrageTimer = 0f;
        agent.speed = normalSpeed;
        animator?.SetTrigger("Calm");
        PlaySound("Calm");
    }

    /// <summary>
    /// Play specific sound effects. Placeholder for sound logic.
    /// </summary>
    /// <param name="soundName">Sound name to play</param>
    private void PlaySound(string soundName)
    {
        // Placeholder: implement your own sound logic here
        Debug.Log("Playing sound: " + soundName);
        // e.g., audioSource.PlayOneShot(clip);
    }
}
