using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager instance { get; private set; }

    [Header("Transforms")]
    [SerializeField] private Transform playableField;

    private List<Tile> tileList = new List<Tile>();

    private GameObject[,] gridArray;
    int columns;
    int rows;

    public enum TileType {
        Ground,
        Water,
    }

    private void Awake() {
        instance = this;
        // Sort the playable field area into vector2
        SortGrid();
        GenerateGridArray();
    }

    private void GenerateGridArray() {
        float topLeftX = tileList[0].X;
        float topLeftY = tileList[0].Y;
        float bottomRightX = tileList[tileList.Count - 1].X;
        float bottomRightY = tileList[tileList.Count - 1].Y;

        columns = (int) (Mathf.Abs(topLeftX) + Mathf.Abs(bottomRightX)) + 1;
        rows = (int) (Mathf.Abs(topLeftY) + Mathf.Abs(bottomRightY)) + 1;

        gridArray = new GameObject[rows, columns];

        int tileListInd = 0;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < columns; j++) {
                gridArray[i, j] = tileList[tileListInd].TileGO;
                tileListInd++;
            }
        }
    }

    // Takes gameobjects in shape of grid
    private void SortGrid() {
        foreach(Transform tileTransform in playableField) {
            Tile tile = new Tile(tileTransform.position.x, tileTransform.position.z, tileTransform.gameObject);
            tileList.Add(tile);
        }

        tileList = tileList.OrderByDescending(vec => vec.Y).ThenBy(vec => vec.X).ToList();
    }

    public GameObject[,] GetGridArray() {
        return gridArray;
    }

    // Return the position of the tile gameobject within the grid array
    public Vector2 GetTileToGridPosition(GameObject tile) {
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < columns; j++) {
                if (gridArray[i, j] == tile) {
                    Vector2 pos = new Vector2(i, j);
                    return pos;
                }
            }
        }
        return new Vector2(-1, -1);
    }

    // Return whether the position is valid (not out of bounds)
    public bool IsValidTilePosition(Vector2 tilePosition) {
        if(tilePosition.x < rows && tilePosition.x > -1 && tilePosition.y < columns && tilePosition.y > -1) {
            return true;
        }
        return false;
    }
}
