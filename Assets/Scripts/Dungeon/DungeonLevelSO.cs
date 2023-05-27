using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    [Tooltip("The name of the level")]
    public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    public List<RoomTemplateSO> roomTemplateList;

    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    [Tooltip("Populate this list with the room node graphs which should be randomly selected for this level.")]
    public List<RoomNodeGraphSO> roomNodeGraphList;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName)) return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList) ||
         HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList)) return;

        // check to make sure that all room node types in the specified node graphs have a corresponding room template
        Dictionary<string, Boolean> currentRoomNodeTemplateTypes = new Dictionary<string, Boolean>();
        roomTemplateList.ForEach((roomTemplate) =>
        {
            string currentRoomNodeTypeName = roomTemplate.roomNodeType.roomNodeTypeName;
            currentRoomNodeTemplateTypes[currentRoomNodeTypeName] = true;
        });

        roomNodeGraphList.ForEach((roomNodeGraph) =>
        {
            roomNodeGraph.roomNodeList.ForEach((roomNodeSO) =>
            {
                RoomNodeTypeSO currentRoomNodeType = roomNodeSO.roomNodeType;
                if (currentRoomNodeType.isNone || currentRoomNodeType.isCorridor) return;
                if (currentRoomNodeTemplateTypes.ContainsKey(currentRoomNodeType.roomNodeTypeName)) return;
                Debug.LogError($"In {this.name}: No room template found for room node type {currentRoomNodeType.roomNodeTypeName} in room node graph {roomNodeGraph.name}.");
            });
        });

    }
#endif
}
