using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(InputSystem))]
public class PlayerControl : MonoBehaviour
{
    [Tooltip("The player WeaponShootPosition gameObject")]
    [SerializeField] Transform weaponShootPosition;
    [SerializeField] MovementDetailsSO movementDetails;

    Player player;

    float moveSpeed;
    Vector2 moveDirection;
    Vector2 currentMousePosition;

    Coroutine playerRollCoroutine;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    bool isPlayerRolling = false;
    float playerRollCooldownTimer = 0f;

    void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    void Start()
    {
        SetPlayerAnimationSpeed();
    }

    void Update()
    {
        if (isPlayerRolling) return;

        ProcessMovementInput();

        PlayerRollCooldownTimer();
    }

    #region Input Actions
    void OnMove(InputValue moveInput) => moveDirection = moveInput.Get<Vector2>();

    void OnMouseMove(InputValue moveInput)
    {
        if (isPlayerRolling) return;
        currentMousePosition = moveInput.Get<Vector2>();
        var (weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection) = AimWeaponInput();
    }

    void OnRightMouseClick()
    {
        if (isPlayerRolling || playerRollCooldownTimer > float.Epsilon) return;

        RollPlayer();
    }
    #endregion

    void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    AimResult AimWeaponInput()
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition(currentMousePosition);

        Vector3 weaponDirection = mouseWorldPosition - weaponShootPosition.position;
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        float playerAngleDegrees = HelperUtilities.GetAngleFromVector(mouseWorldPosition - transform.position);
        AimDirection playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);

        return new AimResult(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);
    }

    void RollPlayer()
    {
        if (moveDirection == Vector2.zero) return;

        playerRollCoroutine = StartCoroutine(RollPlayerToPosition(moveDirection));
    }

    IEnumerator RollPlayerToPosition(Vector3 direction)
    {
        isPlayerRolling = true;
        playerRollCooldownTimer = movementDetails.rollCooldownTime;

        Vector3 targetPosition = transform.position + (direction * movementDetails.rollDistance);

        float minDistance = 0.2f;

        while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, direction, movementDetails.rollSpeed, isPlayerRolling);
            yield return waitForFixedUpdate;
        }

        player.transform.position = targetPosition;
        StopPlayerRoll();
    }


    void ProcessMovementInput()
    {
        if (moveDirection == Vector2.zero)
        {
            player.idleEvent.CallIdleEvent();
            return;
        }

        player.movementByVelocityEvent.CallMovementByVelocityEvent(moveDirection, moveSpeed);
    }

    void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer >= 0f) playerRollCooldownTimer -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        StopPlayerRoll();
    }

    void OnCollisionStay2D(Collision2D other)
    {
        StopPlayerRoll();
    }

    void StopPlayerRoll()
    {
        isPlayerRolling = false;
        AimWeaponInput();

        if (playerRollCoroutine == null) return;
        StopCoroutine(playerRollCoroutine);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
}

record AimResult(Vector3 WeaponDirection, float WeaponAngleDegrees, float PlayerAngleDegrees, AimDirection PlayerAimDirection);
