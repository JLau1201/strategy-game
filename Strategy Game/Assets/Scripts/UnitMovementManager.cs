using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.WSA;

public class UnitMovementManager : MonoBehaviour {
    public static UnitMovementManager instance { get; private set; }

    private GameObject[,] gridArray; // Game board
    private List<GameObject> inMoveRangeList = new List<GameObject>(); // List of tiles in selected units movement range

    // Variables for direction arrow
    private List<GameObject> tilesHoveredList = new List<GameObject>(); // List of tiles hovered by cursor for unit movement
    private bool isAlliedUnitSelected = false;
    private Vector2 unitTilePos; // Position of unit in game
    private Vector2 unitGridPos; // Position of unit in grid array
    private int unitMoveRange;

    public enum SpriteType {
        Arrow,
        Line,
        Corner,
    }

    private enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    private void Start() {
        Player.instance.OnUnitSelected += Player_OnUnitSelected; ;
    }

    private void Player_OnUnitSelected(object sender, Player.OnUnitSelectedEventArgs e) {
        unitTilePos = new Vector2(e.selectedTile.transform.position.x, e.selectedTile.transform.position.y);
        unitGridPos = MapManager.instance.GetTileToGridPosition(e.selectedTile);
        unitMoveRange = e.selectedUnit.GetComponent<Unit>().GetUnitSO().range;
        ShowMoveRange(unitGridPos, unitMoveRange);

        isAlliedUnitSelected = true;
    }

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // While isAlliedUnitSelected display direction arrow
        if (isAlliedUnitSelected) {
            ShowMoveDirection();
        }
    }

    // Directions player can move in
    private Vector2[] dirOffset = {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)};

    // Display the units movement range
    private void ShowMoveRange(Vector2 unitGridPos, int range) {
        // Check if the unit can move on that tile
        if (!MapManager.instance.IsValidTilePosition(unitGridPos)) {
            return;
        }

        gridArray = MapManager.instance.GetGridArray();

        int x = (int)unitGridPos.x;
        int y = (int)unitGridPos.y;

        inMoveRangeList.Add(gridArray[x, y]);

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
        foreach (GameObject tile in inMoveRangeList) {
            tile.GetComponent<TileOverlays>().HideTile();
        }
        inMoveRangeList.Clear();
    }

    // Show the move direction arrow
    private void ShowMoveDirection() {
        GameObject tile = Cursor.instance.GetTileOnCursor();
        int tilesHoveredCount = Cursor.instance.GetDistanceToCursor(unitTilePos);

        // Check tile is not in hovered tiles list
        // Check number of hovered tiles is less equal to unit move range
        if (!tilesHoveredList.Contains(tile) && tilesHoveredCount <= unitMoveRange) {
            tilesHoveredList.Add(tile);

            switch (tilesHoveredList.Count) {
                // If only 1 tile hovered - Dont display anything
                case 1:
                    break;
                // If 2 tiles hovered - Do arrow calulation
                case 2:
                    ComputeArrowAngle();
                    break;
                // Default do Vector subtraction - Arrow based on last tile direction
                // Base Line/Corner - direction based on vector subtraction
                default:
                    ComputeArrowAngle();
                    ComputePrevTile();
                    break;
            }
        } else if (tilesHoveredList.Contains(tile)) {
            // If tile hovered is in list, remove all tiles after it
            int tileIndex = tilesHoveredList.FindIndex(t => t == tile);

            for (int i = tileIndex + 1; i < tilesHoveredList.Count; i++) {
                GameObject tileToRemove = tilesHoveredList[i];
                TileOverlays tileToRemoveOverlay = tileToRemove.GetComponent<TileOverlays>();

                tileToRemoveOverlay.HideCursorMoveSprite();
            }

            tilesHoveredList.RemoveRange(tileIndex + 1, tilesHoveredList.Count - (tileIndex + 1));
            if (tilesHoveredList.Count > 1) {
                ComputeArrowAngle();
            }
        } else if (tilesHoveredList.Count > unitMoveRange) {
            //// Hovered tiles exceed unit move range - reroute to shortest path
            //ComputeShortestPath();
        }
    }

    private void ComputeArrowAngle() {
        GameObject currTile = tilesHoveredList[tilesHoveredList.Count - 1];
        GameObject lastTile = tilesHoveredList[tilesHoveredList.Count - 2];

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
            currTileAngle = 90;
        } else if (dir == Vector2.right) {
            currTileAngle = 270;
        } else {
            //// Tiles skipped - reroute to shortest path
            //ComputeShortestPath();
            //return;
        }

        currTileOverlay.SetCursorMoveSprite(SpriteType.Arrow, currTileAngle);
    }
    

    private void ComputePrevTile() {
        GameObject nextTile = tilesHoveredList[tilesHoveredList.Count - 1];
        GameObject currTile = tilesHoveredList[tilesHoveredList.Count - 2];
        GameObject lastTile = tilesHoveredList[tilesHoveredList.Count - 3];
        
        // Get position of current tile, tile after and tile before current
        Vector2 nextTilePos = new Vector2(nextTile.transform.position.x, nextTile.transform.position.z);
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

            // Check which direction arrow is
            float arrowTileAngle = nextTile.GetComponent<TileOverlays>().GetCursorMoveAngle();
            
            // Check different corner types
            if (dir == new Vector2(1, 1)) {
                // Corner has right and up direction                
                switch (arrowTileAngle) {
                    // Arrow faces right
                    case 90:
                        currTileAngle = 0;
                        break;
                    // Arrow faces up
                    case 0:
                        currTileAngle = 180;
                        break;
                }
            } else if (dir == new Vector2(1, -1)) {
                // Corner has right and down direction
                switch (arrowTileAngle) {
                    // Arrow faces right
                    case 90:
                        currTileAngle = 90;
                        break;
                    // Arrow faces down
                    case 180:
                        currTileAngle = 270;
                        break;
                }
            } else if(dir == new Vector2(-1, 1)) {
                // Corner has left and up direction
                switch (arrowTileAngle) {
                    // Arrow faces left
                    case 270:
                        currTileAngle = 270;
                        break;
                    // Arrow faces up
                    case 0:
                        currTileAngle = 90;
                        break;
                }
            } else if(dir == new Vector2(-1, -1)) {
                // Corner has left and down direction
                switch (arrowTileAngle) {
                    // Arrow faces left
                    case 270:
                        currTileAngle = 180;
                        break;
                    // Arrow faces down
                    case 180:
                        currTileAngle = 0;
                        break;
                }
            }
        }

        currTileOverlay.SetCursorMoveSprite(currSpriteType, currTileAngle);
    }

    private void ComputeShortestPath() {
        // Clear tilesHoveredList
        tilesHoveredList.Clear();

        int unitPosX = (int)unitGridPos.x;
        int unitPosY = (int)unitGridPos.y;

        Vector2 cursorPos = MapManager.instance.GetTileToGridPosition(Cursor.instance.GetTileOnCursor());
        int cursorPosX = (int)unitGridPos.x;
        int cursorPosY = (int)unitGridPos.x;
        

    }
}

