using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpData : ScriptableObject
{
    [field: SerializeField] public float JumpHeight {  get; private set; }
}
