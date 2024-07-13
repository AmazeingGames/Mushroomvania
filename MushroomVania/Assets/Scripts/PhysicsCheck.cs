using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[ExecuteAlways]
public class PhysicsCheck : MonoBehaviour
{
    [SerializeField] Transform leftGroundCheck;
    [SerializeField] Transform rightGroundCheck;

    [Header("Settings")]
    [SerializeField] float raycastLength;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] Vector2 raycastPositionOffset;

    [Header("Components")]
    [SerializeField] Rigidbody2D rigidbody;
    [SerializeField] Collider2D collider;

    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        // Editor & Play Mode
        leftGroundCheck.localPosition = new(-raycastPositionOffset.x, raycastPositionOffset.y, 0);
        rightGroundCheck.localPosition = new(raycastPositionOffset.x, raycastPositionOffset.y, 0);

        if (!Application.IsPlaying(this))
            return;

        // Play Mode Only
        isGrounded = IsGrounded();
    }

    public bool IsGrounded()
    {
        bool isTouchingGroundLeft = Physics.Raycast(leftGroundCheck.position, Vector3.down, raycastLength, groundLayerMask);
        bool isTouchingGroundRight = Physics.Raycast(leftGroundCheck.position, Vector3.down, raycastLength, groundLayerMask);

        Debug.Log($"{isTouchingGroundLeft} || {isTouchingGroundRight}");
        return isTouchingGroundLeft || isTouchingGroundRight;
    }

    void OnDrawGizmos()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.green;

        Gizmos.DrawRay(leftGroundCheck.position, transform.TransformDirection(Vector3.down) * raycastLength);
        Gizmos.DrawRay(rightGroundCheck.position, transform.TransformDirection(Vector3.down) * raycastLength);
    }
}
