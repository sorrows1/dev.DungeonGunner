using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    [field: Space(10)]
    [field: Header("MOVEMENT DETAILS")]

    [Tooltip("The minimum move speed")]
    [field: SerializeField] public float MinMoveSpeed { get; private set; } = 8f;

    [Tooltip("The maximum move speed")]
    [field: SerializeField] public float MaxMoveSpeed { get; private set; } = 8f;

    [field: SerializeField] public float rollSpeed { get; private set; }
    [field: SerializeField] public float rollDistance { get; private set; }
    [field: SerializeField] public float rollCooldownTime { get; private set; }


    public float GetMoveSpeed()
    {
        return Random.Range(MinMoveSpeed, MaxMoveSpeed);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(MinMoveSpeed), MinMoveSpeed, nameof(MaxMoveSpeed), MaxMoveSpeed, false);

        HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, true);
    }
#endif
}
