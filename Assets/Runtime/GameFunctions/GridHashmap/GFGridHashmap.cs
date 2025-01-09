using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFunctions {

    /// <summary> Recommended: T as struct { EntityType, EntityID } </summary>
    public class GFGridHashmap<T> {

        Dictionary<Vector2Int, HashSet<Vector2Int>> bigMap;
        public IEnumerable<Vector2Int> BigMapKeys => bigMap.Keys;

        Vector2Int bigMapSize;
        public Vector2Int BigMapSize => bigMapSize;

        Dictionary<Vector2Int, HashSet<T>> smallMap;
        public IEnumerable<Vector2Int> SmallMapKeys => smallMap.Keys;

        public int SmallCount() => smallMap.Count;

        bool isStackable;
        int stackLimit;

        Pool<HashSet<T>> pool;

        public GFGridHashmap(Vector2Int bigMapSize, int bigMapCapacity, bool isStackable, int stackLimit) {
            this.bigMap = new Dictionary<Vector2Int, HashSet<Vector2Int>>(bigMapCapacity);
            this.bigMapSize = bigMapSize;
            this.smallMap = new Dictionary<Vector2Int, HashSet<T>>(bigMapCapacity * bigMapSize.x * bigMapSize.y);
            this.isStackable = isStackable;
            this.stackLimit = stackLimit;

            pool = new Pool<HashSet<T>>(1000, () => new HashSet<T>());
        }

        public bool Add(Vector2Int posAsKey, T value) {

            // BigMap
            Vector2Int bigMapKey = GetBigKey(posAsKey);

            bool has = bigMap.TryGetValue(bigMapKey, out HashSet<Vector2Int> bigSet);
            if (!has) {
                bigSet = new HashSet<Vector2Int>();
                bigMap.Add(bigMapKey, bigSet);
            }
            bigSet.Add(posAsKey);

            // SmallMap
            has = smallMap.TryGetValue(posAsKey, out HashSet<T> smallSet);
            if (!has) {
                smallSet = pool.Get();
                smallMap.Add(posAsKey, smallSet);
            }

            if (isStackable) {
                if (smallSet.Count < stackLimit) {
                    smallSet.Add(value);
                } else {
                    return false;
                }
            } else {
                if (smallSet.Count > 0) {
                    return false;
                }
                smallSet.Add(value);
            }

            return true;

        }

        public bool Remove(Vector2Int posAsKey, T value) {

            // BigMap
            Vector2Int bigMapKey = GetBigKey(posAsKey);
            bool has = bigMap.TryGetValue(bigMapKey, out HashSet<Vector2Int> bigSet);
            if (!has || bigSet.Count == 0) {
                return false;
            }

            // SmallMap
            has = smallMap.TryGetValue(posAsKey, out HashSet<T> smallSet);
            if (!has || smallSet.Count == 0) {
                return false;
            }

            if (smallSet.Contains(value)) {
                smallSet.Remove(value);
                if (smallSet.Count == 0) {
                    pool.Return(smallSet);
                    smallMap.Remove(posAsKey);
                }
                bigSet.Remove(posAsKey);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2Int GetBigKey(Vector2Int posAsKey) {
            if (posAsKey.x < 0) {
                posAsKey.x -= bigMapSize.x - 1;
            }
            if (posAsKey.y < 0) {
                posAsKey.y -= bigMapSize.y - 1;
            }
            return new Vector2Int(posAsKey.x / bigMapSize.x, posAsKey.y / bigMapSize.y);
        }

        public bool TryGetOneSmallValue(Vector2Int posAsKey, out T value) {
            bool has = smallMap.TryGetValue(posAsKey, out HashSet<T> smallSet);
            if (has && smallSet.Count > 0) {
                value = smallSet.First();
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetSmallValues(Vector2Int posAsKey, out HashSet<T> values) {
            bool has = smallMap.TryGetValue(posAsKey, out values);
            if (has && values.Count > 0) {
                return true;
            }
            return false;
        }

        public bool TryGetSmallKeysInBig(Vector2Int posAsKey, out HashSet<Vector2Int> smallKeys) {
            Vector2Int bigMapKey = GetBigKey(posAsKey);
            bool has = bigMap.TryGetValue(bigMapKey, out smallKeys);
            if (has && smallKeys.Count > 0) {
                return true;
            }
            return false;
        }

        public void ForeachAllSmallValues(Action<Vector2Int, T> action) {
            foreach (var kv in smallMap) {
                var set = kv.Value;
                foreach (T value in set) {
                    action(kv.Key, value);
                }
            }
        }

        public void ForeachSmallValuesInBig(Vector2Int posAsKey, Action<T> action) {
            Vector2Int bigMapKey = GetBigKey(posAsKey);
            bool has = bigMap.TryGetValue(bigMapKey, out HashSet<Vector2Int> values);
            if (has) {
                foreach (Vector2Int pos in values) {
                    bool hasSmall = smallMap.TryGetValue(pos, out HashSet<T> smallSet);
                    if (hasSmall) {
                        foreach (T value in smallSet) {
                            action(value);
                        }
                    }
                }
            }
        }

        #region OverlapAABB
        public int OverlapAABB(Vector2Int center, int radius, GFGridHashmapResult<T>[] results) {
            Vector2Int min_small = center - new Vector2Int(radius, radius);
            Vector2Int max_small = center + new Vector2Int(radius, radius);
            int evaluateCount = (radius * 2 + 1) * (radius * 2 + 1);
            if (evaluateCount > results.Length) {
                throw new Exception("results.Length is not enough");
            }
            return OverlapAABB_Evaluated(min_small, max_small, results);
        }

        public int OverlapAABB(Vector2Int center, Vector2Int size, GFGridHashmapResult<T>[] results) {
            Vector2Int min = center - size / 2;
            Vector2Int max = center + size / 2;
            return OverlapAABB_Evaluated(min, max, results);
        }

        int OverlapAABB_Evaluated(Vector2Int min_small, Vector2Int max_small, GFGridHashmapResult<T>[] results) {
            int evaluateCount = (max_small.x - min_small.x + 1) * (max_small.y - min_small.y + 1);
            if (evaluateCount >= smallMap.Count || evaluateCount > 6 * 6) {
                return OverlapAABB_FromBig(min_small, max_small, results);
            } else {
                return OverlapAABB_FromSmall(min_small, max_small, results);
            }
        }

        int OverlapAABB_FromBig(Vector2Int min_small, Vector2Int max_small, GFGridHashmapResult<T>[] results) {
            int count = 0;
            Vector2Int min_big = GetBigKey(min_small);
            Vector2Int max_big = GetBigKey(max_small);
            for (int x = min_big.x; x <= max_big.x; x += 1) {
                for (int y = min_big.y; y <= max_big.y; y += 1) {
                    Vector2Int bigPosKey = new Vector2Int(x, y);
                    bool has = bigMap.TryGetValue(bigPosKey, out HashSet<Vector2Int> smallKeys);
                    if (!has) {
                        continue;
                    }
                    foreach (Vector2Int smallPosKey in smallKeys) {
                        has = smallMap.TryGetValue(smallPosKey, out HashSet<T> smallSet);
                        if (!has) {
                            continue;
                        }
                        foreach (T value in smallSet) {
                            results[count++] = new GFGridHashmapResult<T> {
                                posKey = smallPosKey,
                                value = value
                            };
                        }
                    }
                }
            }
            return count;
        }

        int OverlapAABB_FromSmall(Vector2Int min_small, Vector2Int max_small, GFGridHashmapResult<T>[] results) {
            int count = 0;
            for (int x = min_small.x; x <= max_small.x; x++) {
                for (int y = min_small.y; y <= max_small.y; y++) {
                    Vector2Int smallPosKey = new Vector2Int(x, y);
                    bool has = smallMap.TryGetValue(smallPosKey, out HashSet<T> smallSet);
                    if (!has) {
                        continue;
                    }
                    foreach (T value in smallSet) {
                        results[count++] = new GFGridHashmapResult<T> {
                            posKey = smallPosKey,
                            value = value
                        };
                    }
                }
            }
            return count;
        }
        #endregion

    }

}
