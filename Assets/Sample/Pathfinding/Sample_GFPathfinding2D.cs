using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFunctions.PathfindingInternal;

namespace GameFunctions.Sample {

    public class Sample_GFPathfinding2D : MonoBehaviour {

        HashSet<I32I32_U64> blockSet = new HashSet<I32I32_U64>();
        Vector2Int startPos;
        Vector2Int endPos;
        Vector2Int curPos;

        Vector2Int[] result = new Vector2Int[10000];
        int resultCount = 0;

        int visited = 0;

        void Start() {
            GFPathfinding2D.openSet?.Clear();
            GFPathfinding2D.closedSet?.Clear();
        }

        void Update() {
            Vector2Int mouseGridPos = MouseGridPos();
            if (Input.GetMouseButton(0)) {
                blockSet.Add(new I32I32_U64(mouseGridPos));
            } else if (Input.GetMouseButtonUp(1)) {
                startPos = mouseGridPos;
            } else if (Input.GetMouseButtonUp(2)) {
                endPos = mouseGridPos;
                visited = 0;
                resultCount = GFPathfinding2D.AStar(startPos, endPos, 5000, (pos) => {
                    return !blockSet.Contains(new I32I32_U64(pos));
                }, result);
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                bool hasResult = GFPathfinding2D.ManualProcess(ref visited, ref resultCount, 5000, startPos, endPos, (pos) => {
                    return !blockSet.Contains(new I32I32_U64(pos));
                }, result, out curPos);
                if (hasResult) {
                    Debug.Log("Res: " + resultCount);
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
            foreach (I32I32_U64 pos in blockSet) {
                Gizmos.DrawCube(new Vector3(pos.i32_0, pos.i32_1, 0), Vector3.one);
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

            // Draw End
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(new Vector3(endPos.x, endPos.y, 0), Vector3.one);

            // Draw Cur
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(curPos.x, curPos.y, 0), Vector3.one);

        }

    }
}
