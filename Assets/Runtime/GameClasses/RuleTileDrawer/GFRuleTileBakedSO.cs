using System;
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
        public Texture2D tex_original;

        // ==== 输出: Rule 关系 ====
        // - 存储时
        public List<GFRuleTilePair> ruleList;

        // - 运行时
        Dictionary<GFRuleTilePosRelation, GFRuleTileSO> ruleDict;

        public void BakeDictionary() {
            ruleDict = new Dictionary<GFRuleTilePosRelation, GFRuleTileSO>();

            for (int i = 0; i < ruleList.Count; i++) {
                var pair = ruleList[i];
                ruleDict.Add(pair.pos, pair.tileSO);
            }
        }

        public void Generate(int tileSize) {
            // 在该SO目录下生成 Tile
        }

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

        public void CalculateHashCode(ReadOnlySpan<GFVector2HalfSByte> must, ReadOnlySpan<GFVector2HalfSByte> mustNot) {
            hashcode = 0;

            for (int i = 0; i < must.Length; i++) {
                int pos = must[i].ToPos();
                if (pos < 64) {
                    lowHashcode |= 0x01ul << pos;
                } else {
                    highHashcode |= 0x01ul << (pos - 64);
                }
            }

            for (int i = 0; i < mustNot.Length; i++) {
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