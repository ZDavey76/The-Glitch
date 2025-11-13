using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions playerInput;
    CharacterController characterController;

    //looking
    public InputActionReference lookAction;
    public float moveSpeed = 2.5f;

    private Vector2 rotStore;
    public float lookSpeed;
    public Camera theCam;

    public float camZoomNormal = 60f;
    public float camZoomOut = 75f;
    public float camZoomSpeed = 5f;

    //move plus run
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    public float runMultiplier;

    public InputActionReference sprintAction;

    //gravity
    float groundedGravity = -0.05f;
    float gravity = -9.8f;

    //jumping
    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 1.0f;
    float maxJumpTime = 0.5f;
    bool isJumping = false;

    [Header("Push Settings")]
    [SerializeField] float pushStrength = 2.5f;
    [SerializeField] float maxPushMass = 200f;
    [SerializeField] bool onlyHorizontalPush = true;
    [SerializeField] float wallDamping = 0.6f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    float CurrentPlanarSpeed()
    {
        Vector3 v = characterController.velocity;
        v.y = 0f;
        return v.magnitude;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.rigidbody;
        if (!rb || rb.isKinematic) return;
        if (rb.mass > maxPushMass) return;

        Vector3 pushDir = hit.moveDirection;

        if (onlyHorizontalPush && pushDir.y > -0.2f) 
            pushDir.y = 0f;

        float speed = CurrentPlanarSpeed();
        float facingWall = 1f - Mathf.Clamp01(Mathf.Abs(hit.normal.y) * 5f);
        float damping = Mathf.Lerp(1f, wallDamping, facingWall);

        Vector3 impulse = pushDir.normalized * pushStrength * speed * damping;
        rb.AddForceAtPosition(impulse, hit.point, ForceMode.Impulse);
    }

    private void Awake()
    {
        playerInput = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        setupJumpVariables();

        playerInput.PlayerControls.Move.started += onMovementInput;
        playerInput.PlayerControls.Move.canceled += onMovementInput;
        playerInput.PlayerControls.Move.performed += onMovementInput;
        playerInput.PlayerControls.Run.started += onRun;
        playerInput.PlayerControls.Run.canceled += onRun;
        playerInput.PlayerControls.Jump.started += onJump;
        playerInput.PlayerControls.Jump.canceled += onJump;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            currentRunMovement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            // Small negative value to keep grounded
            if (currentMovement.y < 0)
                currentMovement.y = groundedGravity;
            if (currentRunMovement.y < 0)
                currentRunMovement.y = groundedGravity;
        }
        else
        {
            currentMovement.y += gravity * Time.deltaTime;
            currentRunMovement.y += gravity * Time.deltaTime;
        }
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void Update()
    {
        // 1) Handle gravity and jumping
        handleGravity();
        handleJump();

        // 2) Build horizontal movement relative to player facing
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 horizontalMove = forward * currentMovementInput.y + right * currentMovementInput.x;

        // 3) Determine speed (walk or sprint)
        bool isSprinting = sprintAction.action.IsPressed() && isMovementPressed;
        float currentSpeed = isSprinting ? (moveSpeed * runMultiplier) : moveSpeed;

        // 4) DON'T normalize yet - we need magnitude for pushing
        // Only normalize if there's input to avoid divide-by-zero
        if (horizontalMove.magnitude > 0.01f)
        {
            horizontalMove = horizontalMove.normalized * currentSpeed;
        }

        // 5) Preserve vertical velocity from gravity/jump
        horizontalMove.y = currentMovement.y;

        // 6) Update currentMovement and currentRunMovement for push physics
        currentMovement = horizontalMove;
        currentRunMovement = horizontalMove;

        // 7) Move the character (this triggers OnControllerColliderHit for pushing)
        characterController.Move(horizontalMove * Time.deltaTime);

        // 8) Handle camera FOV zoom for sprint
        float targetFOV = (isSprinting && isMovementPressed) ? camZoomOut : camZoomNormal;
        theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, targetFOV, Time.deltaTime * camZoomSpeed);

        // 9) Handle camera look
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        lookInput.y = -lookInput.y;
        rotStore = rotStore + (lookInput * lookSpeed * Time.deltaTime);
        rotStore.y = Mathf.Clamp(rotStore.y, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(0f, rotStore.x, 0f);
        theCam.transform.localRotation = Quaternion.Euler(rotStore.y, 0f, 0f);
    }
    private void OnEnable()
    {
        playerInput.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerControls.Disable();
    }
}