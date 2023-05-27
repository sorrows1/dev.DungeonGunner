using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    [field: Space(10)]
    [field: Header("PLAYER BASE DETAILS")]
    [Tooltip("Player character name.")]
    [field: SerializeField] public string PlayerCharacterName { get; private set; }

    [Tooltip("Prefab gameobject for the player")]
    [field: SerializeField] public GameObject PlayerPrefab { get; private set; }

    [Tooltip("Player runtime animator controller")]
    [field: SerializeField] public RuntimeAnimatorController RuntimeAnimatorController { get; private set; }

    [field: Space(10)]
    [field: Header("HEALTH")]
    [Tooltip("Player starting health amount")]
    [field: SerializeField] public int PlayerHealth { get; private set; }

    [field: Space(10)]
    [field: Header("OTHERS")]
    [Tooltip("Player icon sprite to be used in the minimap")]
    [field: SerializeField] public Sprite PlayerMiniMapIcon { get; private set; }

    [Tooltip("Player hand sprite")]
    [field: SerializeField] public Sprite PlayerHandSprite { get; private set; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(PlayerCharacterName), PlayerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerPrefab), PlayerPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerMiniMapIcon), PlayerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerHandSprite), PlayerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(RuntimeAnimatorController), RuntimeAnimatorController);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(PlayerHealth), PlayerHealth, false);
    }
#endif
}
