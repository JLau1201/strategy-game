using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAbilityManager : MonoBehaviour
{

    private HashSet<GameObject> inAbilityRange = new HashSet<GameObject>();

    private GameObject lastTileHovered;

    private Vector2 unitGridPos;
    private GameObject startTile;
    private GameObject selectedUnit;
    private Unit unit;
    private int unitAbilityRange;
    private int unitAbilityRadius;
    private float unitAttack;

    private void Start() {
        Player.Instance.OnUnitAbilityStart += Player_OnUnitAbilityStart;
        Player.Instance.OnUnitAbilityComplete += Player_OnUnitAbilityComplete;
        Player.Instance.OnUnitAbilityCancelled += Player_OnUnitAbilityCancelled;
    }
    private void Player_OnUnitAbilityStart(object sender, Player.OnUnitSelectedEventArgs e) {
        selectedUnit = e.selectedUnit;
        startTile = MapManager.Instance.GetTile(selectedUnit.transform.position);
        unitGridPos = MapManager.Instance.GetTileToGridPosition(startTile);

        unit = selectedUnit.GetComponent<Unit>();
        unitAbilityRange = unit.AbilityRange;
        unitAbilityRadius = unit.AbilityRadius;
        unitAttack = unit.Attack;

        lastTileHovered = startTile;

        ShowAbilityRangeCircle(unitGridPos, unitAbilityRange);
        CursorInput.Instance.ToggleCursor(unitAbilityRadius);
    }

    private void Player_OnUnitAbilityComplete(object sender, System.EventArgs e) {


        ClearAbilityRange();
        Player.Instance.SetTurnAction(Player.TurnAction.Nothing);
    }

    private void Player_OnUnitAbilityCancelled(object sender, System.EventArgs e) {
        ClearAbilityRange();
    }

    private void ShowAbilityRangeCircle(Vector2 unitGridPos, int range) {
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

            GameObject tile = MapManager.Instance.gridArray[(int)current.x, (int)current.y].tileGameObject;
            TileOverlays tileOverlays = tile.GetComponent<TileOverlays>();
            tileOverlays.ShowAttackTile();
            inAbilityRange.Add(tile);

            foreach (Vector2 neighbor in BreadthFirstSearch.GetValidNeighbors(current, false)) {
                if (!visited.Contains(neighbor)) {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }
        CursorInput.Instance.ToggleIsAttacking(inAbilityRange);
    }

    private void ClearAbilityRange() {
        foreach (GameObject tile in inAbilityRange) {
            tile.GetComponent<TileOverlays>().HideAttackTile();
        }
        inAbilityRange.Clear();
        CursorInput.Instance.ToggleCursor(0);
        CursorInput.Instance.ToggleIsAttacking(inAbilityRange);
    }
}
