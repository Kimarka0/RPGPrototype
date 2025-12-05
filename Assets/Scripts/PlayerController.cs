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
    private bool isRunPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControls = new PlayerControls();
        playerControls.Controls.Move.started += OnMovementInput;
        playerControls.Controls.Move.performed += OnMovementInput;
        playerControls.Controls.Move.canceled += OnMovementInput;
        playerControls.Controls.Run.started += OnRunInput;
        playerControls.Controls.Run.performed += OnRunInput;
        playerControls.Controls.Run.canceled += OnRunInput;

        playerControls.Enable();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.y = currentMovementInput.y;
    }

    private void OnRunInput(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
    private void MovementHandle()
    {
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

    private void OnEnable()
    {
        playerControls?.Enable();
    }
    
    private void OnDisable()
    {
        playerControls?.Disable();
    }
}
