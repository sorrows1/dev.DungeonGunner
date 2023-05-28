using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameobject")]
    [SerializeField] BoxCollider2D doorCollider;
    [HideInInspector] public bool isBossRoomDoor { get; set; }

    BoxCollider2D doorTrigger;
    Animator animator;

    bool isOpen = false;

    private void Awake()
    {
        doorCollider.enabled = false;
        doorTrigger = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.SetBool(Settings.open, isOpen);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Tags.player || other.tag == Tags.playerWeapon)
            OpenDoor();
    }

    void OpenDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = false;
        isOpen = true;
        animator.SetBool(Settings.open, true);
    }

    public void LockDoor()
    {
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (isOpen) OpenDoor();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
}
