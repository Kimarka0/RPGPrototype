using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private LayerMask enemyLayer;
    private PlayerControls playerControls;
    private Animator animator;
    private float nextAttackTime = 0f;
    private bool canAttack = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        animator = GetComponentInChildren<Animator>();
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Controls.Attack.started += OnAttackStarted;
    }

    private void Start()
    {
        DialogueNodeManager.instance.OnDialogueStarted.AddListener(DisableAttack);
        DialogueNodeManager.instance.OnDialogueEnded.AddListener(EnableAttack);
        
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if(!canAttack) return;
        if(Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate; 
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D [] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponentInParent<EnemyHealth>().TakeDamage(damage);
            Debug.Log("Damaged!");
        }
    }

    private void EnableAttack()
    {
        canAttack = true;
    }

    private void DisableAttack()
    {
        canAttack = false;
    }
    private void OnDisable()
    {
        playerControls.Controls.Attack.started -= OnAttackStarted;
        playerControls.Disable();
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)  
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
