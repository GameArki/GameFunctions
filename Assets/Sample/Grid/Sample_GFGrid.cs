using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_ : MonoBehaviour {

        Vector2Int[] results = new Vector2Int[GFGrid.RectCycle_GetCellCount(10)];
        int resultCount;
        int resultIndex;

        int cycle = 0;
        int lastCycle = 0;

        void Start() {

        }

        void Update() {
            if (cycle != lastCycle) {
                lastCycle = cycle;
                resultCount = GFGrid.RectCycle_GetCellsBySpirals(new Vector2Int(0, 0), cycle, results);
                resultIndex = 0;
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                if (resultIndex < resultCount - 1) {
                    resultIndex++;
                }
            }
            if (Input.GetKeyUp(KeyCode.W)) {
                cycle++;
            }
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            for (int i = 0; i < resultCount; i++) {
                Vector2Int cell = results[i];
                Gizmos.DrawWireCube(new Vector3(cell.x, cell.y), Vector3.one);
            }
            Gizmos.color = Color.green;
            if (resultIndex < resultCount) {
                Vector2Int cell = results[resultIndex];
                Gizmos.DrawCube(new Vector3(cell.x, cell.y), Vector3.one);
            }
        }

    }

}
