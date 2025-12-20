using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runMultiplier;

    private Vector2 currentMovement;
    private Vector2 currentMovementInput;
    private PlayerControls playerControls;
    private Animator animator;
    private Transform visualTransform;
    private bool isRunPressed;
    private bool canMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        visualTransform = transform.Find("UnitRoot");
        playerControls = new PlayerControls();
        playerControls.Controls.Move.started += OnMovementInput;
        playerControls.Controls.Move.performed += OnMovementInput;
        playerControls.Controls.Move.canceled += OnMovementInput;
        playerControls.Controls.Run.started += OnRunInput;
        playerControls.Controls.Run.performed += OnRunInput;
        playerControls.Controls.Run.canceled += OnRunInput;

        playerControls.Enable();
    }

    private void Start()
    {
        DialogueNodeManager.instance.OnDialogueStarted.AddListener(DisableMovement);
        DialogueNodeManager.instance.OnDialogueEnded.AddListener(EnableMovement);
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        if(!canMove) return;
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.y = currentMovementInput.y;
        animator.SetBool("isMove", true);
    }

    private void OnRunInput(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
    private void MovementHandle()
    {
        if(!canMove) return;
        float currentSpeed = moveSpeed;
        if(isRunPressed) currentSpeed *= runMultiplier;
        Vector2 movement = currentMovement * currentSpeed;
        rb.linearVelocity = movement;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        MovementHandle();
    }

    private void Update()
    {
        UpdateAnimations();
        SpriteFlip();
    }

    private void UpdateAnimations()
    {
        bool isMoving = currentMovement.sqrMagnitude > 0.01f * 0.01f;

        animator.SetBool("isMove", isMoving);
    }

    private void SpriteFlip()
    {
        if(currentMovement.x < -0.01f)
        {
            visualTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (currentMovement.x > 0.01f)
        {
            visualTransform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void EnableMovement()
    {
        canMove = true;
    }
    
    private void DisableMovement()
    {
        canMove = false;
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }
    
    private void OnDisable()
    {
        playerControls?.Disable();
        DialogueNodeManager.instance.OnDialogueStarted.RemoveListener(DisableMovement);
        DialogueNodeManager.instance.OnDialogueEnded.RemoveListener(EnableMovement);
    }
}
