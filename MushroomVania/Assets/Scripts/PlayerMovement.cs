using System;
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
        => horizontalInput = Input.GetAxisRaw("Horizontal");

    bool isGrounded = true;

    private void FixedUpdate()
    {
        MovePlayer();
        AddFriction();
        AddTurnVelocity();
        ClampMovement();
    }

    // Decides what approach to use to move the player
    void MovePlayer()
    {
        if (movementData.MovementType == MovementTypes.Complex2)
        {
            Run();
            return;
        }
        Vector2 force = movementData.MovementType switch
        {
            MovementTypes.Complex => CalculateComplexMovement() * Vector2.right,
            MovementTypes.Simple => horizontalInput * movementData.MovementSpeed * Vector2.right,
            _ => throw new NotImplementedException()
        };

        rigidbody.AddForce(force, movementData.MovementForceMode);
    }

    // Limits the player's horizontal and/or vertical movement based on movement setting parameters
    void ClampMovement()
    {
        Vector2 clampedMovement = new()
        {
            x = movementData.ClampType switch
            {
                ClampTypes.All or
                ClampTypes.Horizontal => Mathf.Clamp(rigidbody.velocity.x, -movementData.MaxMoveVelocity, movementData.MaxMoveVelocity),

                _ => rigidbody.velocity.x,
            },

            y = movementData.ClampType switch
            {
                ClampTypes.All or
                ClampTypes.Vertical => Mathf.Clamp(rigidbody.velocity.y, -movementData.MaxMoveVelocity, movementData.MaxMoveVelocity),
                ClampTypes.FallOnly => Mathf.Clamp(rigidbody.velocity.y, -movementData.MaxMoveVelocity, int.MaxValue),
                
               _ => rigidbody.velocity.y,
            }
        };

        rigidbody.velocity = clampedMovement;
    }

    // Applies varying degrees of force based on the player's current speed, compared to their maximum speed
    // Factors in acceleration and deceleration
    float CalculateComplexMovement()
    {
        float targetSpeed = movementData.MovementSpeed * horizontalInput;
        float speedDifference = targetSpeed - rigidbody.velocity.x;

        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.AccelerationSpeed : movementData.DecelerationSpeed;
        float movement = Mathf.Pow(Mathf.Abs(speedDifference) * accelerationRate, movementData.VelocityPower) * Mathf.Sign(speedDifference);

        return movement;
    }

    private void Run()
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = horizontalInput * movementData.MovementSpeed;

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (isGrounded)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.AccelerationSpeed : movementData.DecelerationSpeed;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.AccelerationSpeed * movementData.AirAccelerationMultiplier: movementData.DecelerationSpeed * movementData.AirDecelerationMultiplier;
        #endregion

        //Not used since no jump implemented here, but may be useful if you plan to implement your own
        /* 
		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < movementData.jumpHangTimeThreshold)
		{
			accelRate *= movementData.jumpHangAccelerationMult;
			targetSpeed *= movementData.jumpHangMaxSpeedMult;
		}
		#endregion
		*/

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (movementData.ConserveMomentum && Mathf.Abs(rigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && isGrounded)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rigidbody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    // Slows player movement over time using LERP* when no movement keys are being pressed
    void AddFriction()
    {
        if (movementData.MovementType == MovementTypes.Complex2)
            return;

        if (Mathf.Abs(horizontalInput) > 0.01)
        {
            current = 0;
            startVelocity = rigidbody.velocity.x;
            return;
        }

        // Decides what approach to use to apply friction
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
                x = movementData.QuickTurnType switch
                {
                    QuickTurnTypes.Multiplicative => rigidbody.velocity.x * movementData.TurnForceMultiplier,
                    QuickTurnTypes.Additive => rigidbody.velocity.x + movementData.TurnForceAddition,
                    QuickTurnTypes.Constant => movementData.TurnForceConstant,
                    QuickTurnTypes.None => 0,
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
