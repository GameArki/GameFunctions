using System;
using UnityEngine;

namespace GameFunctions.Sample {

    [Serializable]
    internal class GeneratorSetting {
        public float cellSize;
        public float cellGap;
        public GFGenGridOption gridOption;
        public GFGenSeaOption seaOption;
        public GFGenLakeOption lakeOption;
    }

    public class Sample_GFGridGenerator : MonoBehaviour {

        int[] cells;
        [SerializeField] GeneratorSetting setting;

        const int VALUE_EMPTY = 0;
        const int VALUE_LAND_GRASS = 1;
        const int VALUE_SEA = 2;
        const int VALUE_LAKE = 3;

        [SerializeField] Color color_empty;
        [SerializeField] Color color_land_grass;
        [SerializeField] Color color_land_lake;
        [SerializeField] Color color_sea;

        void Update() {
            if (this.gameObject.transform.hasChanged) {
                Gen();
            }
        }

        void Gen() {
            var ctx = GFGridGenerator.GenAll(setting.gridOption, setting.seaOption, setting.lakeOption);
            cells = ctx.grid;
        }

        void OnDrawGizmos() {
            if (cells == null || cells.Length == 0) {
                return;
            }

            // Draw cells
            int width = setting.gridOption.width;
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
                } else if (value == VALUE_LAKE) {
                    Gizmos.color = color_land_lake;
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