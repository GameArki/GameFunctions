using UnityEngine;
using UnityEditor;

namespace GameFunctions.Sample {

    public class Sample_GFCellGenerator : MonoBehaviour {

        int[] cells;
        public int seed;
        public int seedTimes;
        public int width;
        public int height;
        public float cellSize;
        public float cellGap;
        public int seaCount;
        [Range(0, 4)]
        public int sea_fromDir;

        bool isGenerated;

        const int VALUE_EMPTY = 0;
        const int VALUE_SEA = 1;

        [SerializeField] Color color_empty;
        [SerializeField] Color color_sea;

        void OnGUI() {
            if (isGenerated) {
                if (GUILayout.Button("Clear")) {
                    cells = null;
                    isGenerated = false;
                }
            } else {
                if (GUILayout.Button("Gen")) {
                    Gen();
                }
            }
        }

        void Gen() {
            System.Random rd = new System.Random(seed);
            for (int i = 0; i < seedTimes; i++) {
                rd.Next();
            }
            cells = GFCellGenerator.NewCells(width, height);
            GFCellGenerator.Gen_Sea(cells, rd, VALUE_SEA, width, seaCount, sea_fromDir);
            isGenerated = true;
        }

        void OnDrawGizmos() {
            if (cells == null || cells.Length == 0) {
                return;
            }

            // Draw cells
            for (int i = 0; i < cells.Length; i++) {
                int x = i % width;
                int y = i / width;
                int value = cells[i];
                if (value == VALUE_EMPTY) {
                    Gizmos.color = color_empty;
                } else if (value == VALUE_SEA) {
                    Gizmos.color = color_sea;
                } else {
                    // Error
                    Gizmos.color = Color.red;
                }
                // Draw cell
                Gizmos.DrawCube(new Vector3(x * (cellSize + cellGap), y * (cellSize + cellGap)), new Vector3(cellSize, cellSize));
            }

        }

    }

}