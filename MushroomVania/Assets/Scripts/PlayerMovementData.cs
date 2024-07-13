using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "MovementSetting", menuName = "Player/MovementData")]
public class PlayerMovementData : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] MovementTypes movementType;
    [SerializeField] FrictionTypes frictionType;
    [SerializeField] QuickTurnTypes quickTurnType;
    [SerializeField] ClampTypes clampType;

    public MovementTypes MovementType => movementType;
    public FrictionTypes FrictionType => frictionType;
    public QuickTurnTypes QuickTurnType => quickTurnType;
    public ClampTypes ClampType => clampType;

    public enum MovementTypes { Simple, Complex }
    public enum FrictionTypes { Simple, Lerp, Force, None }
    public enum ClampTypes { None, Horizontal, Vertical, FallOnly, All }
    public enum QuickTurnTypes { None, Constant, Multiplicative, Additive }

    [Header("Simple Movement")]
    [SerializeField] ForceMode2D movementForceMode;
    [SerializeField] float movementSpeed;
    public ForceMode2D MovementForceMode => movementForceMode;
    public float MovementSpeed => movementSpeed;

    [Header("Complex Movement")]
    [SerializeField] float accelerationSpeed;
    [SerializeField] float decelerationSpeed;
    [SerializeField] float velocityPower;
    public float AccelerationSpeed => accelerationSpeed;
    public float DecelerationSpeed => decelerationSpeed;
    public float VelocityPower => velocityPower;

    [Header("Friction")]
    [Range(0, 1)] [Tooltip("0 is no friction, whereas 1 is an instant stop; used by MoveTowards to set speed to 0 over time")]
    [SerializeField] float groundFrictionAmount;
    public float GroundFrictionAmount => groundFrictionAmount;

    [Header("Force Friction")]
    [SerializeField] float forceFrictionAmount;
    public float ForceFrictionAmount => forceFrictionAmount;

    [Header("Lerp Based Friction")]
    [SerializeField] AnimationCurve lerpCurve;
    [SerializeField] float lerpSpeed;
    public AnimationCurve LerpCurve => lerpCurve;
    public float LerpSpeed => lerpSpeed;
    
    [Header("Quick Turn")]
    [SerializeField] ForceMode2D turnForceMode;
    // Minimum time between two turn forces being added
    [SerializeField] float turnForceDelay;
    [SerializeField] float turnForceAmount;
    [SerializeField] float turnForceMultiplier;
    [SerializeField] float turnForceAddition;
    public ForceMode2D TurnForceMode => turnForceMode;

    public float TurnForceDelay => turnForceDelay;
    public float TurnForceConstant => turnForceAmount;
    public float TurnForceMultiplier => turnForceMultiplier;
    public float TurnForceAddition => turnForceAddition;

    [Header("Clamping")]
    [SerializeField] float maxMoveVelocity;
    [SerializeField] float maxFallVelocity;
    [SerializeField] float maxJumpVelocity;

    public float MaxMoveVelocity => maxMoveVelocity;
    public float MaxFallVelocity => maxFallVelocity;
    public float MaxJumpVelocity => maxJumpVelocity;
}
