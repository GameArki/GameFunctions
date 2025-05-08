using System;
using System.Collections.Generic;
using UnityEngine;
using GameClasses.CellBakerLib;

namespace GameClasses.Sample {

    public class CellBakerSample : MonoBehaviour {

        CellBaker baker;

        const int type_sea = 1;
        public int width;
        public int height;

        Dictionary<int, Color> colorMap = new Dictionary<int, Color>() {
            { type_sea, new Color32(0, 0, 80, 255) },
        };

        void Awake() {

            baker = new CellBaker(1010, width, height);
            baker.Fill(type_sea);

        }

        void OnDrawGizmos() {
            if (baker == null) {
                return;
            }

            var cells = baker.GetCells();
            if (cells == null || cells.Length == 0) {
                return;
            }

            // Draw cells
            int width = baker.width;
            int height = baker.height;
            float cellRenderSize = 1f;
            for (int i = 0; i < cells.Length; i++) {
                int x = i % width;
                int y = i / width;
                int value = cells[i];
                if (colorMap.TryGetValue(value, out Color color)) {
                    Gizmos.color = color;
                } else {
                    Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(new Vector3(x * cellRenderSize, y * cellRenderSize, 0), new Vector3(cellRenderSize, cellRenderSize, cellRenderSize));
            }
        }

    }

}