using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(Vector3 targetPosition, Vector3 currentPosition, Vector2 moveDirection, float moveSpeed, bool isRolling)
    {
        OnMovementToPosition?.Invoke(this, new MovementToPositionArgs() { targetPosition = targetPosition, currentPosition = currentPosition, moveDirection = moveDirection, moveSpeed = moveSpeed, isRolling = isRolling });
    }
}

public class MovementToPositionArgs : EventArgs
{
    public Vector3 targetPosition;
    public Vector3 currentPosition;
    public Vector2 moveDirection;
    public float moveSpeed;
    public bool isRolling;
}