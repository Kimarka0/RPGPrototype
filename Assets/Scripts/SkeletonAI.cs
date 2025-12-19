using Unity.VisualScripting;
using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private Transform attackPoint;

    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    private EnemyState currentState;
    private Animator animator;
    private int currentPatrolIndex;
    private float nextAttackTime = 0f;
    private Vector2 lastKnownPlayerPosition;

    private enum EnemyState{Patrol, Chase, Attack}


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        if(player == null) Debug.Log("Player us null!");
        if(patrolPoints.Length == 0) Debug.Log("Patrol points not find!");
    }

    private void Update()
    {
        if(player == null) return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
        }
    }

    private void HandlePatrolState()
    {
        Patrol();

        if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            lastKnownPlayerPosition = player.position;
            Debug.Log($"{gameObject.name}: Player was find!");
        }
    }

    private void HandleChaseState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            Debug.Log($"{gameObject.name}: In the attack range!");
            return;
        }

        if (CanSeePlayer())
        {
            lastKnownPlayerPosition = player.position;

            MoveTowards(player.position, chaseSpeed);
        }
        else
        {
            MoveTowards(lastKnownPlayerPosition, chaseSpeed);

            if(Vector2.Distance(transform.position, lastKnownPlayerPosition) < 0.5f)
            {
                currentState = EnemyState.Patrol;
                Debug.Log("Going to patrol");
            }
        }
    }

    private void HandleAttackState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            Debug.Log("Player leaved the attack range, chasing!");
            return;
        }

        if(Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Patrol()
    {
        if(patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        MoveTowards(targetPoint.position, patrolSpeed);

        if(Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        animator.SetBool("isMove", true);
    }

    private bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer > detectionRange)
        {
            return false;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayer | obstacleLayer);

        if (showDebugRays)
        {
            Color rayColor = (hit.collider != null && hit.collider.CompareTag("Player")) ? Color.green : Color.red;
            Debug.DrawRay(transform.position, directionToPlayer * detectionRange, rayColor);
        }

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void PerformAttack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log($"{gameObject.name}: damaged!");
            enemy.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if(patrolPoints != null && patrolPoints.Length > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if(patrolPoints[i] == null) continue;

                Vector3 currentPoint = patrolPoints[i].position;
                Vector3 nextPoint = patrolPoints[(i + 1) % patrolPoints.Length].position;

                Gizmos.DrawLine(currentPoint,nextPoint);
                Gizmos.DrawWireSphere(currentPoint, 0.3f);
            }
        }
    }


}

