using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float horizontalSpeed;
    [SerializeField] ForceMode2D movementForceMode;

    float horizontalInput;
    float horizontalInputLastFrame;

    [Header("Friction")]
    [SerializeField] float groundFrictionAmount;
    [SerializeField] bool useLerp;

    [Header("Lerp Based Friction")]
    [SerializeField] AnimationCurve lerpCurve;
    [SerializeField] float lerpSpeed;

    float current;
    float startVelocity;

    [Header("Quick Turn")]
    [SerializeField] TurnForceType turnForceType;
    [SerializeField] ForceMode2D turnForceMode;
    // Minimum time between two turn forces being added
    [SerializeField] float turnForceDelay;
    [SerializeField] float turnForceConstant;
    [SerializeField] float turnForceMultiplier;
    [SerializeField] float turnForceAddition;

    float turnForceTimer;

    enum TurnForceType { Constant, Multiplicative, Additive, None }

    [Header("Clamping")]
    [SerializeField] float maxMoveVelocity;
    [SerializeField] float maxFallVelocity;
    [SerializeField] float maxJumpVelocity;

    [Header("Components")]
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] CircleCollider2D circleCollider;

    // Update is called once per frame
    void Update()
    {
        horizontalInputLastFrame = horizontalInput;
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
        Vector2 force = horizontalInput * horizontalSpeed * Vector2.right;
        rigidbody.AddForce(force, movementForceMode);

        Vector2 clampedMovement = new()
        {
            x = Mathf.Clamp(rigidbody.velocity.x, -maxMoveVelocity, maxMoveVelocity),
            y = Mathf.Clamp(rigidbody.velocity.y, -maxFallVelocity, maxJumpVelocity)
        };

        rigidbody.velocity = clampedMovement;
    }

    // Slows player movement over time using LERP* when no movement keys are being pressed
    void AddFriction()
    {
        if (horizontalInput != 0)
        {
            current = 0;
            startVelocity = rigidbody.velocity.x;
            return;
        }

        Vector2 newVelocity = rigidbody.velocity;
        if (useLerp)
        {
            current = Mathf.MoveTowards(current, 1, lerpSpeed * Time.fixedDeltaTime);
            newVelocity.x = Mathf.Lerp(startVelocity, 0, lerpCurve.Evaluate(current));
        }
        else
            newVelocity.x = Mathf.MoveTowards(newVelocity.x, 0, groundFrictionAmount);

        rigidbody.velocity = newVelocity;
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
                x = turnForceType switch
                {
                    TurnForceType.Multiplicative => rigidbody.velocity.x * turnForceMultiplier,
                    TurnForceType.Additive => rigidbody.velocity.x + turnForceAddition,
                    TurnForceType.Constant => turnForceConstant,
                    TurnForceType.None => 0,
                    _ => throw new System.NotImplementedException("Turn Force Type not handled"),
                }
            };
            addForce.x = Mathf.Abs(addForce.x);

            rigidbody.AddForce(addForce * horizontalInput, turnForceMode);
            Debug.Log($"Added {addForce * horizontalInput} turn force");

            turnForceTimer = turnForceDelay; 
        }

    }
}
