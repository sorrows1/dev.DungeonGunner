using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    public Room room { get; private set; }
    public Grid grid { get; private set; }
    public Tilemap groundTilemap { get; private set; }
    public Tilemap decoration1Tilemap { get; private set; }
    public Tilemap decoration2Tilemap { get; private set; }
    public Tilemap frontTilemap { get; private set; }
    public Tilemap collisionTilemap { get; private set; }
    public Tilemap minimapTilemap { get; private set; }

    [HideInInspector] public Bounds roomColliderBounds;

    private void Awake()
    {
        roomColliderBounds = GetComponent<BoxCollider2D>().bounds;
    }

    public void Initialise(Room _room)
    {
        this.room = _room;

        PopulateTilemapMemberVariables();

        BlockOffUnusedDoorWays();

        // AddObstaclesAndPreferredPaths();

        // CreateItemObstaclesArray();

        // AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    void PopulateTilemapMemberVariables()
    {
        grid = GetComponentInChildren<Grid>();
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        Array.ForEach(tilemaps, (tilemap) =>
        {
            switch (tilemap.gameObject.tag)
            {
                case Tags.ground:
                    groundTilemap = tilemap;
                    break;
                case Tags.decoration1Tilemap:
                    decoration1Tilemap = tilemap;
                    break;
                case Tags.decoration2Tilemap:
                    decoration2Tilemap = tilemap;
                    break;
                case Tags.frontTilemap:
                    frontTilemap = tilemap;
                    break;
                case Tags.collisionTilemap:
                    collisionTilemap = tilemap;
                    break;
                case Tags.minimapTilemap:
                    minimapTilemap = tilemap;
                    break;
                default:
                    break;
            }
        });
    }

    void BlockOffUnusedDoorWays()
    {
        room.doorwayList.ForEach((doorway) =>
        {
            if (doorway.isConnected) return;

            BlockADoorwayOnTilemapLayers(
                new Tilemap[6] { groundTilemap, decoration1Tilemap, decoration2Tilemap, frontTilemap, collisionTilemap, minimapTilemap },
                doorway);
        });
    }

    void BlockADoorwayOnTilemapLayers(Tilemap[] tilemaps, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                Array.ForEach(tilemaps, tilemap => BlockDoorwayHorizontally(tilemap, doorway));
                break;
            case Orientation.east:
            case Orientation.west:
                Array.ForEach(tilemaps, tilemap => BlockDoorwayVertically(tilemap, doorway));
                break;
            default: break;
        }
    }

    void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                Vector3Int currentTilePosition = new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos);
                Vector3Int tileMapToCopyFrom = new Vector3Int(startPosition.x + xPos, startPosition.y - yPos);

                tilemap.SetTile(
                 currentTilePosition,
                 tilemap.GetTile(tileMapToCopyFrom)
                );

                tilemap.SetTransformMatrix(
                    currentTilePosition,
                    tilemap.GetTransformMatrix(tileMapToCopyFrom)
                );
            }
        }
    }
    void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                Vector3Int currentTilePosition = new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos);
                Vector3Int tileMapToCopyFrom = new Vector3Int(startPosition.x + xPos, startPosition.y - yPos);

                tilemap.SetTile(
                 currentTilePosition,
                 tilemap.GetTile(tileMapToCopyFrom)
                );

                tilemap.SetTransformMatrix(
                    currentTilePosition,
                    tilemap.GetTransformMatrix(tileMapToCopyFrom)
                );
            }
        }
    }

    void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
