using System;
using UnityEngine;
using GameFunctions.GridGeneratorInternal;

namespace GameFunctions.Sample {

    [Serializable]
    internal class GeneratorSetting {
        public float cellSize;
        public float cellGap;
        public GridGenGridOption gridOption;
        public GridGenAreaOption[] areaOptions;
    }

    public class Sample_GFGridGenerator : MonoBehaviour {

        int[] cells;
        [SerializeField] GeneratorSetting setting;

        const int VALUE_EMPTY = 0;
        const int VALUE_LAND_GRASS = 1;
        const int VALUE_SEA = 2;
        const int VALUE_LAKE = 3;
        const int VALUE_FOREST = 4;
        const int VALUE_FLOWER = 5;

        [SerializeField] Color color_empty;
        [SerializeField] Color color_land_grass;
        [SerializeField] Color color_land_lake;
        [SerializeField] Color color_sea;
        [SerializeField] Color color_forest;
        [SerializeField] Color color_flower;

        void Update() {
            if (this.gameObject.transform.hasChanged) {
                Gen();
            }
        }

        void Gen() {
            cells = GFGridGenerator.GenAll(setting.gridOption, setting.areaOptions);
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
                } else if (value == VALUE_FOREST) {
                    Gizmos.color = color_forest;
                } else if (value == VALUE_FLOWER) {
                    Gizmos.color = color_flower;
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