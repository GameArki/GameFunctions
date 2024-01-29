using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFunctions.GridGeneratorInternal;
using RD = System.Random;
using CTX = GameFunctions.GridGeneratorInternal.Context;

// Rewrite 算法:
// 侵蚀算法(Erode):     01      = 11
// 描边算法(Outline):   011     = 211
// 洪泛算法(Flood):     01      = 11
// 播种算法(Scatter):   0100    = 0101
// 平滑算法(Smooth):    101     = 111
// 河流算法(River):     01000   = 01111
// 点缀算法(Decorate):  01000   = 01001
namespace GameFunctions {

    public static class GFGridGenerator {

        // ==== Land Init ====
        public static CTX GenAll(GridOption gridOption, params AreaOption[] options) {

            CTX ctx = new CTX();
            ctx.Init(gridOption, options);

            // Gen: Land
            ctx.Land_Foreach(land => {
                bool succ = Gen_Area(ctx, land);
                if (!succ) {
                    Debug.LogError("Gen Land failed");
                }
            });

            // Gen: Sea
            ctx.Sea_Foreach(sea => {

                bool succ = Gen_Area(ctx, sea);
                if (!succ) {
                    Debug.LogError("Gen Sea failed");
                }

                // Cache Update: Land
                // remove sea cells from land cells
                for (int i = 0; i < sea.set.Count; i += 1) {
                    int seaIndex = sea.indices[i];
                    ctx.Land_Remove(seaIndex);
                }
                ctx.Land_UpdateAll();

            });

            // Gen: Lake
            ctx.Lake_Foreach(lake => {

                bool succ = Gen_Area(ctx, lake);
                if (!succ) {
                    Debug.LogError("Gen Lake failed");
                }

                // Cache Update: Land
                // remove lake cells from land cells
                for (int j = 0; j < lake.set.Count; j += 1) {
                    int lakeIndex = lake.indices[j];
                    ctx.Land_Remove(lakeIndex);
                }
                ctx.Land_UpdateAll();

            });

            // Gen: Forest
            ctx.Forest_Foreach(forest => {
                bool succ = Gen_Area(ctx, forest);
                if (!succ) {
                    Debug.LogError("Gen Forest failed");
                }
            });

            return ctx;

        }

        // ==== Sea ====
        // cells[index] = seaValue
        public static bool Gen_Area(CTX ctx, AreaEntity area) {
            var option = area.option;
            option.FROM_DIR = Math.Abs(option.FROM_DIR) % Algorithm.DIR_COUNT;
            AlgorithmType type = option.algorithmType;
            if (type == AlgorithmType.FillAll) {
                return Gen_Fill(ctx, area);
            } else if (type == AlgorithmType.ErodeFromEdge) {
                return Gen_Erode(ctx, area);
            } else if (type == AlgorithmType.Flood) {
                return Gen_Flood(ctx, area);
            } else if (type == AlgorithmType.Scatter) {
                return Gen_Scatter(ctx, area);
            } else {
                Debug.LogWarning("Unknown type: " + type.ToString());
                return false;
            }
        }

        static bool Gen_Fill(CTX ctx, AreaEntity area) {

            var option = area.option;
            Action<int> onHandle = (int index) => {
                ctx.Grid_Set(index, option.value);
                area.Add(index);
            };

            // Fill: Loop
            return Algorithm.Alg_FillAll(ctx.grid,
                                        option.value,
                                        onHandle);

        }

        static bool Gen_Erode(CTX ctx, AreaEntity area) {

            var option = area.option;
            int value = option.value;
            int count = option.count;

            // Erode: start cell
            Algorithm.Pos_GetRandomPointOnEdge(ctx.random, ctx.gridOption.width, ctx.gridOption.height, option.FROM_DIR, out int start_x, out int start_y);
            int startIndex = Algorithm.Index_GetByPos(start_x, start_y, ctx.gridOption.width);
            ctx.Grid_Set(startIndex, option.value);
            area.Add(startIndex);
            --count;

            Action<int> onHandle = (int index) => {
                ctx.Grid_Set(index, option.value);
                area.Add(index);
            };

            // Erode: Loop
            return Algorithm.Alg_Erode_Loop(ctx.grid,
                                        area.indices,
                                        area.set,
                                        ctx.random,
                                        ctx.gridOption.width,
                                        ctx.gridOption.height,
                                        option.count,
                                        option.erodeRate,
                                        option.value,
                                        option.FROM_DIR,
                                        onHandle);

        }

