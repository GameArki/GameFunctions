using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_Hex : MonoBehaviour {

        public int width;
        public int height;
        public float outterRadius;
        public float gap;
        Vector2Int[] hexes;

        [ContextMenu("Generate Hexes")]
        public void Generate() {
            hexes = new Vector2Int[width * height];
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    hexes[y * width + x] = new Vector2Int(x, y);
                }
            }
        }

        // Start is called before the first frame update
        void Start() {
            Generate();
        }

        void OnGUI() {
            Vector2 mousePos = Input.mousePosition;
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2Int logicPos = GFHex.RenderPosToLogicPos(worldPos, outterRadius, gap);
            GUILayout.Label($"Mouse Pos: {logicPos}");

            if (hexes == null) {
                return;
            }
            // Show Label: pos
            for (int i = 0; i < hexes.Length; i++) {
                Vector2Int cur = hexes[i];
                Vector2 center = GFHex.Render_GetCenterPos(cur, outterRadius, gap);
                center.x -= outterRadius * 0.5f;
                Vector2 screenCenter = Camera.main.WorldToScreenPoint(center);
                GUI.color = Color.white;
                GUI.Label(new Rect(screenCenter.x, Screen.height - screenCenter.y, 30, 20), $"{cur.x},{cur.y}");
            }

        }

        // Update is called once per frame
        void Update() {

        }

        void OnDrawGizmos() {
            if (hexes == null) {
                return;
            }
            Gizmos.color = Color.red;
            for (int i = 0; i < hexes.Length; i++) {
                GFHex.DrawGizmos(hexes[i], outterRadius, gap);
            }
        }

    }

}
