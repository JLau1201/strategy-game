using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BreadthFirstSearch 
{
    private static Vector2[] dirOffsets = {
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
    };

    public static List<GameObject> BFS(GameObject[,] gridArray, GameObject startTile, GameObject goalTile) {
        // Get the start and goal positions with respect to the grid
        Vector2 startPos = MapManager.instance.GetTileToGridPosition(startTile);
        Vector2 goalPos = MapManager.instance.GetTileToGridPosition(goalTile);

        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();

        queue.Enqueue(startPos);
        visited.Add(startPos);
        cameFrom[startPos] = startPos;

        while (queue.Count > 0) {
            Vector2 current = queue.Dequeue();

            // Found goal
            if(current == goalPos) {
                return ReconstructPath(gridArray, cameFrom, startPos, goalPos);
            }

            // Iterate through all neighbors
            foreach(Vector2 neighbor in GetNeighbors(current)) {
                if (!visited.Contains(neighbor)) {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }

        }
        
        // Path not found
        return null;
    }
    

    private static List<Vector2> GetNeighbors(Vector2 gridPos) {
        List<Vector2> neighbors = new List<Vector2>();

        // Get all possible neighbors
        foreach (Vector2 dir in dirOffsets) {
            Vector2 neighbor = gridPos + dir;
            if (MapManager.instance.IsValidGridPosition(neighbor)) {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // Helper method to reconstruct the path from the cameFrom dictionary
    private static List<GameObject> ReconstructPath(GameObject[,] gridArray, Dictionary<Vector2, Vector2> cameFrom, Vector2 startPos, Vector2 goalPos) {
        List<GameObject> path = new List<GameObject>();
        Vector2 current = goalPos;

        while (!current.Equals(startPos)) {
            path.Add(gridArray[(int)current.x, (int)current.y]);
            current = cameFrom[current];
        }

        path.Add(gridArray[(int)startPos.x, (int)startPos.y]);
        path.Reverse();

        return path;
    }
}
