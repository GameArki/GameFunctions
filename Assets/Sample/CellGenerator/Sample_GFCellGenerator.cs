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

        bool isGenerated;

        void OnGUI() {

            seed = EditorGUILayout.IntField("Seed", seed);
            seedTimes = EditorGUILayout.IntField("SeedTimes", seedTimes);
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            cellSize = EditorGUILayout.Slider(cellSize, 0, 1);
            cellGap = EditorGUILayout.Slider(cellGap, 0, 0.2f);
            seaCount = EditorGUILayout.IntField("SeaCount", seaCount);

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
            GFCellGenerator.Gen_Sea(cells, rd, width, seaCount, 0, 0, 0, 0, 0);
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
                if (cells[i] == 0) {
                    Gizmos.color = Color.black;
                }
                // Draw cell
                Gizmos.DrawCube(new Vector3(x * (cellSize + cellGap), y * (cellSize + cellGap)), new Vector3(cellSize, cellSize));
            }

        }

    }

}