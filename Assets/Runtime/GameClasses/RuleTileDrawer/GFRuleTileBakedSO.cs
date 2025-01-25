using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameFunctions;

#pragma warning disable 0660
#pragma warning disable 0661

namespace GameClasses.RuleTileDrawer {

    [CreateAssetMenu(fileName = "So_GFRuleTileBaked_", menuName = "GameFunctions/GFRuleTileBakedSO", order = 1)]
    public class GFRuleTileBakedSO : ScriptableObject {

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

        [SerializeField] string gen_dir = "Assets/Res_Runtime/RuleTileDrawer/";

        // ==== 输出: Rule 关系 ====
        // - 存储时
        [SerializeField] List<GFRuleTilePair> ruleList;

        // - 运行时
        Dictionary<GFRuleTilePosRelation, GFRuleTileSO> ruleDict;

        public void BakeDictionary() {
            ruleDict = new Dictionary<GFRuleTilePosRelation, GFRuleTileSO>();

            for (int i = 0; i < ruleList.Count; i++) {
                var pair = ruleList[i];
                ruleDict.Add(pair.pos, pair.tileSO);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Generate")]
        public void Generate() {

            ruleList.Clear();
            UnityEditor.EditorUtility.SetDirty(this);

            if (!Directory.Exists(gen_dir)) {
                Directory.CreateDirectory(gen_dir);
            }

            Texture2D tex = tex_origin;
            int perTileSize = this.perTileSize;

            int maxXCount = tex.width / perTileSize;
            int maxYCount = tex.height / perTileSize;

            // LeftBottom -> RightBottom
            ScanOutterCorner(new Vector2Int(0, 0), new Vector2Int(maxXCount - 1, 0), new Vector2Int(1, 0), new Vector2Int(1, 1));

            // RightBottom -> LeftBottom
            ScanOutterCorner(new Vector2Int(maxXCount - 1, 0), new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1));

            // LeftTop -> RightTop
            ScanOutterCorner(new Vector2Int(0, maxYCount - 1), new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(1, 0), new Vector2Int(1, -1));

            // RightTop -> LeftTop
            ScanOutterCorner(new Vector2Int(maxXCount - 1, maxYCount - 1), new Vector2Int(0, maxYCount - 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1));

            UnityEditor.EditorUtility.SetDirty(this);
        }

        static List<GFVector2HalfSByte> tmp_mustList = new List<GFVector2HalfSByte>();
        static List<GFVector2HalfSByte> tmp_mustNotList = new List<GFVector2HalfSByte>();
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

                tmp_mustList.Add(new GFVector2HalfSByte(0, 0));
                tmp_mustList.Add(new GFVector2HalfSByte((sbyte)(deepDir.x), 0));
                tmp_mustList.Add(new GFVector2HalfSByte(0, (sbyte)(deepDir.y)));

                tmp_mustNotList.Add(new GFVector2HalfSByte((sbyte)(-deepDir.x * (i + 1)), 0));
                tmp_mustNotList.Add(new GFVector2HalfSByte(0, (sbyte)(-deepDir.y * (i + 1))));

                for (int j = 0; j < i; j += 1) {
                    tmp_mustList.Add(new GFVector2HalfSByte((sbyte)(-deepDir.x * (j + 1)), 0));
                    tmp_mustList.Add(new GFVector2HalfSByte(0, (sbyte)(-deepDir.y * (j + 1))));
                }

                GenerateOneTile(tmp_mustList, tmp_mustNotList, x, y);

            }
        }

