using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Junkie_AI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f; // How far the enemy will patrol
    public float patrolWaitTime = 2f; // Wait time before choosing new patrol point
    private NavMeshAgent j_agent;


    private float waitTimer = 0f;

    void Start()
    {
        j_agent = GetComponent<NavMeshAgent>();
        ChooseNewPatrolPoint(); // Start patrol immediately
    }

    void Update()
    {
        // Patrol logic
        if (!j_agent.pathPending && j_agent.remainingDistance <= j_agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                ChooseNewPatrolPoint();
                waitTimer = 0f;
            }
        }
    }

    // Picks a random point within patrol radius and moves there

    void ChooseNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            j_agent.SetDestination(hit.position);
        }
    }
}
