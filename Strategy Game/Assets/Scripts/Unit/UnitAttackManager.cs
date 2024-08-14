using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackManager : MonoBehaviour
{

    private HashSet<GameObject> inAttackRangeSet = new HashSet<GameObject>();

    private Vector2 unitGridPos;
    private GameObject startTile;
    private GameObject selectedUnit;
    private Unit unit;
    private int unitAttackRange;
    private float unitAttack;

    private void Start() {
        Player.Instance.OnUnitAttackStart += Player_OnUnitAttackStart;
        Player.Instance.OnUnitAttackComplete += Player_OnUnitAttackComplete;
        Player.Instance.OnUnitAttackCancelled += Player_OnUnitAttackCancelled;
    }

    private void Player_OnUnitAttackStart(object sender, Player.OnUnitSelectedEventArgs e) {
        selectedUnit = e.selectedUnit;
        startTile = MapManager.Instance.GetTile(selectedUnit.transform.position);
        unitGridPos = MapManager.Instance.GetTileToGridPosition(startTile);

        unit = selectedUnit.GetComponent<Unit>();
        unitAttackRange = unit.AttackRange;
        unitAttack = unit.Attack;

        ShowAttackRange(unitGridPos, unitAttackRange);
    }

    private void Player_OnUnitAttackComplete(object sender, System.EventArgs e) {
        GameObject enemyUnit = MapManager.Instance.GetUnit(CursorInput.Instance.GetCursorPosition());
        enemyUnit.GetComponent<Unit>().ModifyHealth(-unitAttack);
        ClearAttackRange();
        Player.Instance.SetTurnAction(Player.TurnAction.Nothing);
    }

    private void Player_OnUnitAttackCancelled(object sender, System.EventArgs e) {
        ClearAttackRange();
    }

    private void ShowAttackRange(Vector2 unitGridPos, int range) {
        // BFS over range
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();

        queue.Enqueue(unitGridPos);
        visited.Add(unitGridPos);

        while (queue.Count > 0) {
            Vector2 current = queue.Dequeue();

            if (MapManager.Instance.GetManhattanDistance(current, unitGridPos) > range) {
                continue;
            }

            if (!(MapManager.Instance.GetManhattanDistance(current, unitGridPos) == 0)) {
                GameObject tile = MapManager.Instance.gridArray[(int)current.x, (int)current.y].tileGameObject;
                TileOverlays tileOverlays = tile.GetComponent<TileOverlays>();
                tileOverlays.ShowAttackTile();
                inAttackRangeSet.Add(tile);
            }

            foreach (Vector2 neighbor in BreadthFirstSearch.GetValidNeighbors(current, false)) {
                if (!visited.Contains(neighbor)) {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // Set CursorInput isAttacking
        CursorInput.Instance.ToggleIsAttacking(inAttackRangeSet);
        CursorInput.Instance.ToggleCursor(0);
    }

    private void ClearAttackRange() {
        foreach(GameObject tile in inAttackRangeSet) {
            tile.GetComponent<TileOverlays>().HideAttackTile();
        }
        inAttackRangeSet.Clear();
        CursorInput.Instance.ToggleCursor(0);
        CursorInput.Instance.ToggleIsAttacking(inAttackRangeSet);
    }

    public HashSet<GameObject> GetInAttackRangeSet() {
        return inAttackRangeSet;
    }
}
