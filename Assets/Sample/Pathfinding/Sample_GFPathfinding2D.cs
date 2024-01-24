using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_GFPathfinding2D : MonoBehaviour {

        HashSet<Vector2Int> blockSet = new HashSet<Vector2Int>();
        Vector2Int startPos;

        Vector2Int[] result = new Vector2Int[10000];
        int resultCount = 0;

        void Start() {

        }

        void Update() {
            Vector2Int mouseGridPos = MouseGridPos();
            if (Input.GetMouseButton(0)) {
                blockSet.Add(mouseGridPos);
            } else if (Input.GetMouseButtonUp(1)) {
                startPos = mouseGridPos;
            } else if (Input.GetMouseButtonUp(2)) {
                Vector2Int endPos = mouseGridPos;
                resultCount = GFPathfinding2D.AStar(startPos, endPos, 20000, (pos) => {
                    return !blockSet.Contains(pos);
                }, result);
                if (resultCount < 0) {
                    Debug.LogError("No Result" + resultCount);
                }
            }
        }

        Vector2Int MouseGridPos() {
            Vector2 mouseScreenPos = Input.mousePosition;
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2Int mouseGridPos = new Vector2Int(Mathf.RoundToInt(mouseWorldPos.x), Mathf.RoundToInt(mouseWorldPos.y));
            return mouseGridPos;
        }

        void OnDrawGizmos() {

            // Draw Block
            Gizmos.color = Color.red;
            foreach (Vector2Int pos in blockSet) {
                Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0), Vector3.one);
            }

            // Draw CloseSet
            Gizmos.color = Color.black;
            if (GFPathfinding2D.closedSet != null) {
                foreach (var cell in GFPathfinding2D.closedSet) {
                    Gizmos.DrawCube(new Vector3(cell.pos.x, cell.pos.y, 0), Vector3.one);
                }
            }

            // Draw Open
            Gizmos.color = Color.yellow;
            if (GFPathfinding2D.openSet != null) {
                foreach (var cell in GFPathfinding2D.openSet) {
                    Gizmos.DrawCube(new Vector3(cell.pos.x, cell.pos.y, 0), Vector3.one);
                }
            }

            // Draw Result
            Gizmos.color = Color.blue;
            for (int i = 0; i < resultCount; i++) {
                Vector2Int pos = result[i];
                if (pos == Vector2Int.zero) {
                    break;
                }
                Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0), Vector3.one);
            }

            // Draw Start
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(startPos.x, startPos.y, 0), Vector3.one);

        }

    }
}
