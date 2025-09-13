using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NextBotMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 7.0f;
    public float runSpeed = 10.0f;
    public float crouchSpeed = 3.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public float airAcceleration = 50.0f;
    public float groundAcceleration = 10.0f;
    public float friction = 6.0f;

    [Header("Bunnyhopping")]
    public bool enableBunnyhopping = true;
    public float bunnyhopTolerance = 0.1f;

    [Header("Input Settings")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    // Public properties for AI access
    public Vector3 CurrentVelocity => currentVelocity;
    public bool IsGrounded => isGrounded;
    public bool IsCrouching => isCrouching;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private float currentSpeed;
    private bool isGrounded;
    private bool isCrouching;
    private float lastJumpTime;

    // Input values that can be set by AI
    private float aiHorizontalInput;
    private float aiVerticalInput;
    private bool aiJumpRequested;
    private bool aiRunRequested;
    private bool aiCrouchRequested;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        ResetAIInput();
    }

    void Update()
    {
        GatherInput();
        ProcessMovement();
        ApplyMovement();
    }

    /// <summary>
    /// Gathers input from either player or AI
    /// </summary>
    private void GatherInput()
    {
        // Default to player input, but AI can override these values
        /*
        aiHorizontalInput = Input.GetAxis(horizontalAxis);
        aiVerticalInput = Input.GetAxis(verticalAxis);
        aiJumpRequested = Input.GetKey(jumpKey);
        aiRunRequested = Input.GetKey(runKey);
        aiCrouchRequested = Input.GetKey(crouchKey);
        */
    }
    
    public void ForceStop()
    {
        currentVelocity = Vector3.zero;
        ResetAIInput();
    }

    /// <summary>
    /// Main movement processing method
    /// </summary>
    private void ProcessMovement()
    {
        UpdateCrouchState();
        UpdateSpeed();
        CheckGrounded();
        
        if (isGrounded)
        {
            ApplyGroundFriction();
            ProcessGroundMovement();
        }
        else
        {
            ProcessAirMovement();
        }
    }

    /// <summary>
    /// Updates crouching state based on input
    /// </summary>
    private void UpdateCrouchState()
    {
        isCrouching = aiCrouchRequested;
    }

    /// <summary>
    /// Updates movement speed based on current state
    /// </summary>
    private void UpdateSpeed()
    {
        currentSpeed = isCrouching ? crouchSpeed : aiRunRequested ? runSpeed : walkSpeed;
    }

    /// <summary>
    /// Checks if the character is grounded
    /// </summary>
    private void CheckGrounded()
    {
        isGrounded = controller.isGrounded;
    }

    /// <summary>
    /// Applies friction when on ground
    /// </summary>
    private void ApplyGroundFriction()
    {
        if (currentVelocity.magnitude > 0)
        {
            float drop = currentVelocity.magnitude * friction * Time.deltaTime;
            currentVelocity *= Mathf.Max(currentVelocity.magnitude - drop, 0) / currentVelocity.magnitude;
        }
    }

    /// <summary>
    /// Processes movement while on ground
    /// </summary>
    private void ProcessGroundMovement()
    {
        Vector3 wishDir = CalculateWishDirection();
        float wishSpeed = CalculateWishSpeed(wishDir);
        Accelerate(wishDir, wishSpeed, groundAcceleration);
        ProcessJump();
    }

    /// <summary>
    /// Processes movement while in air
    /// </summary>
    private void ProcessAirMovement()
    {
        Vector3 wishDir = CalculateWishDirection();
        float wishSpeed = CalculateWishSpeed(wishDir);
        Accelerate(wishDir, wishSpeed, airAcceleration);
        ApplyGravity();
    }

    /// <summary>
    /// Calculates the desired movement direction
    /// </summary>
    private Vector3 CalculateWishDirection()
    {
        Vector3 wishDir = transform.right * aiHorizontalInput + transform.forward * aiVerticalInput;
        return wishDir;
    }

    /// <summary>
    /// Calculates the desired movement speed
    /// </summary>
    private float CalculateWishSpeed(Vector3 wishDir)
    {
        return wishDir.magnitude * currentSpeed;
    }

    /// <summary>
    /// Accelerates the character in the desired direction
    /// </summary>
    private void Accelerate(Vector3 wishDir, float wishSpeed, float acceleration)
    {
        wishDir.Normalize();
        float currentSpeed = Vector3.Dot(currentVelocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0)
            return;

        float accelSpeed = acceleration * Time.deltaTime * wishSpeed;
        accelSpeed = Mathf.Min(accelSpeed, addSpeed);

        currentVelocity += wishDir * accelSpeed;
    }

    /// <summary>
    /// Processes jump input with bunnyhopping support
    /// </summary>
    private void ProcessJump()
    {
        if (aiJumpRequested && CanJump())
        {
            ExecuteJump();
        }
        else
        {
            ApplyGroundStick();
        }
    }

    /// <summary>
    /// Checks if the character can jump
    /// </summary>
    private bool CanJump()
    {
        return enableBunnyhopping ? (Time.time - lastJumpTime > bunnyhopTolerance) : isGrounded;
    }

    /// <summary>
    /// Executes the jump action
    /// </summary>
    private void ExecuteJump()
    {
        currentVelocity.y = jumpForce;
        lastJumpTime = Time.time;
    }

    /// <summary>
    /// Applies a small downward force to ensure grounding
    /// </summary>
    private void ApplyGroundStick()
    {
        currentVelocity.y = -0.5f;
    }

    /// <summary>
    /// Applies gravity to the character
    /// </summary>
    private void ApplyGravity()
    {
        currentVelocity.y -= gravity * Time.deltaTime;
    }

    /// <summary>
    /// Applies the final movement to the character controller
    /// </summary>
    private void ApplyMovement()
    {
        moveDirection = currentVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    // ==================== AI INTERFACE METHODS ====================

    /// <summary>
    /// Sets movement input for AI control
    /// </summary>
    public void SetAIInput(float horizontal, float vertical)
    {
        aiHorizontalInput = Mathf.Clamp(horizontal, -1f, 1f);
        aiVerticalInput = Mathf.Clamp(vertical, -1f, 1f);
    }

    /// <summary>
    /// Sets jump input for AI control
    /// </summary>
    public void SetAIJump(bool jump)
    {
        aiJumpRequested = jump;
    }

    /// <summary>
    /// Sets run input for AI control
    /// </summary>
    public void SetAIRun(bool run)
    {
        aiRunRequested = run;
    }

    /// <summary>
    /// Sets crouch input for AI control
    /// </summary>
    public void SetAICrouch(bool crouch)
    {
        aiCrouchRequested = crouch;
    }

    /// <summary>
    /// Resets all AI input to default values
    /// </summary>
    public void ResetAIInput()
    {
        aiHorizontalInput = 0f;
        aiVerticalInput = 0f;
        aiJumpRequested = false;
        aiRunRequested = false;
        aiCrouchRequested = false;
    }

    /// <summary>
    /// Gets the current movement state for AI decision making
    /// </summary>
    public MovementState GetMovementState()
    {
        return new MovementState
        {
            velocity = currentVelocity,
            isGrounded = isGrounded,
            isCrouching = isCrouching,
            speed = currentSpeed
        };
    }
}

/// <summary>
/// Data structure for movement state information
/// </summary>
public struct MovementState
{
    public Vector3 velocity;
    public bool isGrounded;
    public bool isCrouching;
    public float speed;
}