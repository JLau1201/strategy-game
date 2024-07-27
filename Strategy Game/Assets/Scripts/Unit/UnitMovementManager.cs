using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UnitMovementManager : MonoBehaviour {
    public static UnitMovementManager instance { get; private set; }

    private GameObject[,] gridArray; // Game board
    private HashSet<GameObject> inMoveRangeSet = new HashSet<GameObject>(); // Hashset of tiles in selected units movement range

    // Variables for direction arrow
    private List<GameObject> tilesHoveredList = new List<GameObject>(); // List of tiles hovered by cursor for unit movement
    private bool isAlliedUnitSelected = false;
    private Vector2 unitTilePos; // Position of unit in game
    private Vector2 unitGridPos; // Position of unit in grid array
    private int unitMoveRange;
    private GameObject lastTileHovered = null;
    private GameObject selectedUnit;

    // Variables for BFS
    private GameObject startTile;

    [SerializeField] private float moveSpeed = 1;
    private bool isUnitMoving = false;

    // Directions player can move in
    private Vector2[] dirOffset = {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)};

    public enum SpriteType {
        Arrow,
        Line,
        Corner,
    }

    private void Start() {
        Player.instance.OnUnitSelected += Player_OnUnitSelected;
        Player.instance.OnUnitUnselected += Player_OnUnitUnselected;
    }

    private void Player_OnUnitUnselected(object sender, System.EventArgs e) {
        // Check if tile selected when unit is unseleceted is in move the move range
        // Move unit following tilesHoveredList
        // Clear path/moverange when done moving
        // Moverange clears while moving over it
        isAlliedUnitSelected = false;
        lastTileHovered = null;
        GameObject targetTile = Cursor.instance.GetTileOnCursor();
        if (targetTile == tilesHoveredList[tilesHoveredList.Count - 1]) {
            // Move unit
            tilesHoveredList.RemoveAt(0);
            Player.instance.SetCanSelect(false);
            isUnitMoving = true;
        } else {
            ClearMoveRange();
            ClearPath(0);
        }
    }

    private void Player_OnUnitSelected(object sender, Player.OnUnitSelectedEventArgs e) {
        startTile = e.selectedTile;
        unitTilePos = new Vector2(startTile.transform.position.x, startTile.transform.position.y);
        unitGridPos = MapManager.instance.GetTileToGridPosition(startTile);
        selectedUnit = e.selectedUnit;
        unitMoveRange = selectedUnit.GetComponent<Unit>().GetUnitSO().range;

        ShowMoveRange(unitGridPos, unitMoveRange);

        isAlliedUnitSelected = true;
    }

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // While isAlliedUnitSelected display direction arrow
        if (isAlliedUnitSelected) {
            GameObject tile = Cursor.instance.GetTileOnCursor();
            if(tile != lastTileHovered) {
                ShowMovePath(tile);
            }
        }

        if (isUnitMoving) {
            HandleMovement(); 
        }
    }

    private void HandleMovement() {
        // If movement is done reset everything
        if(tilesHoveredList.Count <= 0) {
            Player.instance.SetCanSelect(true);
            isUnitMoving = false;
            ClearMoveRange();
            tilesHoveredList.Clear();
            return;
        }

        Vector3 unitPos = selectedUnit.transform.position;
        Vector3 targetPos = new Vector3(tilesHoveredList[0].transform.position.x - .5f, 1.5f, tilesHoveredList[0].transform.position.z - .5f);

        selectedUnit.transform.position = Vector3.MoveTowards(unitPos, targetPos, moveSpeed * Time.deltaTime);

        if (unitPos == targetPos) {
            tilesHoveredList[0].GetComponent<TileOverlays>().HideCursorMoveSprite();
            tilesHoveredList.RemoveAt(0);
        }
    }

    // Display the units movement range
    private void ShowMoveRange(Vector2 unitGridPos, int range) {
        // Check if the unit can move on that tile
        if (!MapManager.instance.IsValidGridPosition(unitGridPos)) {
            return;
        }

        gridArray = MapManager.instance.GetGridArray();

        int x = (int)unitGridPos.x;
        int y = (int)unitGridPos.y;

        inMoveRangeSet.Add(gridArray[x, y]);

        gridArray[x, y].GetComponent<TileOverlays>().ShowTile();
        range--;
        if (range > -1) {
            foreach (Vector2 dir in dirOffset) {
                ShowMoveRange(unitGridPos + dir, range);
            }
        }
    }

    // Clear all the tiles in the units movement range list
    public void ClearMoveRange() {
        foreach (GameObject tile in inMoveRangeSet) {
            tile.GetComponent<TileOverlays>().HideTile();
        }
        inMoveRangeSet.Clear();
    }

    // Show the move path from unit to cursor
    private void ShowMovePath(GameObject tile) {
        lastTileHovered = tile;

        int currTileInd;

        // Check tile is not in hovered tiles list
        // Check number of hovered tiles is less equal to unit move range
        if (!tilesHoveredList.Contains(tile) && inMoveRangeSet.Contains(tile)) {
            tilesHoveredList.Add(tile);

            if(tilesHoveredList.Count > 1) {
                // Check for skipped tiles
                GameObject currTile = tilesHoveredList[tilesHoveredList.Count - 1];
                GameObject lastTile = tilesHoveredList[tilesHoveredList.Count - 2];

                // Get position of current tile and tile before current
                Vector2 currTilePos = new Vector2(currTile.transform.position.x, currTile.transform.position.z);
                Vector2 lastTilePos = new Vector2(lastTile.transform.position.x, lastTile.transform.position.z);

                // Compute direction between current and last tile
                Vector2 dir = currTilePos - lastTilePos;
                if (dir != Vector2.up && dir != Vector2.down && dir != Vector2.left && dir != Vector2.right) {
                    ComputeShortestPath();
                    return;
                }
            }

            // If path exceeds unit move range - reroute to shortest path
            if (tilesHoveredList.Count <= unitMoveRange+1) {
                switch (tilesHoveredList.Count) {
                    // If only 1 tile hovered - Dont display anything
                    case 1:
                        break;
                    // If 2 tiles hovered - Do arrow calulation
                    case 2:
                        currTileInd = tilesHoveredList.Count - 1;
                        ComputeArrowAngle(currTileInd);
                        break;
                    // Default do Vector subtraction - Arrow based on last tile direction
                    // Base Line/Corner - direction based on vector subtraction
                    default:
                        currTileInd = tilesHoveredList.Count;
                        ComputeArrowAngle(currTileInd - 1);
                        ComputePrevTile(currTileInd - 2);
                        break;
                }
            } else {
                ComputeShortestPath();
            }
        } else if (tilesHoveredList.Contains(tile) && tile != tilesHoveredList[tilesHoveredList.Count-1]) {
            // If tile hovered is in list, remove all tiles after it
            int tileIndex = tilesHoveredList.FindIndex(t => t == tile);
            ClearPath(tileIndex);
        }
    }

    private void ComputeArrowAngle(int currTileInd) {
        GameObject currTile = tilesHoveredList[currTileInd];
        GameObject lastTile = tilesHoveredList[currTileInd - 1];

        // Get position of current tile and tile before current
        Vector2 currTilePos = new Vector2(currTile.transform.position.x, currTile.transform.position.z);
        Vector2 lastTilePos = new Vector2(lastTile.transform.position.x, lastTile.transform.position.z);

        TileOverlays currTileOverlay = currTile.GetComponent<TileOverlays>();

        float currTileAngle = 0;

        // Compute direction between current and last tile
        Vector2 dir = currTilePos - lastTilePos;

        // Set angle based on direction
        if (dir == Vector2.up) {
            currTileAngle = 0;
        } else if (dir == Vector2.down) {
            currTileAngle = 180;
        } else if (dir == Vector2.left) {
            currTileAngle = 270;
        } else if (dir == Vector2.right) {
            currTileAngle = 90;
        }

        currTileOverlay.SetCursorMoveSprite(SpriteType.Arrow, currTileAngle);
    }

    private void ComputePrevTile(int currTileInd) {
        GameObject nextTile = tilesHoveredList[currTileInd + 1];
        GameObject currTile = tilesHoveredList[currTileInd];
        GameObject lastTile = tilesHoveredList[currTileInd - 1];
        
        // Get position of current tile, tile after and tile before current
        Vector2 nextTilePos = new Vector2(nextTile.transform.position.x, nextTile.transform.position.z);
        Vector2 currTilePos = new Vector2(currTile.transform.position.x, currTile.transform.position.z);
        Vector2 lastTilePos = new Vector2(lastTile.transform.position.x, lastTile.transform.position.z);

        TileOverlays currTileOverlay = currTile.GetComponent<TileOverlays>();

        SpriteType currSpriteType = SpriteType.Line;
        float currTileAngle = 0;

        // Compute direction between next tile and last tile
        Vector2 dir = nextTilePos - lastTilePos;

        // Set angle based on direction
        // If dir = (0, y) overlay is line on y axis
        if (dir.x == 0) {
            currSpriteType = SpriteType.Line;
            currTileAngle = 0;

        // If dir = (x, 0) overlay is line on x axis
        } else if (dir.y == 0) {
            currSpriteType = SpriteType.Line;
            currTileAngle = 90;
        
        // If dir = (x, y) overlay is an corner
        } else if(dir != Vector2.zero) {
            currSpriteType = SpriteType.Corner;

            // Add the next and last directions to directions list
            List<Vector2> directions = new List<Vector2>();
            directions.Add(nextTilePos - currTilePos);
            directions.Add(lastTilePos - currTilePos);

            // Check each combination of paths
            if(directions.Contains(Vector2.right) && directions.Contains(Vector2.down)) {
                currTileAngle = 0;
            }else if(directions.Contains(Vector2.right) && directions.Contains(Vector2.up)){
                currTileAngle = 270;
            }else if(directions.Contains(Vector2.left) && directions.Contains(Vector2.up)) {
                currTileAngle = 180;
            }else if(directions.Contains(Vector2.left) && directions.Contains(Vector2.down)){
                currTileAngle = 90;
            }
            directions.Clear();
        }

        currTileOverlay.SetCursorMoveSprite(currSpriteType, currTileAngle);
    }

    private void ComputeShortestPath() {
        // Clear the path
        ClearPath(0);

        GameObject goalTile = Cursor.instance.GetTileOnCursor();

        tilesHoveredList = BreadthFirstSearch.BFS(gridArray, startTile, goalTile);
        // Draw new path
        for (int i = 0; i < tilesHoveredList.Count; i++) {
            if (i == 0) continue;

            if(i + 1 == tilesHoveredList.Count) {
                ComputeArrowAngle(i);
            } else {
                ComputePrevTile(i);
            }
        }
    }

    private void ClearPath(int startInd) {
        for (int i = startInd + 1; i < tilesHoveredList.Count; i++) {
            GameObject tileToRemove = tilesHoveredList[i];
            TileOverlays tileToRemoveOverlay = tileToRemove.GetComponent<TileOverlays>();

            tileToRemoveOverlay.HideCursorMoveSprite();
        }

        tilesHoveredList.RemoveRange(startInd + 1, tilesHoveredList.Count - (startInd + 1));
        if (tilesHoveredList.Count > 1) {
            ComputeArrowAngle(tilesHoveredList.Count-1);
        }
    }
}

