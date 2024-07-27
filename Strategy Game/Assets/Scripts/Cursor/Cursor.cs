using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cursor : MonoBehaviour
{
    public static Cursor instance { get; private set; }

    [Header("Layer Masks")]
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask unitMask;

    [SerializeField] private Grid grid;

    private Vector3 cursorToMapPos;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // Get where the mouse is on the ground plane
        GetCursorToMapPosition();

        // Set the cell indicator position to the cell on the grid
        Vector3Int gridPos = grid.WorldToCell(cursorToMapPos);
        transform.position = grid.CellToWorld(gridPos);
    }

    // Snap the cursor position to the grid
    private void GetCursorToMapPosition() {
        Vector3 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // Raycast to the ground plane
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask)) {
            cursorToMapPos = hit.point;
        }
    }

    // Get the tile under the cursor
    public GameObject GetTileOnCursor() {
        RaycastHit hit;
        float maxDist = 5f;
        Vector3 positionOffset = new Vector3(transform.position.x + .5f, transform.position.y + 2, transform.position.z + .5f);
        if (Physics.Raycast(positionOffset, Vector3.down, out hit, maxDist, tileMask)) {
            return hit.transform.gameObject;
        }

        return null;
    }

    // Get the Unit above the cursor
    public GameObject GetUnitOnCursor() {
        RaycastHit hit;
        float maxDist = 5f;
        Vector3 positionOffset = new Vector3(transform.position.x + .5f, transform.position.y + 3, transform.position.z + .5f);
        if (Physics.Raycast(positionOffset, Vector3.down, out hit, maxDist, unitMask)) {
            return hit.transform.gameObject;
        }

        return null;
    }

    // Get the manhattan distance from target to the cursor
    public int GetDistanceToCursor(Vector2 targetPos) {
        Vector2 cursorPos = new Vector2(transform.position.x, transform.position.z);
        
        Vector3Int targetGridPos = grid.WorldToCell(new Vector3(targetPos.x, 0f, targetPos.y));
        targetPos = new Vector2(grid.CellToWorld(targetGridPos).x, grid.CellToWorld(targetGridPos).z);

        return (int) (Mathf.Abs(cursorPos.x - targetPos.x) + Mathf.Abs(cursorPos.y - targetPos.y));
    }
}
