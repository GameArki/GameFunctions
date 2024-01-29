using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_ : MonoBehaviour {

        Vector2Int[] results = new Vector2Int[100000];
        int resultCount;
        int resultIndex;

        public int mode;

        void Start() {
            results = new Vector2Int[100000];
        }

        void Update() {
            if (mode == 0) {
                RectCycle_Update();
            } else if (mode == 1) {
                Slant_Update();
            }
        }

        void OnDrawGizmos() {
            if (mode == 0) {
                RectCycle_Draw();
            } else if (mode == 1) {
                Slant_Draw();
            }
        }

        // ==== RectCycle ====
        int rectCycle_index = 0;
        int rectCycle_lastIndex = 0;
        void RectCycle_Update() {
            if (rectCycle_index != rectCycle_lastIndex) {
                rectCycle_lastIndex = rectCycle_index;
                resultCount = GFGrid.RectCycle_GetCellsBySpirals(new Vector2Int(0, 0), rectCycle_index, results);
                resultIndex = 0;
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                if (resultIndex < resultCount - 1) {
                    resultIndex++;
                }
            }
            if (Input.GetKeyUp(KeyCode.W)) {
                rectCycle_index++;
            }
        }

        void RectCycle_Draw() {
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

        // ==== Slant ====
        [SerializeField] GameObject slant_start;
        [SerializeField] GameObject slant_end;
        void Slant_Update() {
            if (transform.hasChanged) {
                Vector2 start = slant_start.transform.position;
                Vector2 end = slant_end.transform.position;
                resultCount = GFGrid.Slant_GetCells(start, end, results);
            }
        }

        void Slant_Draw() {

            Gizmos.color = Color.green;
            for (int i = 0; i < resultCount; i++) {
                Vector2Int cell = results[i];
                Gizmos.DrawWireCube(new Vector3(cell.x, cell.y), Vector3.one);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine((Vector2)slant_start.transform.position, (Vector2)slant_end.transform.position);

        }

    }

}
