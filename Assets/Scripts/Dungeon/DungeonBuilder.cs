using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    List<RoomTemplateSO> roomTemplateList = null;
    RoomNodeTypeListSO roomNodeTypeList;
    bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();

    }

    void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // Load the scriptable object room templates into the dictionary
        // LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts <= Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            // Select a random room node graph from the list
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                // Clear dungeon room gameobjects and dungeon room dictionary
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // Attempt To Build A Random Dungeon For The Selected room node graph
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
            {
                // Instantiate Room Gameobjects
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graphs
    /// </summary>
    RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }

    void ClearDungeon()
    {
        // Destroy instantiated dungeon gameobjects and clear dungeon manager room dictionary
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }

    /// <summary>
    /// Attempt to randomly build the dungeon for the specified room nodeGraph. Returns true if a
    /// successful random layout was generated, else returns false if a problem was encoutered and
    /// another attempt is required.
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {

        // Create Open Room Node Queue
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Add Entrance Node To Room Node Queue From Room Node Graph
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode == null)
        {
            Debug.Log("No Entrance Node");
            return false;  // Dungeon Not Built
        }

        RoomTemplateSO roomTemplate = GetRandomRoomTemplate(entranceNode);
        Room entranceRoom = CreateRoomFromRoomTemplate(roomTemplate, entranceNode);
        dungeonBuilderRoomDictionary.Add(entranceRoom.roomNodeID, entranceRoom);
        entranceNode.childRoomNodeIDList.ForEach((roomNode) => openRoomNodeQueue.Enqueue(roomNodeGraph.GetRoomNode(roomNode)));

        return ProcessRoomsInOpenRoomNodeQueue(openRoomNodeQueue, roomNodeGraph);
    }

    /// <summary>
    /// Process rooms in the open room node queue, returning true if there are no room overlaps
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(Queue<RoomNodeSO> openRoomNodeQueue, RoomNodeGraphSO roomNodeGraph)
    {
        // While room nodes in open room node queue & no room overlaps detected.
        while (openRoomNodeQueue.Count > 0)
        {
            // Get next room node from open room node queue.
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();
            // add the current roomNode's child room node to the queue
            roomNode.childRoomNodeIDList.ForEach((roomNode) => openRoomNodeQueue.Enqueue(roomNodeGraph.GetRoomNode(roomNode)));

            // get parent room for node
            Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];
            // See if room can be placed without overlaps, if not return false
            if (!CanPlaceRoomWithNoOverlaps(roomNode, parentRoom)) return false;
        }

        return true;
    }

    /// <summary>
    /// Attempt to place the room node in the dungeon - if room can be placed return the room, else return null
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {

        // initialise and assume overlap until proven otherwise.
        bool roomOverlaps = true;

        // Do While Room Overlaps - try to place against all available doorways of the parent until
        // the room is successfully placed without overlap.
        while (roomOverlaps)
        {
            // Select random unconnected available doorway for Parent
            List<Doorway> unconnectedAvailableParentDoorways = parentRoom.doorwayList.FindAll(doorway => !doorway.isConnected && doorway.isAvailable);
            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // Debug.LogError($"roomTemplate {parentRoom.roomNodeID} does not have enough doorways.");
                return false;
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // Get a random room template for room node that is consistent with the parent door orientation
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // Create a room
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            // Place the room - returns true if the room doesn't overlap
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // If room doesn't overlap then set to false to exit while loop
                roomOverlaps = false;

                // Mark room as positioned
                room.isPositioned = true;

                // Add room to dictionary
                dungeonBuilderRoomDictionary.Add(room.roomNodeID, room);

            }
            else
            {
                roomOverlaps = true;
            }

        }

        return true;  // no room overlaps
    }

    /// <summary>
    /// Get random room template for room node taking into account the parent doorway orientation.
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // If room node is a corridor then select random correct Corridor room template based on
        // parent doorway orientation
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomCorridorTemplate(roomNodeTypeList.list.Find(roomNodeType => roomNodeType.isCorridorNS));
                    break;


                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomCorridorTemplate(roomNodeTypeList.list.Find(roomNodeType => roomNodeType.isCorridorEW));
                    break;


                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        // Else select random room template
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode);
        }


        return roomtemplate;
    }

    /// <summary>
    /// Create room based on roomTemplate and layoutNode, and return the created room
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // Initialise room from template
        Room room = new Room(roomTemplate, roomNode);

        // Set parent ID for room
        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.isPositioned = true;
            room.isPreviouslyVisited = true;

            // Set entrance in game manager
            GameManager.Instance.SetCurrentRoom(room);
        }

        return room;
    }

    /// <summary>
    /// Place the room - returns true if the room doesn't overlap, false otherwise
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room roomToPlace)
    {
        // Get current room doorway position
        Doorway doorway = GetOppositeDoorway(doorwayParent, roomToPlace.doorwayList);

        // Return if no doorway in room opposite to parent doorway
        if (doorway == null)
        {
            // Debug.LogError($"Room Template {roomToPlace.templateID} does not have a doorway opposite to {doorwayParent.orientation}");
            doorwayParent.isAvailable = false;
            return false;
        }


        // Calculate 'world' grid parent doorway position
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds - parentRoom.templateLowerBounds + doorwayParent.position;

        Vector2Int adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)

        switch (doorwayParent.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
        Vector2Int zeroVectorInTemplatePlacedInWorldPosition = parentDoorwayPosition + adjustment - doorway.position;
        roomToPlace.lowerBounds = zeroVectorInTemplatePlacedInWorldPosition + roomToPlace.templateLowerBounds;
        roomToPlace.upperBounds = zeroVectorInTemplatePlacedInWorldPosition + roomToPlace.templateUpperBounds;

        if (CheckForRoomOverlap(roomToPlace))
        {
            // Debug.LogError("Room has overlapped.");
            doorwayParent.isAvailable = false;
            return false;
        };

        // mark doorways as connected & unavailable
        doorwayParent.isConnected = true;
        doorwayParent.isAvailable = false;

        doorway.isConnected = true;
        doorway.isAvailable = false;
        roomToPlace.isPositioned = true;
        return true;
    }

    bool CheckForRoomOverlap(Room roomToCheck)
    {
        return dungeonBuilderRoomDictionary.Values.Any(placedRoom => Mathf.Max(roomToCheck.lowerBounds.x, placedRoom.lowerBounds.x) <= Mathf.Min(roomToCheck.upperBounds.x, placedRoom.upperBounds.x)
            && Mathf.Max(roomToCheck.lowerBounds.y, placedRoom.lowerBounds.y) <= Mathf.Min(roomToCheck.upperBounds.y, placedRoom.upperBounds.y));
    }

    RoomTemplateSO GetRandomCorridorTemplate(RoomNodeTypeSO roomNodeType)
    {
        // find the room template that has
        List<RoomTemplateSO> roomTemplateByType = roomTemplateList.FindAll(roomTemplate => roomTemplate.roomNodeType == roomNodeType);
        return roomTemplateByType[Random.Range(0, roomTemplateByType.Count)];
    }

    RoomTemplateSO GetRandomRoomTemplate(RoomNodeSO roomNode)
    {
        // find the room template that has
        List<RoomTemplateSO> roomTemplateByType = roomTemplateList.FindAll(roomTemplate => roomTemplate.roomNodeType == roomNode.roomNodeType && roomTemplate.doorwayList.Count >= roomNode.childRoomNodeIDList.Count);
        return roomTemplateByType[Random.Range(0, roomTemplateByType.Count)];
    }

    Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {
        return doorwayList.Find((doorwayToCheck) =>
        {
            return
                (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
                || (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
                || (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
                || (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south);
        });
    }

    void InstantiateRoomGameobjects()
    {
        dungeonBuilderRoomDictionary.Values.All(room =>
        {
            Vector3Int roomPosition = new Vector3Int(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0);
            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameObject.GetComponent<InstantiatedRoom>();
            instantiatedRoom.Initialise(room);
            room.instantiatedRoom = instantiatedRoom;

            return true;
        });
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        return roomTemplateList.Find((roomTemplate) => roomTemplate.guid == roomTemplateID);
    }
}