        void GenerateOneTile(List<GFVector2HalfSByte> must, List<GFVector2HalfSByte> mustNot, int xGrid, int yGrid) {
            short typeID = (short)xGrid;
            typeID |= (short)(yGrid << 8);

            GFRuleTilePosRelation posRelation = new GFRuleTilePosRelation();
            posRelation.CalculateHashCode(must, mustNot);

            var spr = Sprite.Create(tex_origin, new Rect(xGrid * perTileSize, yGrid * perTileSize, perTileSize, perTileSize), new Vector2(0.5f, 0.5f), perTileSize);
            string sprFilePath = Path.Combine(gen_dir, $"Spr_{typeID}.asset");
            UnityEditor.AssetDatabase.CreateAsset(spr, sprFilePath);
            spr = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(sprFilePath);
            UnityEditor.AssetDatabase.SaveAssets();

            var tile = ScriptableObject.CreateInstance<GFRuleTileSO>();
            tile.typeID = typeID;
            tile.spr_default = spr;
            var fileFilePath = Path.Combine(gen_dir, $"Tile_{typeID}.asset");
            UnityEditor.AssetDatabase.CreateAsset(tile, fileFilePath);
            UnityEditor.AssetDatabase.SaveAssets();

            tile = UnityEditor.AssetDatabase.LoadAssetAtPath<GFRuleTileSO>(fileFilePath);

            ruleList.Add(new GFRuleTilePair {
                pos = posRelation,
                tileSO = tile,
            });

            Debug.Log($"GenerateOneTile: {typeID}");

        }
#endif

