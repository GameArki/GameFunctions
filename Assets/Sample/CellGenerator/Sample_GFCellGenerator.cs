using System;
using UnityEngine;

namespace GameFunctions.Sample {

    [Serializable]
    internal class GeneratorSetting {
        public float cellSize;
        public float cellGap;
        public GFGenCellOption cellOption;
        public GFGenSeaOption seaOption;
    }

    public class Sample_GFCellGenerator : MonoBehaviour {

        int[] cells;
        [SerializeField] GeneratorSetting setting;

        bool isGenerated;

        const int VALUE_EMPTY = 0;
        const int VALUE_LAND_GRASS = 1;
        const int VALUE_SEA = 2;

        [SerializeField] Color color_empty;
        [SerializeField] Color color_land_grass;
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

            cells = GFCellGenerator.GenAll(setting.cellOption, setting.seaOption);

            isGenerated = true;

        }

        void OnDrawGizmos() {
            if (cells == null || cells.Length == 0) {
                return;
            }

            // Draw cells
            int width = setting.cellOption.width;
            float cellSize = setting.cellSize;
            float cellGap = setting.cellGap;
            for (int i = 0; i < cells.Length; i++) {
                int x = i % width;
                int y = i / width;
                int value = cells[i];
                if (value == VALUE_EMPTY) {
                    Gizmos.color = color_empty;
                } else if (value == VALUE_LAND_GRASS) {
                    Gizmos.color = color_land_grass;
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