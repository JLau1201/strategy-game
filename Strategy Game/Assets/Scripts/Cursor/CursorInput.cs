using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorInput : MonoBehaviour
{
    public static CursorInput Instance { get; private set; }

    [Header("Layer Masks")]
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask unitMask;

    [SerializeField] private Grid grid;

    [SerializeField] private GameObject baseCursor;
    [SerializeField] private Transform abilityCursor;
    [SerializeField] private Image abilityCursorImage;

    private Vector3 cursorToMapPos;
    private bool canMoveCursor = true;
    private bool isAttacking = false;
    private HashSet<Vector3Int> inAttackRangeSet = new HashSet<Vector3Int>();

    // For moving cursor with mouse
    private bool isUsingMouse = true;
    private float moveDelay = .1f;
    private float lastMoveTime = 0;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        GameInputManager.Instance.OnToggleCursorInput += GameInputManager_OnToggleCursorInput;
    }

    private void GameInputManager_OnToggleCursorInput(object sender, System.EventArgs e) {
        isUsingMouse = !isUsingMouse;
    }

    private void Update() {
        lastMoveTime -= Time.deltaTime;
        if (canMoveCursor) {
            if (isUsingMouse) {
                ControlCursorWithMouse();
            } else {
                ControlCursorWithKeys();
            }
        }
    }

    private void ControlCursorWithMouse() {
        GetCursorToMapPosition();

        if (isAttacking) {
            Vector3Int gridPos = grid.WorldToCell(cursorToMapPos);
            if (inAttackRangeSet.Contains(gridPos)) {
                transform.position = grid.CellToWorld(gridPos);
            }
        } else {
            // Set the cell indicator position to the cell on the grid
            Vector3Int gridPos = grid.WorldToCell(cursorToMapPos);
            transform.position = grid.CellToWorld(gridPos);
        }
    }

    private void ControlCursorWithKeys() {
        Vector2 moveInput = GameInputManager.Instance.GetCursorInput();
        if (moveInput == Vector2.zero) {
            lastMoveTime = 0;
            return;
        }
        if (lastMoveTime > 0) return;
        Vector2 direction = UnnormalizeInput(moveInput);
        Vector3 nextPos = transform.position + new Vector3(direction.x, 0f, direction.y);

        Vector3Int gridPos = grid.WorldToCell(nextPos);
        transform.position = grid.CellToWorld(gridPos);

        lastMoveTime = moveDelay;
    }

    private Vector2 UnnormalizeInput(Vector2 input) {
        Vector2 direction = new Vector2(
            input.x != 0 ? Mathf.Sign(input.x) : 0,
            input.y != 0 ? Mathf.Sign(input.y) : 0
        );

        return direction;
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
    
    public Vector3 GetCursorPosition() {
        return transform.position;
    }

    // Get the manhattan distance from target to the cursor
    public int GetDistanceToCursor(Vector2 targetPos) {
        Vector2 cursorPos = new Vector2(transform.position.x, transform.position.z);
        
        Vector3Int targetGridPos = grid.WorldToCell(new Vector3(targetPos.x, 0f, targetPos.y));
        targetPos = new Vector2(grid.CellToWorld(targetGridPos).x, grid.CellToWorld(targetGridPos).z);

        return (int) (Mathf.Abs(cursorPos.x - targetPos.x) + Mathf.Abs(cursorPos.y - targetPos.y));
    }

    public void SetCanMoveCursor(bool enabled) {
        canMoveCursor = enabled;
    }

    // When attacking, cursor can only be within attack range
    public void ToggleIsAttacking(HashSet<GameObject> inAttackRangeSet) {
        isAttacking = !isAttacking;
        this.inAttackRangeSet.Clear();

        if (isAttacking) {
            foreach (GameObject tile in inAttackRangeSet) {
                this.inAttackRangeSet.Add(new Vector3Int((int)Mathf.Floor(tile.transform.position.x), (int)Mathf.Floor(tile.transform.position.z)));
            }
        }
    }

    // Change cursor type to attack cursor
    public void ToggleCursor(int abilityRadius) {
        baseCursor.SetActive(!baseCursor.activeSelf);

        if (!baseCursor.activeSelf) {
            // BFS over range
            Queue<Vector2> queue = new Queue<Vector2>();
            HashSet<Vector2> visited = new HashSet<Vector2>();

            queue.Enqueue(Vector2.zero);
            visited.Add(Vector2.zero);

            while (queue.Count > 0) {
                Vector2 current = queue.Dequeue();

                if (MapManager.Instance.GetManhattanDistance(current, Vector2.zero) > abilityRadius) {
                    continue;
                }

                Image abilityTile = Instantiate(abilityCursorImage, abilityCursor);
                abilityTile.rectTransform.localPosition = current;
                
                foreach (Vector2 dir in BreadthFirstSearch.dirOffsets) {
                    Vector2 neighbor = current + dir;
                    if (!visited.Contains(neighbor)) {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        } else {
            foreach(Transform abilityTile in abilityCursor) {
                Destroy(abilityTile.gameObject);
            }
        }
    }


    // While Ability cursor is active
    // Get all targets based off layermask 
    public List<GameObject> GetAbilityTargets(LayerMask targetMask) {
        List<GameObject> abilityTargets = new List<GameObject>();

        foreach (Transform abilityTile in abilityCursor) {
            Vector3 rayOrigin = new Vector3(abilityTile.position.x, -5f, abilityTile.position.z);
            float maxRayDist = 20f;

            RaycastHit hit;
            if(Physics.Raycast(rayOrigin, Vector3.up, out hit, maxRayDist, targetMask)) {
                abilityTargets.Add(hit.transform.gameObject);
            }
        }

        return abilityTargets;
    }
}
