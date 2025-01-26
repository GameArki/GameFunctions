using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable 0660
#pragma warning disable 0661

namespace GameClasses.RuleTileDrawer.Internal {

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct RuleTileRelationDescription : IEquatable<RuleTileRelationDescription>, IComparable<RuleTileRelationDescription> {

        // 128bit(实际只用了98bit)
        // |00| 每2bit表示一个Vector2HalfSByte, 00表示空, 01表示此位置必须有Tile, 10表示此位置必须没有Tile
        [FieldOffset(0)]
        public decimal hashcode;

        [FieldOffset(0)]
        ulong lowHashcode;

        [FieldOffset(8)]
        ulong highHashcode;

        [FieldOffset(16)]
        byte conditionCount;

        public void CalculateHashCode(List<Vector2HalfSByte> must, List<Vector2HalfSByte> mustNot) {
            hashcode = 0;

            for (int i = 0; i < must.Count; i++) {
                int pos = must[i].ToPos();
                if (pos < 64) {
                    lowHashcode |= (0x01ul << pos);
                } else {
                    highHashcode |= (0x01ul << (pos - 64));
                }
            }

            for (int i = 0; i < mustNot.Count; i++) {
                int pos = mustNot[i].ToPos();
                if (pos < 64) {
                    lowHashcode |= (0b10ul << pos);
                } else {
                    highHashcode |= (0b10ul << (pos - 64));
                }
            }

            conditionCount = (byte)(must.Count + mustNot.Count);
        }

        public void AddExist(int pos) {
            if (pos < 64) {
                lowHashcode |= (0b01ul << pos);
            } else {
                highHashcode |= (0b01ul << (pos - 64));
            }
        }

        public bool IsFit(RuleTileRelationDescription state) {

            // this is condition

            // `Must`
            ulong mustLow = this.lowHashcode & 0x55_55_55_55_55_55_55_55ul;
            ulong mustHigh = this.highHashcode & 0x55_55_55_55_55_55_55_55ul;
            ulong stateLow = state.lowHashcode & mustLow;
            ulong stateHigh = state.highHashcode & mustHigh;
            if (stateLow != mustLow) {
                return false;
            }
            if (stateHigh != mustHigh) {
                return false;
            }

            // `MustNot`
            ulong mustNotLow = this.lowHashcode & 0xAA_AA_AA_AA_AA_AA_AA_AAul;
            ulong mustNotHigh = this.highHashcode & 0xAA_AA_AA_AA_AA_AA_AA_AAul;
            ulong stateNotLow = (state.lowHashcode | 0xAA_AA_AA_AA_AA_AA_AA_AAul) & mustNotLow;
            ulong stateNotHigh = (state.highHashcode | 0xAA_AA_AA_AA_AA_AA_AA_AAul) & mustNotHigh;
            if (stateNotLow != mustNotLow) {
                return false;
            }
            if (stateNotHigh != mustNotHigh) {
                return false;
            }

            return true;

        }

        bool IEquatable<RuleTileRelationDescription>.Equals(RuleTileRelationDescription other) {
            return hashcode == other.hashcode;
        }

        public int CompareTo(RuleTileRelationDescription other) {
            return conditionCount.CompareTo(other.conditionCount);
        }

        public static bool operator ==(RuleTileRelationDescription a, RuleTileRelationDescription b) {
            return a.hashcode == b.hashcode;
        }

        public static bool operator !=(RuleTileRelationDescription a, RuleTileRelationDescription b) {
            return a.hashcode != b.hashcode;
        }

    }

}