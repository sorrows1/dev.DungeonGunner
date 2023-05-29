using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    [Tooltip("Populate with the dungeon level scriptable objects")]
    [SerializeField] List<DungeonLevelSO> dungeonlevelList;

    [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]
    [SerializeField] int currentDungeonListIndex = 0;

    public Room CurrentRoom { get; private set; }
    Room previousRoom;
    PlayerDetailsSO playerDetails;
    public Player Player { get; private set; }

    GameState gameState;

    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.PlayerDetails;

        InstantiatePlayer();
    }

    void Start()
    {
        gameState = GameState.gameStarted;
    }

    void Update()
    {
        HandleGameState();

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }

    void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.PlayerPrefab);
        Player = playerGameObject.GetComponent<Player>();
        Player.Initialize(playerDetails);
    }

    void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonListIndex);
                gameState = GameState.playingLevel;
                break;
            case GameState.playingLevel:
                break;
            default:
                break;
        }
    }

    void PlayDungeonLevel(int dungeonLevellistIndex)
    {
        DungeonBuilder.Instance.GenerateDungeon(dungeonlevelList[currentDungeonListIndex]);

        StaticEventHandler.CallRoomChangeEvent(CurrentRoom);

        Vector3 midRoomPosition = new Vector3((CurrentRoom.lowerBounds.x + CurrentRoom.upperBounds.x) / 2f, (CurrentRoom.lowerBounds.y + CurrentRoom.upperBounds.y) / 2f);
        
        // get nearest spawn point in room
        Player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(midRoomPosition);
    }

    public void SetCurrentRoom(Room room)
    {
        previousRoom = CurrentRoom;
        CurrentRoom = room;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonlevelList), dungeonlevelList);
    }
#endif
}
