using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_GFGridHashmap : MonoBehaviour {

        GFGridHashmap<int> gridHashmap;
        int searchRadius;

        GFGridHashmapResult<int>[] tmpResults = new GFGridHashmapResult<int>[10000];

        void Start() {
            gridHashmap = new GFGridHashmap<int>(new Vector2Int(24, 16), 100, true, 10);
            searchRadius = 1;
        }

        void OnGUI() {
            if (gridHashmap == null) {
                return;
            }

            GUILayout.Label("Help: LeftClick: Add, RightClick: Detect Big, MiddleClick: Detect Small");
            GUILayout.Label("\t Scroll: Scale Size, W/S: SearchType");
            GUILayout.Label("Big Size: " + gridHashmap.BigMapSize);
            GUILayout.Label("Small Count: " + gridHashmap.SmallCount());
            Vector2Int cursorPos = CursorPos();
            GUILayout.Label("Cursor: " + cursorPos);
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                gridHashmap.Add(CursorPos(), 1);
            }
            if (Input.GetMouseButtonDown(1)) {
                gridHashmap.Remove(CursorPos(), 1);
            }

            if (Input.mouseScrollDelta.y > 0) {
                searchRadius++;
            } else if (Input.mouseScrollDelta.y < 0) {
                searchRadius--;
            }

        }

        Vector2Int CursorPos() {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2Int((int)mousePos.x, (int)mousePos.y);
        }

        void OnDrawGizmos() {

            if (gridHashmap == null) {
                return;
            }

            Vector2Int cursorPos = CursorPos();
            Vector2Int bigSize = gridHashmap.BigMapSize;
            Vector3 halfSize = new Vector2(bigSize.x * 0.5f, bigSize.y * 0.5f);

            // Draw Cursor
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(cursorPos.x, cursorPos.y, 0), Vector3.one * searchRadius);

            // Draw Big Grid
            Gizmos.color = Color.white;
            foreach (var key in gridHashmap.BigMapKeys) {
                Gizmos.DrawWireCube(new Vector3(key.x * bigSize.x, key.y * bigSize.y, 0) + halfSize, new Vector3(bigSize.x, bigSize.y, 0));
            }

            // Draw Small Grid
            foreach (var key in gridHashmap.SmallMapKeys) {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(key.x, key.y, 0), Vector3.one);
            }

            // Draw Overlap
            int resultLen = gridHashmap.OverlapAABB(cursorPos, searchRadius, tmpResults);
            for (int i = 0; i < resultLen; i++) {
                GFGridHashmapResult<int> result = tmpResults[i];
                Vector2Int smallPos = result.posKey;
                Vector2Int bigPos = gridHashmap.GetBigKey(smallPos);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(new Vector3(bigPos.x * bigSize.x, bigPos.y * bigSize.y, 0) + halfSize, new Vector3(bigSize.x, bigSize.y, 0));

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(new Vector3(smallPos.x, smallPos.y, 0), Vector3.one);
            }

        }

        void DrawCube(Vector3 pos, Vector3 size) {
            Gizmos.DrawWireCube(pos + size / 2, new Vector3(size.x, size.y, 0));
        }

    }

}
