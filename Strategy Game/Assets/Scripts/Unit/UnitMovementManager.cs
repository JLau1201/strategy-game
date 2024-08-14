using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitMovementManager : MonoBehaviour {

    [SerializeField] private LayerMask enemyLayer;

    private HashSet<GameObject> inMoveRangeSet = new HashSet<GameObject>(); // Hashset of tiles in selected units movement range
    private HashSet<GameObject> inAttackRangeSet = new HashSet<GameObject>();

    // Variables for direction arrow
    private List<GameObject> tilesHoveredList = new List<GameObject>(); // List of tiles hovered by cursor for unit movement
    private Vector2 unitGridPos; // Position of unit in grid array

    private int unitMoveRange;
    private int unitAttackRange;
    private GameObject lastTileHovered = null;
    private GameObject selectedUnit;
    private Unit unit;

    private bool isAlliedUnitSelected = false;

    // Variables for BFS
    private GameObject startTile;

    [SerializeField] private float moveSpeed = 1;
    private bool isUnitMoving = false;
    
    public enum SpriteType {
        Arrow,
        Line,
        Corner,
    }

    private void Start() {
        Player.Instance.OnUnitMoveStart += Player_OnUnitMoveStart;
        Player.Instance.OnUnitMoveComplete += Player_OnUnitMoveComplete;
        Player.Instance.OnUnitMoveCancelled += Player_OnUnitMoveCancelled;
    }

    private void Player_OnUnitMoveStart(object sender, Player.OnUnitSelectedEventArgs e) {
        startTile = e.selectedTile;
        selectedUnit = e.selectedUnit;
        
        unit = selectedUnit.GetComponent<Unit>();
        unitMoveRange = unit.MoveRange;
        unitAttackRange = unit.AttackRange;

        unitGridPos = MapManager.Instance.GetTileToGridPosition(startTile);

        ShowMoveRange(unitGridPos, unitMoveRange + unitAttackRange);

        isAlliedUnitSelected = true;

        ComputeShortestPath();
    }

    private void Player_OnUnitMoveComplete(object sender, System.EventArgs e) {
        isAlliedUnitSelected = false;
        lastTileHovered = null;
        // Check if tile selected when unit is unseleceted is in move the move range
        GameObject targetTile = MapManager.Instance.GetTile(CursorInput.Instance.GetCursorPosition());

        if (targetTile == tilesHoveredList[tilesHoveredList.Count - 1]) {
            // Move unit following tilesHoveredList
            tilesHoveredList.RemoveAt(0);

            // Unit is moving disable cursor/select
            Player.Instance.SetCanSelect(false);
            CursorInput.Instance.SetCanMoveCursor(false);
            isUnitMoving = true;
        } else {
            // Clear path/moverange
            ClearMoveRange();
            ClearPath(0);

            // Tile not in move range
            Player.Instance.SetTurnAction(Player.TurnAction.Nothing);
        }
    }

    private void Player_OnUnitMoveCancelled(object sender, System.EventArgs e) {
        isAlliedUnitSelected = false;
        lastTileHovered = null;
        ClearMoveRange();
        ClearPath(0);
    }

    private void Update() {
        // While isAlliedUnitSelected display direction arrow
        if (isAlliedUnitSelected) {
            GameObject tile = MapManager.Instance.GetTile(CursorInput.Instance.GetCursorPosition());
            if(tile != lastTileHovered) {
                ShowMovePath(tile);
            }
        }

        if (isUnitMoving) {
            HandleMovement(); 
        }
    }

    // Unit is moving
    private void HandleMovement() {
        // If movement is done reset everything
        if(tilesHoveredList.Count <= 0) {
            isUnitMoving = false;
            ClearMoveRange();
            tilesHoveredList.Clear();

            Player.Instance.SetTurnAction(Player.TurnAction.UnitMenu);
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

    class QNode {
        public Vector2 pos { get; set; }
        public int range { get; set; }
    }

    // Display the units movement range
    private void ShowMoveRange(Vector2 unitGridPos, int range) {
        // BFS over range
        Queue<QNode> queue = new Queue<QNode>();
        HashSet<QNode> visited = new HashSet<QNode>();

        QNode qnode = new QNode();
        qnode.pos = unitGridPos;
        qnode.range = 0;

        queue.Enqueue(qnode);
        visited.Add(qnode);

        while (queue.Count > 0) {
            QNode current = queue.Dequeue();

            if (current.range > range) {
                continue;
            }

            GameObject tile = MapManager.Instance.gridArray[(int)current.pos.x, (int)current.pos.y].tileGameObject;
            TileOverlays tileOverlays = tile.GetComponent<TileOverlays>();

            GameObject unit = MapManager.Instance.GetUnit(tile.transform.position);
            if (current.range > unitMoveRange) {
                tileOverlays.ShowAttackTile();
                inAttackRangeSet.Add(tile);
            } else if (unit != null) {
                if (1 << unit.layer == enemyLayer) {
                    tileOverlays.ShowAttackTile();
                    inAttackRangeSet.Add(tile);
                } else {
                    tileOverlays.ShowMoveTile();
                    inMoveRangeSet.Add(tile);
                }
            } else if (MapManager.Instance.gridArray[(int)current.pos.x, (int)current.pos.y].tileType != MapManager.TileType.Land) {
                tileOverlays.ShowAttackTile();
                inAttackRangeSet.Add(tile);
                // UH OH if range > 100
                current.range += 100;
            } else {
                tileOverlays.ShowMoveTile();
                inMoveRangeSet.Add(tile);
            }

            foreach(Vector2 neighbor in BreadthFirstSearch.GetValidNeighbors(current.pos, false)){
                QNode nextNode = new QNode();
                nextNode.pos = neighbor;
                nextNode.range = current.range + 1;
                bool found = false;
                foreach (QNode visitedNode in visited) {
                    if(nextNode.pos == visitedNode.pos) {
                        found = true;
                        break;
                    }
                }

                if (!found && nextNode.range < range + 1) {
                    queue.Enqueue(nextNode);
                    visited.Add(nextNode);
                }
            }
        }
    }

    // Clear all the tiles in the units movement range list
    public void ClearMoveRange() {
        foreach (GameObject tile in inMoveRangeSet) {
            tile.GetComponent<TileOverlays>().HideMoveTile();
        }
        foreach (GameObject tile in inAttackRangeSet) {
            tile.GetComponent<TileOverlays>().HideAttackTile();
        }

        inMoveRangeSet.Clear();
        inAttackRangeSet.Clear();
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

        GameObject goalTile = MapManager.Instance.GetTile(CursorInput.Instance.GetCursorPosition());

        tilesHoveredList = BreadthFirstSearch.BFS(MapManager.Instance.gridArray, startTile, goalTile);
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
        if (tilesHoveredList.Count > 0) { 
            tilesHoveredList.RemoveRange(startInd + 1, tilesHoveredList.Count - (startInd + 1));
        }
        if (tilesHoveredList.Count > 1) {
            ComputeArrowAngle(tilesHoveredList.Count-1);
        }
    }
}

