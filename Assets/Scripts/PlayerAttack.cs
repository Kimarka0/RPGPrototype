using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Collider2D attackTrigger;
    private PlayerControls playerControls;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(attackTrigger != null)
        {
            attackTrigger.enabled = false;
        }
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.Controls.Attack.started += OnAttackStarted;
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            enemyHealth.TakeDamage(damage);
            Debug.Log("Attacked");
        }
    }

    public void OnAttackStartedAnimationEvent()
    {
        attackTrigger.enabled = true;
    }

    public void OnAttackEndedAnimationEvent()
    {
        attackTrigger.enabled = false;
    }
    private void OnDisable()
    {
        playerControls.Controls.Attack.started -= OnAttackStarted;
        playerControls.Disable();
    }
}
