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

        void OnGUI() {
            if (mode == 0) {

            } else if (mode == 1) {
                GUILayout.Label("CircleCycle Radius: " + circleCycle_radius);
            } else if (mode == 2) {

            } else if (mode == 3) {
                GUILayout.Label("CrossCycle Radius: " + crossCycle_radius);
            }
        }

        void Update() {
            if (mode == 0) {
                RectCycle_Update();
            } else if (mode == 1) {
                CircleCycle_Update();
            } else if (mode == 2) {
                Slant_Update();
            } else if (mode == 3) {
                CrossCycle_Update();
            }
        }

        void OnDrawGizmos() {
            if (mode == 0) {
                RectCycle_Draw();
            } else if (mode == 1) {
                CircleCycle_Draw();
            } else if (mode == 2) {
                Slant_Draw();
            } else if (mode == 3) {
                CrossCycle_Draw();
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

        // ==== CircleCycle ====
        [SerializeField] float circleCycle_radius = 0;
        float circleCycle_lastRadius = 0;
        void CircleCycle_Update() {
            if (Input.GetKeyUp(KeyCode.W)) {
                circleCycle_radius += 0.5f;
            } else if (Input.GetKeyUp(KeyCode.S)) {
                circleCycle_radius -= 0.5f;
            }
            if (circleCycle_lastRadius != circleCycle_radius) {
                resultCount = GFGrid.CircleCycle_GetCells(new Vector2Int(0, 0), circleCycle_radius, results);
                circleCycle_lastRadius = circleCycle_radius;
            }
        }

        void CircleCycle_Draw() {
            Gizmos.color = Color.red;
            for (int i = 0; i < resultCount; i++) {
                Vector2Int cell = results[i];
                Gizmos.DrawWireCube(new Vector3(cell.x, cell.y), Vector3.one);
            }
        }

        // ==== CrossCycle ====
        [SerializeField] int crossCycle_radius = 0;
        int crossCycle_lastRadius = 0;
        void CrossCycle_Update() {
            if (Input.GetKeyUp(KeyCode.W)) {
                crossCycle_radius++;
            } else if (Input.GetKeyUp(KeyCode.S)) {
                crossCycle_radius--;
            }
            if (crossCycle_lastRadius != crossCycle_radius) {
                resultCount = GFGrid.CrossCycle_GetCells(new Vector2Int(0, 0), crossCycle_radius, results);
                crossCycle_lastRadius = crossCycle_radius;
            }
        }

        void CrossCycle_Draw() {
            Gizmos.color = Color.red;
            for (int i = 0; i < resultCount; i++) {
                Vector2Int cell = results[i];
                Gizmos.DrawWireCube(new Vector3(cell.x, cell.y), Vector3.one);
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
