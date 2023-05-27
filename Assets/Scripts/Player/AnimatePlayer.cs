using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class AnimatePlayer : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition += MovementByVelocityEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition -= MovementByVelocityEvent_OnMovementToPosition;
    }

    void IdleEvent_OnIdle(IdleEvent sender)
    {
        InitalizeRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    void AimWeaponEvent_OnWeaponAim(AimWeaponEvent sender, AimWeaponEventArgs e)
    {
        InitializeAimAnimationParameters();
        InitalizeRollAnimationParameters();
        SetAimWeaponAnimationParameters(e.aimDirection);
    }

    void InitializeAimAnimationParameters()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        switch (aimDirection)
        {
            case AimDirection.Up:
                player.animator.SetBool(Settings.aimUp, true);
                break;

            case AimDirection.UpRight:
                player.animator.SetBool(Settings.aimUpRight, true);
                break;

            case AimDirection.UpLeft:
                player.animator.SetBool(Settings.aimUpLeft, true);
                break;

            case AimDirection.Right:
                player.animator.SetBool(Settings.aimRight, true);
                break;

            case AimDirection.Left:
                player.animator.SetBool(Settings.aimLeft, true);
                break;

            case AimDirection.Down:
                player.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }

    void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent sender, MovementByVelocityArgs e)
    {
        InitalizeRollAnimationParameters();
        SetMovementAnimationParameters();
    }

    void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    void MovementByVelocityEvent_OnMovementToPosition(MovementToPositionEvent sender, MovementToPositionArgs e)
    {
        InitializeAimAnimationParameters();
        InitalizeRollAnimationParameters();
        SetMovementToPositonAnimationParameters(e);
    }

    void InitalizeRollAnimationParameters()
    {
        player.animator.SetBool(Settings.rollDown, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollUp, false);
    }

    void SetMovementToPositonAnimationParameters(MovementToPositionArgs e)
    {
        if (e.moveDirection.x > 0f)
        {
            player.animator.SetBool(Settings.rollRight, true);
        }
        else if (e.moveDirection.x < 0f)
        {
            player.animator.SetBool(Settings.rollLeft, true);
        }
        else if (e.moveDirection.y > 0f)
        {
            player.animator.SetBool(Settings.rollUp, true);
        }
        else if (e.moveDirection.y < 0f)
        {
            player.animator.SetBool(Settings.rollDown, true);
        }
    }

}