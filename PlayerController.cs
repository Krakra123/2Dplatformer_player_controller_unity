using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D thisRigidbody;
    BoxCollider2D thisCollider;

    private float desiredtVelocityX;

    [Header("Ground")]
    public string movementInputAxis;
    public string jumpInputButton;
    [HideInInspector] public int inputDirectionX;
    [HideInInspector] public bool inputJump;

    [Header("Ground")]
    public LayerMask groundMask;
    [HideInInspector] public bool onGround;

    [Header("Running")]
    public float runningMaxSpeed;
    [Range(.1f, 50f)] public float runningSpeedAcceleration;
    [Range(.1f, 50f)] public float runningSpeedDeceleration;
    [Range(.1f, 50f)] public float runningSpeedTurn;

    [Header("Jumping")]
    public float jumpHeight;
    public float gravityScale;
    [Range(1f, 10f)] public float onAirDownGravityScale;
    [Range(.1f, 50f)] public float onAirSpeedAcceleration;
    [Range(.1f, 50f)] public float onAirSpeedControl;
    [Range(0f, 5f)] public float variableJumpScale;

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
        inputJump = Input.GetButton(jumpInputButton);

        OnGroundCheck();

        Jumping();

        desiredtVelocityX = inputDirectionX * runningMaxSpeed;
    }

    private void FixedUpdate()
    {
        Movement();

        Gravity();
    }

    private void OnGroundCheck()
    {
        float groundCheckThick = .1f;
        float groundCheckOffset = .1f;

        Vector2[] groundCheckCorner = new Vector2[2];
        groundCheckCorner[0] = transform.position + (Vector3)thisCollider.offset + new Vector3(-thisCollider.size.x / 2 + groundCheckOffset, -thisCollider.size.y / 2 - groundCheckOffset);
        groundCheckCorner[1] = transform.position + (Vector3)thisCollider.offset + new Vector3(+thisCollider.size.x / 2 - groundCheckOffset, -thisCollider.size.y / 2 - groundCheckOffset - groundCheckThick);

        if (Physics2D.OverlapArea(groundCheckCorner[0], groundCheckCorner[1], groundMask))
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

    private void Jumping()
    {
        Vector2 _velocity = thisRigidbody.velocity;

        bool variableJumpReset = true;

        if (onGround)
        {
            variableJumpReset = true;

            if (inputJump)
            {
                _velocity.y = jumpHeight;
            }
        }
        else
        {
            if (!inputJump && variableJumpReset) variableJumpReset = false;

            if (!variableJumpReset)
            {
                _velocity.y += variableJumpScale * gravityScale * Physics2D.gravity.y * Time.deltaTime;
            }
        }

        thisRigidbody.velocity = _velocity;
    }

    private void Gravity()
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
