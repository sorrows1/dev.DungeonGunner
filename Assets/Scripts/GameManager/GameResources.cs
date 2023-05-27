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
}
