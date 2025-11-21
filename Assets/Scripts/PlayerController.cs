using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions playerInput;
    CharacterController characterController;

    //move plus run
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    float runMultiplier = 7.5f;

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
    [SerializeField] float maxPushMass = 200f;    // ignore very heavy bodies
    [SerializeField] bool onlyHorizontalPush = true; // avoid launching things upward
    [SerializeField] float wallDamping = 0.6f;    // reduce impulse when running straight into a wall

    float CurrentPlanarSpeed()
    {
        // Use the same vector you pass to Move() so walking vs. sprinting feels different
        Vector3 v = isRunPressed ? currentRunMovement : currentMovement;
        v.y = 0f;
        return v.magnitude;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.rigidbody;
        if (!rb || rb.isKinematic) return;        // nothing to push
        if (rb.mass > maxPushMass) return;        // too heavy

        // Direction we moved into the object from Unity's collision info
        Vector3 pushDir = hit.moveDirection;

        // Keep push horizontal so jumps/ledges don't yeet props
        if (onlyHorizontalPush && pushDir.y > -0.2f) pushDir.y = 0f;

        // Scale by our current speed for natural feel (walk < run)
        float speed = CurrentPlanarSpeed();

        // If we're basically hitting a wall (normal ~ horizontal), damp the shove a bit
        float facingWall = 1f - Mathf.Clamp01(Mathf.Abs(hit.normal.y) * 5f); // 1 when vertical surface
        float damping = Mathf.Lerp(1f, wallDamping, facingWall);

        // Build the impulse
        Vector3 impulse = pushDir.normalized * pushStrength * speed * damping;

        // Apply at the contact point for nicer torque on tall objects
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
        currentMovement.x = currentMovementInput.x * 5f;
        currentMovement.z = currentMovementInput.y * 5f;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
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
            currentMovement.y = groundedGravity;
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

    // Update is called once per frame
    void Update()
    {
        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
        handleGravity();
        handleJump();
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

