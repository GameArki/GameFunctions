using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameFunctions;
using GameClasses.RuleTileDrawer.Internal;

namespace GameClasses.RuleTileDrawer {

    [CreateAssetMenu(fileName = "So_GFRuleTileBaker_", menuName = "GameFunctions/GFRuleTileBakerSO", order = 1)]
    public class RuleTileBakerSO : ScriptableObject {

        // - 输入: 原图
        // 1. OutCornerLv1: UpLeft, UpRight, DownLeft, DownRight
        // 2. EdgeLv1: UpEdge * n, DownEdge * n, LeftEdge * n, RightEdge * n
        // 3. InCornerLv1: UpLeft, UpRight, DownLeft, DownRight

        // 4. OutCornerLv2 * 4
        // 5. EdgeLv2: UpEdge * n, DownEdge * n, LeftEdge * n, RightEdge * n
        // 6. InCornerLv2 * 4

        // 7. OutCornerLv3 * 4
        // 8. EdgeLv3: UpEdge * n, DownEdge * n, LeftEdge * n, RightEdge * n
        // 9. InCornerLv3 * 4

        // 10. Center

        [SerializeField] Texture2D tex_origin;
        [SerializeField] int perTileSize = 16;
        [SerializeField] RuleTileSO tile_center;

        [SerializeField] string gen_dir = "Assets/Res_Runtime/RuleTileDrawer/";

        // ==== 输出: Rule 关系 ====
        // - 存储时
        [SerializeField] List<RuleTilePair> ruleList;

        HashSet<Vector2Int> existTiles = new HashSet<Vector2Int>(1024);

        int gen_id = 0;
        #region Runtime
        public void Init() {
            existTiles.Clear();

            // ruleList.Sort((a, b) => {
            //     return b.relation.CompareTo(a.relation);
            // });
        }

        Vector2Int[] tempCells = new Vector2Int[49];
        public void FillOneCell(Tilemap tilemap, Vector2Int pos, bool isRefreshExists) {
            RuleTileRelationDescription posRelation = new RuleTileRelationDescription();
            // 根据 pos 位置填充 Tile
            int len = GFGrid.RectCycle_GetCellsBySpirals(pos, 3, tempCells);
            for (int i = 0; i < len; i++) {
                var cell = tempCells[i];
                var existTile = tilemap.GetTile<RuleTileSO>(new Vector3Int(cell.x, cell.y, 0));
                if (existTile != null) {
                    Vector2Int diff = cell - pos;
                    Vector2HalfSByte diffHalf = new Vector2HalfSByte();
                    diffHalf.SetX(diff.x);
                    diffHalf.SetY(diff.y);
                    int hashPos = diffHalf.ToPos();
                    posRelation.AddExist(hashPos);
                }
            }

            Vector3Int fillPos = new Vector3Int(pos.x, pos.y, 0);
            for (int i = ruleList.Count - 1; i >= 0; i--) {
                var kv = ruleList[i];
                var condition = kv.relation;
                if (condition.IsFit(posRelation)) {
                    tilemap.SetTile(fillPos, kv.tileSO.Next());
                    existTiles.Add(pos);
                    if (isRefreshExists) {
                        RefreshExists(tilemap, pos);
                    }
                    return;
                }
            }

            // fallback
            tilemap.SetTile(fillPos, tile_center);
            existTiles.Add(pos);
            if (isRefreshExists) {
                RefreshExists(tilemap, pos);
            }
        }

        Vector2Int[] tempCellsForRefresh = new Vector2Int[49];
        void RefreshExists(Tilemap tilemap, Vector2Int pos) {
            int len = GFGrid.RectCycle_GetCellsBySpirals(pos, 3, tempCellsForRefresh);
            for (int i = 0; i < len; i++) {
                var cell = tempCellsForRefresh[i];
                if (existTiles.Contains(cell)) {
                    FillOneCell(tilemap, cell, false);
                }
            }
        }
        #endregion

#if UNITY_EDITOR
        #region Editor
        [ContextMenu("Generate")]
        void Generate() {

            gen_id = 0;

            ruleList.Clear();
            UnityEditor.EditorUtility.SetDirty(this);

            if (!Directory.Exists(gen_dir)) {
                Directory.CreateDirectory(gen_dir);
            }

            Texture2D tex = tex_origin;
            int perTileSize = this.perTileSize;

            int maxXCount = tex.width / perTileSize;
            int maxYCount = tex.height / perTileSize;

            // ==== OutterCorners ====
            // LeftBottom -> RightBottom
            ScanOutterCorner(new Vector2Int(0, 0), new Vector2Int(maxXCount - 1, 0), new Vector2Int(1, 0), new Vector2Int(1, 1));

            // RightBottom -> LeftBottom
            ScanOutterCorner(new Vector2Int(maxXCount - 1, 0), new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1));

