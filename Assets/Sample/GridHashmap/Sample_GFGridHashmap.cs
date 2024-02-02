using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_GFGridHashmap : MonoBehaviour {

        GFGridHashmap<int> gridHashmap;
        Vector2Int[] tempCells;

        int mode;
        GFGridSearchType searchType;
        int searchSize;

        void Start() {
            gridHashmap = new GFGridHashmap<int>(new Vector2Int(24, 16), 100, true, 10);
            tempCells = new Vector2Int[gridHashmap.BigMapSize.x * gridHashmap.BigMapSize.y * 100];
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
                mode = 1;
            }
            if (Input.GetMouseButtonDown(2)) {
                mode = 2;
            }

            if (Input.mouseScrollDelta.y > 0) {
                searchSize++;
            } else if (Input.mouseScrollDelta.y < 0) {
                searchSize--;
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                searchType = (GFGridSearchType)(((int)searchType + 1) % 4);
            } else if (Input.GetKeyDown(KeyCode.S)) {
                searchType = (GFGridSearchType)(((int)searchType + 3) % 4);
            }
        }

        Vector2Int CursorPos() {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2Int((int)mousePos.x, (int)mousePos.y);
        }

        void OnDrawGizmos() {

            Vector2Int cursorPos = CursorPos();

            if (gridHashmap == null) {
                // Draw Cursor
                Gizmos.color = Color.red;
                DrawCube(new Vector3(cursorPos.x, cursorPos.y, 0), Vector3.one);
                return;
            }

            Vector2Int bigSize = gridHashmap.BigMapSize;
            Vector3 halfSize = new Vector2(bigSize.x * 0.5f, bigSize.y * 0.5f);

            // Draw Grid
            Gizmos.color = Color.white;
            foreach (var key in gridHashmap.BigMapKeys) {
                Gizmos.DrawWireCube(new Vector3(key.x * bigSize.x, key.y * bigSize.y, 0) + halfSize, new Vector3(bigSize.x, bigSize.y, 0));
            }

            // Draw SmallValues
            gridHashmap.ForeachAllSmallValues((pos, value) => {
                Gizmos.color = Color.white;
                DrawCube(new Vector3(pos.x, pos.y, 0), Vector3.one);
            });

            // Draw Search Big
            int searchLen = GFGrid.GetCells(searchType, searchSize, cursorPos, tempCells);
            for (int i = 0; i < searchLen; i++) {
                Vector2Int cell = tempCells[i];
                Vector2Int bigKey = gridHashmap.GetBigKey(cell);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(new Vector3(bigKey.x * bigSize.x, bigKey.y * bigSize.y, 0) + halfSize, new Vector3(bigSize.x, bigSize.y, 0));
            }

            // Draw Saerch Cursor
            for (int i = 0; i < searchLen; i++) {
                Vector2Int cell = tempCells[i];
                Gizmos.color = Color.red;
                DrawCube(new Vector3(cell.x, cell.y, 0), Vector3.one);
            }

            Gizmos.color = Color.green;
            if (mode == 1) {
                for (int i = 0; i < searchLen; i++) {
                    Vector2Int cell = tempCells[i];
                    // Draw SmallValuesInBig
                    gridHashmap.TryGetSmallKeysInBig(cell, out var set);
                    if (set != null) {
                        foreach (var key in set) {
                            DrawCube(new Vector3(key.x, key.y, 0), Vector3.one);
                        }
                    }
                }
            } else if (mode == 2) {
                for (int i = 0; i < searchLen; i++) {
                    Vector2Int cell = tempCells[i];

                    // Draw SmallValues
                    gridHashmap.TryGetSmallValues(cell, out var set);
                    if (set != null) {
                        float mult = 0;
                        foreach (var value in set) {
                            Vector3 size = Vector3.one + new Vector3(mult, mult) * mult;
                            Vector3 pos = new Vector3(cell.x, cell.y, 0) - new Vector3(mult, mult) * 0.5f;
                            DrawCube(pos, size);
                            mult += 0.1f;
                        }
                    }
                }
            }

        }

        void DrawCube(Vector3 pos, Vector3 size) {
            Gizmos.DrawWireCube(pos + size / 2, new Vector3(size.x, size.y, 0));
        }

    }

}
