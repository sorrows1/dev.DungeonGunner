using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }
    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent sender, MovementToPositionArgs e)
    {
        MoveRigidBody(e.moveDirection, e.moveSpeed);
    }

    void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        Vector2 targetPosition = rigidBody2D.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rigidBody2D.MovePosition(targetPosition);
    }
}
