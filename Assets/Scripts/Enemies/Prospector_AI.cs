using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Prospector_AI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    private float waitTimer = 0f;

    [Header("Sound Reaction")]
    public float hearingRange = 15f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileForce = 20f;
    public float fireRate = 0.5f;
    private float nextFire = 0.0f;

    [Header("Audio")]
    public AudioClip projectileSound;
    public float soundVolume = 1.0f;

    [Header("Ragdoll & Death")]
    private Animator animator;
    private Rigidbody[] ragdollBodies;
    private NavMeshAgent agent;
    private bool isDead = false;

    private bool isHearingSound = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        SetRagdoll(false); // start alive
        ChooseNewPatrolPoint();
    }

    void Update()
    {
        if (isDead) return; // stop AI if dead

        if (isHearingSound) return;

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

    // ---------------- Patrol ----------------
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

    // ---------------- Sound Reaction ----------------
    public void HearSound(Vector3 soundPos)
    {
        if (isDead) return;

        if (Vector3.Distance(transform.position, soundPos) <= hearingRange)
        {
            StartCoroutine(ReactToSound(soundPos));
            ThrowProjectile(soundPos);
        }
    }

    IEnumerator ReactToSound(Vector3 soundPos)
    {
        isHearingSound = true;
        agent.isStopped = true;

        // Face sound
        Vector3 direction = (soundPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        float elapsed = 0f;

        while (elapsed < 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        agent.isStopped = false;
        isHearingSound = false;
    }

    void ThrowProjectile(Vector3 target)
    {
        if (projectilePrefab && projectileSpawnPoint)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                AudioSource.PlayClipAtPoint(projectileSound, transform.position, soundVolume);

                if (rb)
                {
                    Vector3 direction = (target - projectileSpawnPoint.position).normalized;
                    rb.AddForce(direction * projectileForce, ForceMode.VelocityChange);
                }
            }
        }
    }

    // ---------------- Death / Ragdoll ----------------
    public void EnableRagdoll()
    {
        if (isDead) return;

        isDead = true;

        // Stop AI
        if (animator != null) animator.enabled = false;
        if (agent != null) agent.enabled = false;

        // Enable physics on body parts
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Notify EnemyManager
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyDied();
        }
    }

    private void SetRagdoll(bool active)
    {
        if (animator != null) animator.enabled = !active;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !active;
            rb.useGravity = active;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}
