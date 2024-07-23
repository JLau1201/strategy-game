using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayRange : MonoBehaviour
{
    private GameObject[,] gridArray;
    private List<GameObject> visitedList = new List<GameObject>();

    private Vector2[] dirOffset = { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1),  };

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            ClearMoveRange();

            GameObject selectedUnit = Cursor.instance.GetUnitOnCursor();
            if (selectedUnit == null) return;
            
            GameObject startingTile = Cursor.instance.GetTileOnCursor();
            int range = selectedUnit.GetComponent<Unit>().GetUnitSO().range;
            gridArray = MapManager.instance.GetGridArray();
            Vector2 startingPos = MapManager.instance.GetTilePosition(startingTile);

            ShowMoveRange(startingPos, range);
        }
    }

    private void ShowMoveRange(Vector2 startingPos, int range) {
        if (!MapManager.instance.IsValidTilePosition(startingPos)) {
            return;
        }

        int x = (int)startingPos.x;
        int y = (int)startingPos.y;

        visitedList.Add(gridArray[x, y]);

        gridArray[x, y].GetComponent<ShowMoveAvailable>().ShowTile();
        range--;
        if(range > 0) {
            foreach (Vector2 dir in dirOffset) {
                ShowMoveRange(startingPos + dir, range);
            }
        }
    }

    private void ClearMoveRange() {
        foreach(GameObject tile in visitedList) {
            tile.GetComponent<ShowMoveAvailable>().HideTile();
        }
    }
}
