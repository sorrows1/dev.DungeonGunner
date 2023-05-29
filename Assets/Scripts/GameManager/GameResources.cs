using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get{
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    [Space(10)]
    [Header("DUNGEON")]
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    public RoomNodeTypeListSO roomNodeTypeList;    
    
    [Space(10)]
    [Header("PLAYER")]
    [Tooltip("The current player SO")]
    public CurrentPlayerSO currentPlayer;
    
    [Space(10)]
    [Header("MATERIALS")]
    public Material dimmedMaterial;

    [Tooltip("Sprite-Lit default material")]
    public Material litMaterial;

    [Tooltip("Variable Lit SHader")]
    public Shader variableLitShader;

    #if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader),variableLitShader);
    }
    #endif
}