            // LeftTop -> RightTop
            ScanOutterCorner(new Vector2Int(0, maxYCount - 1), new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(1, 0), new Vector2Int(1, -1));

            // RightTop -> LeftTop
            ScanOutterCorner(new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(0, maxYCount - 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1));

            // ==== InnerCorners ====
            // LeftBottom -> RightTop
            ScanInnerCorner(new Vector2Int(0, 0), new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(1, 1), new Vector2Int(1, 1));

            // RightBottom -> LeftTop
            ScanInnerCorner(new Vector2Int(maxXCount - 1, 0), new Vector2Int(0, maxYCount - 1), new Vector2Int(-1, 1), new Vector2Int(-1, 1));

            // LeftTop -> RightBottom
            ScanInnerCorner(new Vector2Int(0, maxYCount - 1), new Vector2Int(maxXCount - 1, 0), new Vector2Int(1, -1), new Vector2Int(1, -1));

            // RightTop -> LeftBottom
            ScanInnerCorner(new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(0, 0), new Vector2Int(-1, -1), new Vector2Int(-1, -1));

            // ==== Horizontal Edges ====
            // LeftBottom -> RightBottom
            ScanHorizontalEdge(new Vector2Int(0, 0), new Vector2Int(maxXCount - 1, 0), new Vector2Int(1, 0), new Vector2Int(0, 1));

            // LeftTop -> RightTop
            ScanHorizontalEdge(new Vector2Int(0, maxYCount - 1), new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(1, 0), new Vector2Int(0, -1));

            // ==== Vertical Edges ====
            // Left
            ScanVerticalEdge(new Vector2Int(0, 0), new Vector2Int(0, maxYCount - 1), new Vector2Int(0, 1), new Vector2Int(1, 0));

            // Right
            ScanVerticalEdge(new Vector2Int(maxXCount - 1, 0), new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(0, 1), new Vector2Int(-1, 0));

            UnityEditor.EditorUtility.SetDirty(this);
        }

