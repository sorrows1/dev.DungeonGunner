using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    // Room Information
    public string roomNodeID { get; private set; }
    public string templateID { get; private set; }
    public GameObject prefab { get; private set; }
    public RoomNodeTypeSO roomNodeType { get; private set; }

    // Room Vector Positions
    public Vector2Int lowerBounds { get; set; }
    public Vector2Int upperBounds { get; set; }
    public Vector2Int templateLowerBounds { get; private set; }
    public Vector2Int templateUpperBounds { get; private set; }
    public Vector2Int[] spawnPositionArray { get; private set; }

    // Room Connections
    public string parentRoomID { get; private set; }
    public List<string> childRoomIDList { get; private set; }
    public List<Doorway> doorwayList;

    // Room Details
    public InstantiatedRoom instantiatedRoom { get; set; }
    public bool isPositioned { get; set; } = false;
    public bool isLit { get; set; } = false;
    public bool isClearedOfEnemies { get; set; } = false;
    public bool isPreviouslyVisited { get; set; } = false;

    public Room(RoomTemplateSO _roomTemplate, RoomNodeSO _roomNode)
    {
        this.templateID = _roomTemplate.guid;
        this.prefab = _roomTemplate.prefab;
        this.roomNodeType = _roomTemplate.roomNodeType;
        this.lowerBounds = _roomTemplate.lowerBounds;
        this.upperBounds = _roomTemplate.upperBounds;
        this.templateLowerBounds = _roomTemplate.lowerBounds;
        this.templateUpperBounds = _roomTemplate.upperBounds;
        this.spawnPositionArray = _roomTemplate.spawnPositionArray;
        this.spawnPositionArray = HelperUtilities.CloneEnumerable<Vector2Int>(_roomTemplate.spawnPositionArray);
        this.doorwayList = CloneDoorwayList(_roomTemplate.doorwayList);

        this.roomNodeID = _roomNode.id;
        this.parentRoomID = !roomNodeType.isEntrance ? _roomNode.parentRoomNodeIDList[0] : "";
        this.childRoomIDList = HelperUtilities.CloneEnumerable<string>(_roomNode.childRoomNodeIDList);
    }

    List<Doorway> CloneDoorwayList(List<Doorway> listToClone)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in listToClone)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isAvailable = doorway.isAvailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }
}
