using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovementData;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    float current;
    float startVelocity;
    float turnForceTimer;

    [Header("Data")]
    [SerializeField] PlayerMovementData movementData;

    [Header("Components")]
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] CircleCollider2D circleCollider;

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        MovePlayer();
        AddFriction();
        AddTurnVelocity();
    }

    // Uses forces and physics to move the player
    // Clamps movement on the x and y axes
    void MovePlayer()
    {
        Vector2 force = movementData.UseTargetSpeedMovement switch
        {
            true    => CalculateTargetSpeed() * Vector2.right,
            false   => horizontalInput * movementData.MovementSpeed * Vector2.right,
        };

        rigidbody.AddForce(force, movementData.MovementForceMode);

        if (movementData.UseTargetSpeedMovement)
            return;

        Vector2 clampedMovement = new()
        {
            x = Mathf.Clamp(rigidbody.velocity.x, -movementData.MaxMoveVelocity, movementData.MaxMoveVelocity),
            y = Mathf.Clamp(rigidbody.velocity.y, -movementData.MaxMoveVelocity, movementData.MaxMoveVelocity)
        };

        rigidbody.velocity = clampedMovement;
    }

    float targetSpeed;
    float currentSpeed;
    float speedDifference;

    float accelerationRate;
    float movement;
    // Applies varying degrees of force based on the player's current speed, compared to their maximum speed
    // Factors in acceleration and deceleration
    float CalculateTargetSpeed()
    {
        targetSpeed = movementData.MovementSpeed * horizontalInput;
        currentSpeed = rigidbody.velocity.x;
        speedDifference = targetSpeed - currentSpeed;

        accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.AccelerationSpeed : movementData.DecelerationSpeed;
        movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, movementData.VelocityPower) * Mathf.Sign(speedDifference);

        return movement;
    }

    // Slows player movement over time using LERP* when no movement keys are being pressed
    void AddFriction()
    {
        if (Mathf.Abs(horizontalInput) > 0.01)
        {
            current = 0;
            startVelocity = rigidbody.velocity.x;
            return;
        }

        switch (movementData.FrictionType)
        {
            case FrictionTypes.Lerp:
                Vector2 newVelocity = rigidbody.velocity;

                current = Mathf.MoveTowards(current, 1, movementData.LerpSpeed * Time.fixedDeltaTime);
                newVelocity.x = Mathf.Lerp(startVelocity, 0, movementData.LerpCurve.Evaluate(current));
                rigidbody.velocity = newVelocity;
            break;

            case FrictionTypes.Simple:
                newVelocity = rigidbody.velocity;

                newVelocity.x = Mathf.MoveTowards(newVelocity.x, 0, movementData.GroundFrictionAmount);
                rigidbody.velocity = newVelocity;
            break;

            case FrictionTypes.Force:
                float frictionToAdd = Mathf.Min(Mathf.Abs(rigidbody.velocity.x), Mathf.Abs(movementData.ForceFrictionAmount));
                frictionToAdd *= Mathf.Sign(rigidbody.velocity.x);
                rigidbody.AddForce(Vector2.right * -frictionToAdd, ForceMode2D.Impulse);
            break;
        }

    }

    // We want the player to be able to turn quickly when switching directions
    // Applies a greater force in the direction the player wants to go
    void AddTurnVelocity()
    {
        turnForceTimer -= Time.fixedDeltaTime;

        if (turnForceTimer > 0)
            return;

        var velocity = rigidbody.velocity.x;
        if ((velocity > 0 && horizontalInput < 0) || (velocity < 0 && horizontalInput > 0))
        {
            Vector2 addForce = new()
            {
                x = movementData.TurnForceType switch
                {
                    TurnForceTypes.Multiplicative => rigidbody.velocity.x * movementData.TurnForceMultiplier,
                    TurnForceTypes.Additive => rigidbody.velocity.x + movementData.TurnForceAddition,
                    TurnForceTypes.Constant => movementData.TurnForceConstant,
                    TurnForceTypes.None => 0,
                    _ => throw new System.NotImplementedException("Turn Force Type not handled"),
                }
            };
            addForce.x = Mathf.Abs(addForce.x);

            rigidbody.AddForce(addForce * horizontalInput, movementData.TurnForceMode);
            Debug.Log($"Added {addForce * horizontalInput} turn force");

            turnForceTimer = movementData.TurnForceDelay; 
        }

    }
}
