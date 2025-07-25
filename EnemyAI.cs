using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{

    // These are the events that will be triggered when the enemy dies - So the spawner knows when to spawn a new enemy
    public delegate void EnemyDeathHandler(GameObject enemy);
    public event EnemyDeathHandler OnEnemyDeath;

    
    public NavMeshAgent agent; // Reference to the enemy's Navmesh agent
    public Transform player; // Reference to the player
    public LayerMask whatIsGround, whatIsPlayer;
    
    public float health;
    public GameObject projectile;


    // State variables

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    
    private Animator animator; // Reference to the Animator component


    // Upon start, we get the player transform and the NavMeshAgent component
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Reference to the Animator component
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (health <= 10)
        {
            AvoidPlayer();
        }
        else if (!playerInSightRange && !playerInAttackRange)
        {
            Patrolling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer(); // My own code
        }
    }

    private void Patrolling()
    {
        if (!walkPointSet)
        {
        SearchWalkPoint();
        }

    if (walkPointSet)
    {
        agent.SetDestination(walkPoint);
    }

    Vector3 distanceToWalkPoint = transform.position - walkPoint;

    // Walkpoint reached
    if (distanceToWalkPoint.magnitude < 1f)
    {
        walkPointSet = false;
    }

    // Set walking animation
    animator.SetBool("PlayerInAttackRange", false); 
    animator.SetTrigger("Walking");
}

    private void SearchWalkPoint() // Youtube 
    {   
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
        
            walkPointSet = true;
            agent.SetDestination(walkPoint);
        }
    
    }

    private void ChasePlayer()
    {
        // Set the destination to the player's position
        agent.SetDestination(player.position);
        
        // Face the player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Set walking animation
        animator.SetTrigger("Walking");
    }

   private void AttackPlayer()
   {
        // Stop moving
        agent.SetDestination(transform.position);

        // Keep looping in the sitting animation
        animator.SetBool("PlayerInAttackRange", true);

        // Face the player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            alreadyAttacked = true;

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    // My own code
    private void AvoidPlayer()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 newPos = transform.position + direction * 10f; // Move away from the player
        agent.SetDestination(newPos);
        animator.SetTrigger("Walking");
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Invoke(nameof(Die), 0.5f); // Delay before calling Die
        }
    }

    public void Die()
    {
        OnEnemyDeath?.Invoke(gameObject);
        // Handle enemy death (e.g., play animation, destroy object)
        Destroy(gameObject);
    }
}