        Vector2Int[] tempCells = new Vector2Int[49];
        public void FillOneCell(Tilemap tilemap, Vector2Int pos) {
            GFRuleTilePosRelation posRelation = new GFRuleTilePosRelation();
            // 根据 pos 位置填充 Tile
            int len = GFGrid.RectCycle_GetCellsBySpirals(pos, 3, tempCells);
            for (int i = 0; i < len; i++) {
                var cell = tempCells[i];
                var existTile = tilemap.GetTile<GFRuleTileSO>(new Vector3Int(cell.x, cell.y, 0));
                if (existTile != null) {
                    Vector2Int diff = cell - pos;
                    GFVector2HalfSByte diffHalf;
                    diffHalf.val = 0;
                    diffHalf.x = (sbyte)diff.x;
                    diffHalf.y = (sbyte)diff.y;
                    int hashPos = diffHalf.ToPos();
                    posRelation.AddExist(hashPos);
                }
            }

            foreach (var kv in ruleDict) {
                var condition = kv.Key;
                if (condition.IsFit(posRelation)) {
                    tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), kv.Value);
                    break;
                }
            }
        }

    }

    [Serializable]
    public struct GFRuleTilePair {
        public GFRuleTilePosRelation pos;
        public GFRuleTileSO tileSO;
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct GFVector2HalfSByte : IEquatable<GFVector2HalfSByte> {

        // | 0000 | 0000 |
        // |  y   |  x   |
        [FieldOffset(0)]
        public byte val;

        // -8 ~ 7
        public sbyte x {
            get {
                return (sbyte)(val & 0b0000_1111);
            }
            set {
                val |= (byte)value;
            }
        }

        // -8 ~ 7
        public sbyte y {
            get {
                return (sbyte)((val & 0b1111_0000) >> 4);
            }
            set {
                val |= (byte)(value << 4);
            }
        }

        public GFVector2HalfSByte(sbyte x, sbyte y) {
            val = 0;
            this.x = x;
            this.y = y;
        }

        public int ToPos() {

            // 最大支持7*7的Tile

            // -3,-3 -> 0, 左下角往上0行
            // -2,-3 -> 1
            // -1,-3 -> 2
            //  0,-3 -> 3
            //  1,-3 -> 4
            //  2,-3 -> 5
            //  3,-3 -> 6, 右下角往上0行

            //  -3,-2 -> 7, 左下角往上1行
            //  -2,-2 -> 8
            //  -1,-2 -> 9
            //   0,-2 -> 10
            //   1,-2 -> 11
            //   2,-2 -> 12
            //   3,-2 -> 13, 右下角往上1行

            //  -3,-1 -> 14, 左下角往上2行
            //  -2,-1 -> 15
            //  -1,-1 -> 16
            //   0,-1 -> 17
            //   1,-1 -> 18
            //   2,-1 -> 19
            //   3,-1 -> 20, 右下角往上2行

            //  -3, 0 -> 21, 左下角往上3行
            //  -2, 0 -> 22
            //  -1, 0 -> 23
            //   0, 0 -> 24
            //   1, 0 -> 25
            //   2, 0 -> 26
            //   3, 0 -> 27, 右下角往上3行

            //  -3, 1 -> 28, 左下角往上4行
            //  -2, 1 -> 29
            //  -1, 1 -> 30
            //   0, 1 -> 31
            //   1, 1 -> 32
            //   2, 1 -> 33
            //   3, 1 -> 34, 右下角往上4行

            //  -3, 2 -> 35, 左下角往上5行
            //  -2, 2 -> 36
            //  -1, 2 -> 37
            //   0, 2 -> 38
            //   1, 2 -> 39
            //   2, 2 -> 40
            //   3, 2 -> 41, 右下角往上5行

            //  -3, 3 -> 42, 左下角往上6行
            //  -2, 3 -> 43
            //  -1, 3 -> 44
            //   0, 3 -> 45
            //   1, 3 -> 46
            //   2, 3 -> 47
            //   3, 3 -> 48, 右下角往上6行

            return ((y + 3) * 7 + (x + 3)) * 2;

        }

        bool IEquatable<GFVector2HalfSByte>.Equals(GFVector2HalfSByte other) {
            return val == other.val;
        }

        public static bool operator ==(GFVector2HalfSByte a, GFVector2HalfSByte b) {
            return a.val == b.val;
        }

        public static bool operator !=(GFVector2HalfSByte a, GFVector2HalfSByte b) {
            return a.val != b.val;
        }

    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct GFRuleTilePosRelation : IEquatable<GFRuleTilePosRelation> {

        // 128bit(实际只用了96bit)
        // |00| 每2bit表示一个Vector2HalfSByte, 00表示空, 01表示此位置必须有Tile, 10表示此位置必须没有Tile
        [FieldOffset(0)]
        public decimal hashcode;

        [FieldOffset(0)]
        ulong lowHashcode;

        [FieldOffset(8)]
        ulong highHashcode;

        public void CalculateHashCode(List<GFVector2HalfSByte> must, List<GFVector2HalfSByte> mustNot) {
            hashcode = 0;

            for (int i = 0; i < must.Count; i++) {
                int pos = must[i].ToPos();
                if (pos < 64) {
                    lowHashcode |= 0x01ul << pos;
                } else {
                    highHashcode |= 0x01ul << (pos - 64);
                }
            }

            for (int i = 0; i < mustNot.Count; i++) {
                int pos = mustNot[i].ToPos();
                if (pos < 64) {
                    lowHashcode |= 0b10ul << pos;
                } else {
                    highHashcode |= 0b10ul << (pos - 64);
                }
            }
        }

        public void AddExist(int pos) {
            if (pos < 64) {
                lowHashcode |= 0x01ul << pos;
            } else {
                highHashcode |= 0x01ul << (pos - 64);
            }
        }

        public bool IsFit(GFRuleTilePosRelation state) {

            // this is condition

            // `Must`
            ulong mustLow = this.lowHashcode & 0x55_55_55_55_55_55_55_55ul;
            ulong mustHigh = this.highHashcode & 0x55_55_55_55_55_55_55_55ul;
            ulong stateLow = state.lowHashcode & mustLow;
            ulong stateHigh = state.highHashcode & mustHigh;
            if (stateLow != mustLow || stateHigh != mustHigh) {
                return false;
            }

            // `MustNot`
            ulong mustNotLow = this.lowHashcode & 0xAA_AA_AA_AA_AA_AA_AA_AAul;
            ulong mustNotHigh = this.highHashcode & 0xAA_AA_AA_AA_AA_AA_AA_AAul;
            ulong stateNotLow = (state.lowHashcode | 0xAA_AA_AA_AA_AA_AA_AA_AAul) & mustNotLow;
            ulong stateNotHigh = (state.highHashcode | 0xAA_AA_AA_AA_AA_AA_AA_AAul) & mustNotHigh;
            if (stateNotLow != mustNotLow || stateNotHigh != mustNotHigh) {
                return false;
            }

            return true;

        }

        bool IEquatable<GFRuleTilePosRelation>.Equals(GFRuleTilePosRelation other) {
            return hashcode == other.hashcode;
        }

        public static bool operator ==(GFRuleTilePosRelation a, GFRuleTilePosRelation b) {
            return a.hashcode == b.hashcode;
        }

        public static bool operator !=(GFRuleTilePosRelation a, GFRuleTilePosRelation b) {
            return a.hashcode != b.hashcode;
        }

    }

}