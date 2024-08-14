using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Transforms")]
    [SerializeField] private Transform playableField;
    [SerializeField] private Transform obstacles;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask unitMask;
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask obstacleMask;

    private List<Transform> tileList = new List<Transform>();
    private List<Transform> obstacleList = new List<Transform>();

    public Tile[,] gridArray;
    int columns;
    int rows;

    public class Tile {
        public TileType tileType { get; set; }

        public GameObject tileGameObject { get; set; }
    }

    public enum TileType {
        Land,
        Obstacle,
        Structure,
    }

    private void Awake() {
        Instance = this;
        // Sort the playable field area into vector2
        SortGrid();
        GenerateGridArray();
    }

    // Generate the grid based on the map
    private void GenerateGridArray() {
        float topLeftX = tileList[0].position.x;
        float topLeftY = tileList[0].position.z;
        float bottomRightX = tileList[tileList.Count - 1].position.x;
        float bottomRightY = tileList[tileList.Count - 1].position.z;

        columns = (int) (Mathf.Abs(topLeftX) + Mathf.Abs(bottomRightX)) + 1;
        rows = (int) (Mathf.Abs(topLeftY) + Mathf.Abs(bottomRightY)) + 1;

        gridArray = new Tile[rows, columns];

        int tileListInd = 0;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {

                Tile tile = new Tile();
                tile.tileGameObject = tileList[tileListInd].gameObject;
                Vector2 tilePos = new Vector2(tile.tileGameObject.transform.position.x, tile.tileGameObject.transform.position.z);

                if (obstacleList.Count > 0) {
                    Vector2 obsPos = new Vector2(obstacleList[0].transform.position.x, obstacleList[0].transform.position.z);
                    tile.tileType = (tilePos == obsPos) ? TileType.Obstacle : TileType.Land;
                    if (tile.tileType == TileType.Obstacle) {
                        obstacleList.RemoveAt(0);
                    }
                } else {
                    tile.tileType = TileType.Land;
                }

                gridArray[i, j] = tile;
                tileListInd++;
            }
        }
    }

    // Takes gameobjects in shape of grid
    private void SortGrid() {
        foreach(Transform tileTransform in playableField) {
            tileList.Add(tileTransform);
        }

        foreach(Transform obstacleTransform in obstacles) {
            obstacleList.Add(obstacleTransform);
        }

        tileList = tileList.OrderByDescending(vec => vec.position.z).ThenBy(vec => vec.position.x).ToList();
        obstacleList = obstacleList.OrderByDescending(vec => vec.position.z).ThenBy(vec => vec.position.x).ToList();
    }

    // Return the position of the tile gameobject within the grid array
    public Vector2 GetTileToGridPosition(GameObject tile) {
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < columns; j++) {
                if (gridArray[i, j].tileGameObject == tile) {
                    Vector2 pos = new Vector2(i, j);
                    return pos;
                }
            }
        }
        return new Vector2(-1, -1);
    }

    // Return whether the position is valid (not out of bounds) or is non land
    public bool IsValidGridPosition(Vector2 gridPosition, bool isTileType) {
        bool isWithinBounds = gridPosition.x >= 0 && gridPosition.x < rows && gridPosition.y >= 0 && gridPosition.y < columns;
        if (isTileType) {
            return isWithinBounds && gridArray[(int)gridPosition.x, (int)gridPosition.y].tileType == TileType.Land;
        } else {
            return isWithinBounds;
        }
    }

    // Get the tile under an object
    public GameObject GetTile(Vector3 objPosition) {
        RaycastHit hit;
        float maxDist = 10f;
        Vector3 positionOffset;

        if(objPosition.x % 1 == 0) {
            positionOffset = new Vector3(objPosition.x + .5f, objPosition.y + 5, objPosition.z + .5f);
        }else {
            positionOffset = objPosition;
        }

        if (Physics.Raycast(positionOffset, Vector3.down, out hit, maxDist, tileMask)) {
            return hit.transform.gameObject;
        }

        return null;
    }

    // Get the Unit above a tile
    public GameObject GetUnit(Vector3 objPosition) {
        RaycastHit hit;
        float maxDist = 10f;
        Vector3 positionOffset;

        if (objPosition.x % 1 == 0) {
            positionOffset = new Vector3(objPosition.x + .5f, objPosition.y - 5, objPosition.z + .5f);
        } else {
            positionOffset = objPosition;
        }

        
        if (Physics.Raycast(positionOffset, Vector3.up, out hit, maxDist, unitMask)) {
            return hit.transform.gameObject;
        }

        return null;
    }

    // Get the manhattan distance from one position to another
    public float GetManhattanDistance(Vector2 startPos, Vector2 endPos) {
        return Mathf.Abs(endPos.x - startPos.x) + Mathf.Abs(endPos.y - startPos.y); 
    }
}
