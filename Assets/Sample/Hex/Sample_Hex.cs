using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_Hex : MonoBehaviour {

        public int width;
        public int height;
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

        // Update is called once per frame
        void Update() {

        }

        void OnDrawGizmos() {
            if (hexes == null) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < hexes.Length; i++) {
                GFHex.DrawGizmos(hexes[i], 1f, 0);
            }
        }

    }

}