        static List<Vector2HalfSByte> tmp_mustList = new List<Vector2HalfSByte>(16);
        static List<Vector2HalfSByte> tmp_mustNotList = new List<Vector2HalfSByte>(16);
        /*
            o-----
            -o----
            --o---
            ------

             x
            xo√
             √
        */
        void ScanOutterCorner(Vector2Int start, Vector2Int end, Vector2Int scanDir, Vector2Int deepDir) {
            Vector2Int meetPos = Vector2Int.zero;
            for (int x = start.x; x != end.x; x += scanDir.x) {
                Color cur = tex_origin.GetPixel(x * perTileSize + perTileSize / 2, start.y * perTileSize + perTileSize / 2);
                if (cur.a != 0) {
                    meetPos = new Vector2Int(x, start.y);
                    break;
                }
            }

            int maxDeep = 3;
            for (int i = 0; i < maxDeep; i += 1) {
                int x = meetPos.x + deepDir.x * i;
                int y = meetPos.y + deepDir.y * i;
                Color cur = tex_origin.GetPixel(x * perTileSize + perTileSize / 2, y * perTileSize + perTileSize / 2);
                if (cur.a == 0) {
                    Debug.LogError($"Failed OutterCorner: {x}, {y}");
                    break;
                }

                tmp_mustList.Clear();
                tmp_mustNotList.Clear();

                tmp_mustList.Add(new Vector2HalfSByte(0, 0));
                tmp_mustList.Add(new Vector2HalfSByte((sbyte)(deepDir.x), 0));
                tmp_mustList.Add(new Vector2HalfSByte(0, (sbyte)(deepDir.y)));

                tmp_mustNotList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * (i + 1)), 0));
                tmp_mustNotList.Add(new Vector2HalfSByte(0, (sbyte)(-deepDir.y * (i + 1))));

                for (int j = 1; j < i + 1; j += 1) {
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * j), 0));
                    tmp_mustList.Add(new Vector2HalfSByte(0, (sbyte)(-deepDir.y * j)));
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * j), (sbyte)(-deepDir.y * j)));
                }

                const string prefix = "OutterCorner";
                GenerateOneTile(tmp_mustList, tmp_mustNotList, x, y, prefix);
            }
        }

        /*
               -----
               -----
               -----
            ---o----
            ----o---
            -----o--
            --------
        */
        void ScanInnerCorner(Vector2Int start, Vector2Int end, Vector2Int scanDir, Vector2Int deepDir) {
            // Horizontal: Find the first meet point
            Vector2Int meetPos = Vector2Int.zero;
            for (int x = start.x; x != end.x; x += scanDir.x) {
                Color cur = tex_origin.GetPixel(x * perTileSize + perTileSize / 2, start.y * perTileSize + perTileSize / 2);
                Color nextHorizontal = tex_origin.GetPixel((x + scanDir.x) * perTileSize + perTileSize / 2, start.y * perTileSize + perTileSize / 2);
                if (cur.a == 0 && nextHorizontal.a != 0) {
                    meetPos = new Vector2Int(x, start.y);
                    break;
                }
            }

            // Vertical: Follow the meet point to Find the final meet point
            for (int y = start.y; y != end.y; y += scanDir.y) {
                Color cur = tex_origin.GetPixel(meetPos.x * perTileSize + perTileSize / 2, y * perTileSize + perTileSize / 2);
                Color nextVertical = tex_origin.GetPixel(meetPos.x * perTileSize + perTileSize / 2, (y + scanDir.y) * perTileSize + perTileSize / 2);
                if (cur.a == 0 && nextVertical.a != 0) {
                    meetPos = new Vector2Int(meetPos.x, y) + deepDir;
                    break;
                }
            }

            int maxDeep = 3;
            for (int i = 0; i < maxDeep; i += 1) {
                int x = meetPos.x + deepDir.x * i;
                int y = meetPos.y + deepDir.y * i;
                Color cur = tex_origin.GetPixel(x * perTileSize + perTileSize / 2, y * perTileSize + perTileSize / 2);
                if (cur.a == 0) {
                    Debug.LogError($"Failed InnerCorner: {x}, {y}");
                    break;
                }

                tmp_mustList.Clear();
                tmp_mustNotList.Clear();

                for (int kx = -1; kx <= 1; kx += 1) {
                    for (int ky = -1; ky <= 1; ky += 1) {
                        if (i == 0 && kx == -deepDir.x && ky == -deepDir.y) {
                            continue;
                        }
                        tmp_mustList.Add(new Vector2HalfSByte((sbyte)kx, (sbyte)ky));
                    }
                }

                tmp_mustNotList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * (i + 1)), (sbyte)(-deepDir.y * (i + 1))));

                if (i > 0) {
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * i), (sbyte)(-deepDir.y * (i + 1))));
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * (i + 1)), (sbyte)(-deepDir.y * i)));
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * i), (sbyte)(-deepDir.y * i)));
                }

                const string prefix = "InnerCorner";
                GenerateOneTile(tmp_mustList, tmp_mustNotList, x, y, prefix);
            }

        }

        /*
            -oooo-
            ------
            ------
        */
        void ScanHorizontalEdge(Vector2Int start, Vector2Int end, Vector2Int scanDir, Vector2Int deepDir) {
            bool isFirstMeet = false;
            for (int sx = start.x; sx != end.x; sx += scanDir.x) {
                Color cur = tex_origin.GetPixel(sx * perTileSize + perTileSize / 2, start.y * perTileSize + perTileSize / 2);
                Color next = tex_origin.GetPixel((sx + scanDir.x) * perTileSize + perTileSize / 2, start.y * perTileSize + perTileSize / 2);
                Color last = tex_origin.GetPixel((sx - scanDir.x) * perTileSize + perTileSize / 2, start.y * perTileSize + perTileSize / 2);
                if (!isFirstMeet && cur.a != 0) {
                    isFirstMeet = true;
                    continue;
                }

                if (isFirstMeet && cur.a != 0 && next.a == 0) {
                    break;
                }

                if (cur.a == 0) {
                    continue;
                }

                Vector2Int meetPos = new Vector2Int(sx, start.y);

                int maxDeep = 3;
                for (int i = 0; i < maxDeep; i += 1) {
                    int x = meetPos.x + deepDir.x * i;
                    int y = meetPos.y + deepDir.y * i;

                    tmp_mustList.Clear();
                    tmp_mustNotList.Clear();

                    tmp_mustList.Add(new Vector2HalfSByte(0, 0));
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-1), 0));
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(1), 0));
                    tmp_mustList.Add(new Vector2HalfSByte(0, (sbyte)(deepDir.y)));

                    tmp_mustNotList.Add(new Vector2HalfSByte(0, (sbyte)(-deepDir.y * (i + 1))));

                    for (int j = 1; j < i + 1; j += 1) {
                        tmp_mustList.Add(new Vector2HalfSByte(-1, (sbyte)(-deepDir.y * j)));
                        tmp_mustList.Add(new Vector2HalfSByte(0, (sbyte)(-deepDir.y * j)));
                        tmp_mustList.Add(new Vector2HalfSByte(1, (sbyte)(-deepDir.y * j)));
                    }

                    string prefix = "HorizontalEdge";
                    GenerateOneTile(tmp_mustList, tmp_mustNotList, x, y, prefix);
                }
            }
        }

        /*
            -----
            o----
            o----
            o----
            -----
        */
        void ScanVerticalEdge(Vector2Int start, Vector2Int end, Vector2Int scanDir, Vector2Int deepDir) {
            bool isFirstMeet = false;
            for (int sy = start.y; sy != end.y; sy += scanDir.y) {
                Color cur = tex_origin.GetPixel(start.x * perTileSize + perTileSize / 2, sy * perTileSize + perTileSize / 2);
                Color next = tex_origin.GetPixel(start.x * perTileSize + perTileSize / 2, (sy + scanDir.y) * perTileSize + perTileSize / 2);
                Color last = tex_origin.GetPixel(start.x * perTileSize + perTileSize / 2, (sy - scanDir.y) * perTileSize + perTileSize / 2);
                if (!isFirstMeet && cur.a != 0) {
                    isFirstMeet = true;
                    continue;
                }

                if (isFirstMeet && cur.a != 0 && next.a == 0) {
                    break;
                }

                if (cur.a == 0) {
                    continue;
                }

                Vector2Int meetPos = new Vector2Int(start.x, sy);

                int maxDeep = 3;
                for (int i = 0; i < maxDeep; i += 1) {
                    int x = meetPos.x + deepDir.x * i;
                    int y = meetPos.y + deepDir.y * i;

                    tmp_mustList.Clear();
                    tmp_mustNotList.Clear();

                    tmp_mustList.Add(new Vector2HalfSByte(0, 0));
                    tmp_mustList.Add(new Vector2HalfSByte(0, (sbyte)(-1)));
                    tmp_mustList.Add(new Vector2HalfSByte(0, (sbyte)(1)));
                    tmp_mustList.Add(new Vector2HalfSByte((sbyte)(deepDir.x), 0));

                    tmp_mustNotList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * (i + 1)), 0));

                    for (int j = 1; j < i + 1; j += 1) {
                        tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * j), -1));
                        tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * j), 0));
                        tmp_mustList.Add(new Vector2HalfSByte((sbyte)(-deepDir.x * j), 1));
                    }

                    string prefix = "VerticalEdge";
                    GenerateOneTile(tmp_mustList, tmp_mustNotList, x, y, prefix);
                }
            }
        }

        void GenerateOneTile(List<Vector2HalfSByte> must, List<Vector2HalfSByte> mustNot, int xGrid, int yGrid, string prefix) {
            short typeID = (short)xGrid;
            typeID |= (short)(yGrid << 8);

            RuleTileRelationDescription posRelation = new RuleTileRelationDescription();
            posRelation.CalculateHashCode(must, mustNot);

            var spr = Sprite.Create(tex_origin, new Rect(xGrid * perTileSize, yGrid * perTileSize, perTileSize, perTileSize), new Vector2(0.5f, 0.5f), perTileSize);
            string sprFilePath = Path.Combine(gen_dir, $"Spr_{prefix}_{xGrid}_{yGrid}_{gen_id}.asset");
            UnityEditor.AssetDatabase.CreateAsset(spr, sprFilePath);
            spr = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(sprFilePath);

            var tile = ScriptableObject.CreateInstance<RuleTileSO>();
            tile.typeID = typeID;
            var fileFilePath = Path.Combine(gen_dir, $"Tile_{prefix}_{xGrid}_{yGrid}_{gen_id}.asset");
            UnityEditor.AssetDatabase.CreateAsset(tile, fileFilePath);

            UnityEditor.AssetDatabase.SaveAssets();

            tile.spr_default = spr;

            tile = UnityEditor.AssetDatabase.LoadAssetAtPath<RuleTileSO>(fileFilePath);

            ListWithIndex<RuleTileSO> tileList;
            int existIndex = ruleList.FindIndex(value => value.relation == posRelation);
            if (existIndex != -1) {
                tileList = ruleList[existIndex].tileSO;
            } else {
                tileList = new ListWithIndex<RuleTileSO>();
                ruleList.Add(new RuleTilePair() {
                    relation = posRelation,
                    tileSO = tileList
                });
            }
            tileList.Add(tile);

            gen_id++;

            UnityEditor.EditorUtility.SetDirty(this);

            // Debug.Log($"GenerateOneTile: {typeID}");

        }
        #endregion
#endif

    }

}