        static bool Gen_Flood(CTX ctx, AreaEntity area) {

            int width = ctx.gridOption.width;
            int height = ctx.gridOption.height;
            RD random = ctx.random;

            var option = area.option;
            int count = option.count;
            int value = option.value;

            // Flood: start cell
            int start_index;
            bool succ = Pos_GetAwayFrom(ctx, width, height, random, area, out start_index);
            if (!succ) {
                Debug.LogError("Gen Flood failed");
                return false;
            }

            area.Add(start_index);
            ctx.Grid_Set(start_index, value);
            --count;

            Action<int> onHandle = (int index) => {
                area.Add(index);
                ctx.Grid_Set(index, value);
            };

            // Flood: loop
            return Algorithm.Alg_Flood_Loop(ctx.grid,
                                        area.indices,
                                        area.set,
                                        random,
                                        width,
                                        height,
                                        count,
                                        value,
                                        onHandle);

        }

        static bool Gen_Scatter(CTX ctx, AreaEntity area) {

            var option = area.option;
            int count = option.count;
            int value = option.value;
            int width = ctx.gridOption.width;
            int height = ctx.gridOption.height;
            RD random = ctx.random;

            // Scatter: start cell
            int start_index;
            bool succ = Pos_GetAwayFrom(ctx, width, height, random, area, out start_index);
            if (!succ) {
                Debug.LogError("Gen Scatter failed");
                return false;
            }

            area.Add(start_index);
            ctx.Grid_Set(start_index, value);
            --count;

            Action<int> onHandle = (int index) => {
                area.Add(index);
                ctx.Grid_Set(index, value);
            };

            // Scatter: loop
            return Algorithm.Alg_Scatter_Loop(ctx.grid,
                                            area.indices,
                                            area.set,
                                            random,
                                            width,
                                            height,
                                            count,
                                            option.value,
                                            option.scatterMinMax,
                                            onHandle);

        }

        static bool Pos_GetAwayFrom(CTX ctx, int width, int height, RD random, AreaEntity area, out int start_index) {
            var option = area.option;
            HashSet<int> awayFromValues = ctx.GetAwayFromValues(option.awayFromCellType);
            GFVector4Int edgeOffset = new GFVector4Int();
            int start_x = 0;
            int start_y = 0;
            start_index = 0;
            int failedTimes = width * height * 10;
            do {

                failedTimes--;
                if (failedTimes <= 0) {
                    break;
                }

                // Select a random land cell
                ctx.GetRandomCell(random, option.baseOnCellType, out start_index);
                if (awayFromValues.Contains(ctx.grid[start_index])) {
                    continue;
                }

                start_x = start_index % width;
                start_y = start_index / width;

                edgeOffset = Pos_DetectAwayFrom(ctx.grid, width, height, start_x, start_y, awayFromValues, option.awayFromManhattanDis);

            } while (edgeOffset.Min() < option.awayFromManhattanDis);

            if (failedTimes <= 0) {
                Debug.LogError("Gen_Land_Lake_Normal failed");
                return false;
            }
            return true;
        }

        public static GFVector4Int Pos_DetectAwayFrom(int[] cells, int width, int height, int x, int y, HashSet<int> awayValues, int awayFromSize) {
            // up down left right, walk awayFromSize steps
            // if found awayValue, return awayFromSize
            bool findLeft, findRight, findTop, findBottom;
            findTop = false; // x
            findRight = false; // y
            findBottom = false; // w
            findLeft = false; // z
            GFVector4Int edgeOffset = new GFVector4Int();
            for (int i = 0; i <= awayFromSize; i += 1) {
                if (!findTop) {
                    int upIndex = Algorithm.Index_GetByPos(x, y + i, width);
                    if (upIndex != -1 && awayValues.Contains(cells[upIndex])) {
                        edgeOffset.x = i;
                        findTop = true;
                    }
                }

                if (!findRight) {
                    int rightIndex = Algorithm.Index_GetByPos(x + i, y, width);
                    if (rightIndex != -1 && awayValues.Contains(cells[rightIndex])) {
                        edgeOffset.y = i;
                        findRight = true;
                    }
                }

                if (!findBottom) {
                    int downIndex = Algorithm.Index_GetByPos(x, y - i, width);
                    if (downIndex != -1 && awayValues.Contains(cells[downIndex])) {
                        edgeOffset.w = i;
                        findBottom = true;
                    }
                }

                if (!findLeft) {
                    int leftIndex = Algorithm.Index_GetByPos(x - i, y, width);
                    if (leftIndex != -1 && awayValues.Contains(cells[leftIndex])) {
                        edgeOffset.z = i;
                        findLeft = true;
                    }
                }

                if (findTop && findRight && findBottom && findLeft) {
                    break;
                }
            }
            if (!findTop) {
                edgeOffset.x = awayFromSize;
            }
            if (!findRight) {
                edgeOffset.y = awayFromSize;
            }
            if (!findBottom) {
                edgeOffset.w = awayFromSize;
            }
            if (!findLeft) {
                edgeOffset.z = awayFromSize;
            }
            return edgeOffset;
        }

    }

}