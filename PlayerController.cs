using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D thisRigidbody;
    BoxCollider2D thisCollider;

    [Header("Input")]
    public string movementInputAxis;
    public string jumpInputButton;
    private int inputDirectionX;
    private bool inputJump;
    private bool inputJumpHold;

    [Header("Ground")]
    public LayerMask groundLayerMask;
    [HideInInspector] public bool onGround;

    [Header("Running")]
    public float runningMaxSpeed;
    [Range(.1f, 50f)] public float runningSpeedAcceleration;
    [Range(.1f, 50f)] public float runningSpeedDeceleration;
    [Range(.1f, 50f)] public float runningSpeedTurn;

    [Header("Jumping")]
    public float jumpHeight;
    public float gravityScale;
    private bool canJump;
    [Range(1f, 10f)] public float onAirDownGravityScale;
    [Range(.1f, 50f)] public float onAirSpeedAcceleration;
    [Range(.1f, 50f)] public float onAirSpeedControl;
    [Range(0f, 5f)] public float variableJumpCutOff;
    private bool variableJumpDragDownStart = false;

    [Header("Other")]
    [Range(0f, .3f)] public float coyoteTime;
    private float coyoteTimeCounter;
    [Range(0f, .3f)] public float jumpBuffer;
    private float bufferTimeCounter;
    private bool earlyJumpInput = false;
    public float terminalVelocity = -10000f;

    private bool isJumping;

    private float desiredtVelocityX;

    private float movementAcceleration;
    private float movementDeceleration;
    private float movementTurnSpeed;

    private void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        inputDirectionX = (int)Input.GetAxisRaw(movementInputAxis);
        inputJump = Input.GetButtonDown(jumpInputButton);
        inputJumpHold = Input.GetButton(jumpInputButton);

        OnGroundCheck();

        JumpingCalculation();

        desiredtVelocityX = inputDirectionX * runningMaxSpeed;
    }

    private void FixedUpdate()
    {
        Movement();

        GravityCalculation();

        Vector2 _velocity = thisRigidbody.velocity;
        _velocity.y = Mathf.Clamp(_velocity.y, terminalVelocity, Mathf.Infinity);
        thisRigidbody.velocity = _velocity;
    }

    private void OnGroundCheck()
    {
        float groundCheckThick = Physics2D.defaultContactOffset * 2f;
        float groundCheckOffset = .1f;

        Vector2[] groundCheckCorner = new Vector2[2];
        groundCheckCorner[0] = transform.position + (Vector3)thisCollider.offset + new Vector3(-thisCollider.size.x / 2 + groundCheckOffset, -thisCollider.size.y / 2 - groundCheckOffset);
        groundCheckCorner[1] = transform.position + (Vector3)thisCollider.offset + new Vector3(+thisCollider.size.x / 2 - groundCheckOffset, -thisCollider.size.y / 2 - groundCheckOffset - groundCheckThick);

        if (Physics2D.OverlapArea(groundCheckCorner[0], groundCheckCorner[1], groundLayerMask))
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }
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

        canJump = (coyoteTimeCounter >= 0);

        // Jump Buffer
        if (!earlyJumpInput) bufferTimeCounter = jumpBuffer;
        else bufferTimeCounter -= Time.deltaTime;

        if (isJumping && onGround && thisRigidbody.velocity.y <= 0f) isJumping = false;

        if (inputJump)
        {
            if (canJump)
            {
                if (!isJumping) Jump(jumpHeight);
            }
            else
            {
                earlyJumpInput = true;
            }
        }

        if (isJumping)
        {
            if (!inputJumpHold && thisRigidbody.velocity.y > 0 && !variableJumpDragDownStart) variableJumpDragDownStart = true;
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
        movementAcceleration = onGround ? runningSpeedAcceleration : onAirSpeedAcceleration;
        movementDeceleration = onGround ? runningSpeedDeceleration : onAirSpeedControl;
        movementTurnSpeed = (onGround ? runningSpeedTurn : onAirSpeedControl) * 2;

        float result = 0f;
        if (inputDirectionX != 0)
        {
            if (Mathf.Sign(inputDirectionX) != Mathf.Sign(thisRigidbody.velocity.x))
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
