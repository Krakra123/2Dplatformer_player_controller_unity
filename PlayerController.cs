using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public class Inputs
    {
        public float movementDirection;
        public bool jump;
        public bool jumpHold;

        public Inputs(float movementInput, bool jumpInput, bool jumpHoldInput)
        {
            movementDirection = movementInput;
            jump = jumpInput;
            jumpHold = jumpHoldInput;
        }
    }

    Rigidbody2D thisRigidbody;

    [Header("Input")]
    public string movementInputAxis;
    public string jumpInputButton;
    [HideInInspector] private Inputs inputs = new Inputs(0f, false, false);

    [Header("Ground Check")]
    public LayerMask groundLayerMask;
    [HideInInspector] public bool onGround;
    public Vector2 groundCheckCenterOffset;
    public float groundCheckRadius;

    [Header("Running")]
    public float runningMaxSpeed;
    [Range(.1f, 10f)] public float runningSpeedAcceleration;
    [Range(.1f, 10f)] public float runningSpeedDeceleration;
    [Range(.1f, 10f)] public float runningSpeedTurn;

    [Header("Jumping")]
    public float jumpHeight;
    public float gravityScale;
    private bool canJump;
    [Range(1f, 5f)] public float onAirDownGravityScale;
    [Range(.1f, 10f)] public float onAirSpeedAcceleration;
    [Range(.1f, 10f)] public float onAirSpeedControl;
    [Range(0f, 5f)] public float variableJumpCutOff;
    private bool variableJumpDragDownStart = false;

    [Header("Other")]
    [Range(0f, .25f)] public float coyoteTime;
    private float coyoteTimeCounter;
    [Range(0f, .25f)] public float jumpBuffer;
    private float bufferTimeCounter;
    private bool earlyJumpInput = false;
    public float terminalVelocity = -10000f;
    public int maxJumpsNumber = 1;
    private int jumpsNumber;

    private bool isJumping;

    private float desiredtVelocityX;

    private float movementAcceleration;
    private float movementDeceleration;
    private float movementTurnSpeed;

    private void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputs.movementDirection = (int)Input.GetAxisRaw(movementInputAxis);
        inputs.jump = Input.GetButtonDown(jumpInputButton);
        inputs.jumpHold = Input.GetButton(jumpInputButton);

        desiredtVelocityX = inputs.movementDirection * runningMaxSpeed;

        OnGroundCheck();

        JumpingCalculation();
    }

    private void FixedUpdate()
    {
        GravityCalculation();

        Movement();

        Vector2 _velocity = thisRigidbody.velocity;
        _velocity.y = Mathf.Clamp(_velocity.y, terminalVelocity, Mathf.Infinity);
        thisRigidbody.velocity = _velocity;
    }

    private void OnGroundCheck()
    {
        float offset = Physics2D.defaultContactOffset;
        float maxRayLength = 2 * groundCheckRadius - offset;
        float rayDirection = Mathf.Sign(thisRigidbody.velocity.x);
        Vector3 upperRayOrigin = transform.position + (Vector3)groundCheckCenterOffset - Vector3.right * rayDirection * groundCheckRadius - new Vector3(-offset * rayDirection, offset);
        Vector3 groundCheckRayOrigin = transform.position + (Vector3)groundCheckCenterOffset - Vector3.right * rayDirection * groundCheckRadius - new Vector3(-offset * rayDirection, 2f * offset);

        float rayLength = maxRayLength;
        RaycastHit2D upperRayCast = Physics2D.Raycast(upperRayOrigin, Vector3.right * rayDirection, maxRayLength, groundLayerMask);
        if (upperRayCast.collider != null) rayLength = Vector3.Distance(upperRayCast.point, upperRayOrigin);

        RaycastHit2D groundCheckRayCast = Physics2D.Raycast(groundCheckRayOrigin, Vector3.right * rayDirection, rayLength - offset, groundLayerMask);

        onGround = groundCheckRayCast.collider != null;
    }

    private void Movement()
    {
        Vector2 _velocity = thisRigidbody.velocity;

        float velocityChangeSpeed = MovementVelocityChangeSpeed();
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredtVelocityX, velocityChangeSpeed);

        thisRigidbody.velocity = _velocity;
    }

    private void JumpingCalculation()
    {
        // Coyote Time
        if (onGround) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        if (!isJumping) canJump = (coyoteTimeCounter >= 0);

        // Jump Buffer
        if (!earlyJumpInput) bufferTimeCounter = jumpBuffer;
        else bufferTimeCounter -= Time.deltaTime;

        if (isJumping && onGround && thisRigidbody.velocity.y <= 0f)
        {
            isJumping = false;
            jumpsNumber = maxJumpsNumber;
        }

        if (inputs.jump)
        {
            if (jumpsNumber <= 0) canJump = false;

            if (canJump)
            {
                Jump(jumpHeight);
            }
            else
            {
                earlyJumpInput = true;
            }
        }

        if (isJumping)
        {
            if (!inputs.jumpHold && thisRigidbody.velocity.y > 0f && !variableJumpDragDownStart) variableJumpDragDownStart = true;
            if (variableJumpDragDownStart)
            {
                DragDown(variableJumpCutOff * gravityScale * Physics2D.gravity.y);
            }
        }
        else
        {
            variableJumpDragDownStart = false;
        }

        if (canJump && earlyJumpInput)
        {
            if (bufferTimeCounter >= 0)
            {
                Jump(jumpHeight);
            }

            earlyJumpInput = false;
        }
    }

    private void DragDown(float _gravity)
    {
        Vector2 _velocity = thisRigidbody.velocity;
        _velocity.y += _gravity * Time.deltaTime;
        thisRigidbody.velocity = _velocity;
    }

    public void Jump(float jumpForce)
    {
        isJumping = true;
        jumpsNumber--;

        variableJumpDragDownStart = false;

        Vector2 _velocity = thisRigidbody.velocity;
        _velocity.y = jumpForce;
        thisRigidbody.velocity = _velocity;
    }

    private void GravityCalculation()
    {
        if (!onGround && thisRigidbody.velocity.y < 0)
        {
            thisRigidbody.gravityScale = gravityScale * onAirDownGravityScale;

            return;
        }

        thisRigidbody.gravityScale = gravityScale;
    }

    private float MovementVelocityChangeSpeed()
    {
        movementAcceleration = (onGround ? runningSpeedAcceleration : onAirSpeedAcceleration) * runningMaxSpeed;
        movementDeceleration = (onGround ? runningSpeedDeceleration : onAirSpeedControl) * runningMaxSpeed;
        movementTurnSpeed = ((onGround ? runningSpeedTurn : onAirSpeedControl) * 2) * runningMaxSpeed;

        float result = 0f;
        if (inputs.movementDirection != 0)
        {
            if (Mathf.Sign(inputs.movementDirection) != Mathf.Sign(thisRigidbody.velocity.x))
            {
                result = movementTurnSpeed * Time.deltaTime;
            }
            else
            {
                result = movementAcceleration * Time.deltaTime;
            }
        }
        else
        {
            result = movementDeceleration * Time.deltaTime;
        }

        return result;
    }
}