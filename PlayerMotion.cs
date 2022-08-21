using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    Rigidbody2D thisRigidbody;

    Vector2 velocity;

    [HideInInspector] public int inputDirectionX;

    [Header("Running")]
    public float runningMaxSpeed;
    [Range(.1f, 100f)] public float runningSpeedAcceleration;
    [Range(.1f, 100f)] public float runningSpeedDeceleration;
    [Range(.1f, 100f)] public float runningSpeedTurn;
    
    [Header("Jumping")]
    public float jumpHeight;

    float movementAcceleration;
    float movementDeceleration;
    float movementTurnSpeed;

    float desiredtVelocityX;

    private void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
    }

    public void Run(float inputMovement)
    {
        inputDirectionX = (int)Mathf.Sign(inputMovement);
        if (inputMovement == 0) inputDirectionX = 0;
    }

    private void Update()
    {
        desiredtVelocityX = inputDirectionX * runningMaxSpeed;
    }

    private void FixedUpdate()
    {
        velocity = thisRigidbody.velocity;

        movementAcceleration = runningSpeedAcceleration;
        movementDeceleration = runningSpeedDeceleration;
        movementTurnSpeed = runningSpeedTurn;

        float velocityXChangeSpeed = 0f;
        if (inputDirectionX != 0)
        {
            if (Mathf.Sign(inputDirectionX) != Mathf.Sign(velocity.x))
            {
                velocityXChangeSpeed = movementTurnSpeed * Time.deltaTime;
            }
            else
            {
                velocityXChangeSpeed = movementAcceleration * Time.deltaTime;
            }
        }
        else
        {
            velocityXChangeSpeed = movementDeceleration * Time.deltaTime;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, desiredtVelocityX, velocityXChangeSpeed);
        Debug.Log(velocity.x);

        thisRigidbody.velocity = velocity;
    }
}
