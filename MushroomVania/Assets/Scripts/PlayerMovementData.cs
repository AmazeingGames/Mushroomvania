using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementSetting", menuName = "Player/MovementData")]
public class PlayerMovementData : ScriptableObject
{
    [field: Header("Movement")]
    [field: SerializeField] public ForceMode2D MovementForceMode { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; }


    [field: Header("Target Movement")]
    [field: SerializeField] public bool UseTargetSpeedMovement { get; private set; }
    [field: SerializeField] public float AccelerationSpeed { get; private set; }
    [field: SerializeField] public float DecelerationSpeed { get; private set; }
    [field: SerializeField] public float VelocityPower { get; private set; }


    [field: Header("Friction")]
    [field: SerializeField] [field: Range(0, 1)] public float GroundFrictionAmount { get; private set; }


    [field: Header("Force Friction")]
    [field: SerializeField] public float ForceFrictionAmount { get; private set; }


    [field: Header("Lerp Based Friction")]
    [field: SerializeField] public FrictionTypes FrictionType { get; private set; }
    [field: SerializeField] public AnimationCurve LerpCurve { get; private set; }
    [field: SerializeField] public float LerpSpeed { get; private set; }
    public enum FrictionTypes { Simple, Lerp, Force, None }

    
    [field: Header("Quick Turn")]
    [field: SerializeField] public TurnForceTypes TurnForceType { get; private set; }
    [field: SerializeField] public ForceMode2D TurnForceMode { get; private set; }

    // Minimum time between two turn forces being added
    [field: SerializeField] public float TurnForceDelay { get; private set; }
    [field: SerializeField] public float TurnForceConstant { get; private set; }
    [field: SerializeField] public float TurnForceMultiplier { get; private set; }
    [field: SerializeField] public float TurnForceAddition { get; private set; }

    public enum TurnForceTypes { Constant, Multiplicative, Additive, None }


    [field: Header("Clamping")]
    [field: SerializeField] public float MaxMoveVelocity { get; private set; }
    [field: SerializeField] public float MaxFallVelocity { get; private set; }
    [field: SerializeField] public float MaxJumpVelocity { get; private set; }
}
