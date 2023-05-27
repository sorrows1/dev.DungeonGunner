using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    MovementByVelocityEvent movementByVelocityEvent;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }
    private void OnDisable()
    {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent sender, MovementByVelocityArgs e)
    {
        MoveRigidBody(e.moveDirection, e.moveSpeed);
    }

    void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        Debug.Log("called");
        rigidBody2D.velocity = moveDirection * moveSpeed;
    }
}