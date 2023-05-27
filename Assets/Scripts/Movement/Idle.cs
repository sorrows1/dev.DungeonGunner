using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
public class Idle : MonoBehaviour
{
    Rigidbody2D rigidBody;
    IdleEvent idleEvent;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable() {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    void IdleEvent_OnIdle(IdleEvent sender)
    {
        StopRigidBody();
    }

    void StopRigidBody()
    {
        rigidBody.velocity = Vector2.zero;
    }
}
