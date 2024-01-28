using System;
using UnityEngine;

namespace GameFunctions.Sample {

    [Serializable]
    internal class GeneratorSetting {
        public int seed;
        public int seedTimes;
        public int width;
        public int height;
        public float cellSize;
        public float cellGap;
        public int seaCount;
        [Range(0, 4)]
        public int sea_fromDir;
    }

    public class Sample_GFCellGenerator : MonoBehaviour {

        int[] cells;
        [SerializeField] GeneratorSetting setting;

        bool isGenerated;

        const int VALUE_EMPTY = 0;
        const int VALUE_LAND = 1;
        const int VALUE_SEA = 2;

        [SerializeField] Color color_empty;
        [SerializeField] Color color_land;
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

        void Update() {
            if (this.gameObject.transform.hasChanged) {
                cells = null;
                isGenerated = false;
                Gen();
            }
        }

        void Gen() {
            System.Random rd = new System.Random(setting.seed);
            for (int i = 0; i < setting.seedTimes; i++) {
                rd.Next();
            }
            cells = GFCellGenerator.NewCells(setting.width, setting.height, VALUE_LAND);
            GFCellGenerator.Gen_Sea(cells, rd, VALUE_SEA, setting.width, setting.seaCount, setting.sea_fromDir);
            isGenerated = true;
        }

        void OnDrawGizmos() {
            if (cells == null || cells.Length == 0) {
                return;
            }

            // Draw cells
            for (int i = 0; i < cells.Length; i++) {
                int x = i % setting.width;
                int y = i / setting.width;
                int value = cells[i];
                if (value == VALUE_EMPTY) {
                    Gizmos.color = color_empty;
                } else if (value == VALUE_LAND) {
                    Gizmos.color = color_land;
                } else if (value == VALUE_SEA) {
                    Gizmos.color = color_sea;
                } else {
                    // Error
                    Gizmos.color = Color.red;
                }
                // Draw cell
                Gizmos.DrawCube(new Vector3(x * (setting.cellSize + setting.cellGap), y * (setting.cellSize + setting.cellGap)), new Vector3(setting.cellSize, setting.cellSize));
            }

        }

    }

}