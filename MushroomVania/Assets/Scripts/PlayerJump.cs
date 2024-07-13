using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Settings")]
    PlayerJumpData jumpData;

    [Header("Components")]
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] CircleCollider2D circleCollider;
    public float LastPressedJumpTime {get; private set; } = 0;
    public float LastOnGroundTime { get; private set; } = 0;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = jumpData.JumpHeight;

        if (rigidbody.velocity.y < 0)
            force -= rigidbody.velocity.y;

        rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }
}
