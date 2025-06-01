using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GameFunctions.GridGeneratorInternal;
using RD = System.Random;
using CTX = GameFunctions.GridGeneratorInternal.GridGenContext;

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
        public static int[] GenAll(GridGenGridOption gridOption, params GridGenAreaOption[] options) {

            CTX ctx = new CTX();
            ctx.Init(gridOption, options);

            int areaLen = ctx.TakeAllArea(out var areas);
            foreach (var area in areas) {
                Gen_Area(ctx, area);
            }

            return ctx.grid;
        }

        // ==== Sea ====
        // cells[index] = seaValue
        static bool Gen_Area(CTX ctx, GridGenAreaEntity area) {
            var option = area.option;
            option.FROM_DIR = Math.Abs(option.FROM_DIR) % GridGenAlgorithm.DIR_COUNT;

            GridGenStartType startType = option.startType;
            if (startType == GridGenStartType.GridEdge) {
                Start_GridEdge(ctx, area);
            } else if (startType == GridGenStartType.AwayFromCell) {
                Start_AwayFromCell(ctx, area);
            } else if (startType == GridGenStartType.Random) {
                Start_Random(ctx, area);
            } else {
                Debug.LogError("Unknown StartType: " + startType.ToString());
                return false;
            }

            GridGenAlgorithmType loopType = option.algorithmType;
            if (loopType == GridGenAlgorithmType.FillAll) {
                return Loop_Fill(ctx, area);
            } else if (loopType == GridGenAlgorithmType.ErodeFromEdge) {
                return Loop_Erode(ctx, area);
            } else if (loopType == GridGenAlgorithmType.Flood) {
                return Loop_Flood(ctx, area);
            } else if (loopType == GridGenAlgorithmType.Scatter) {
                return Loop_Scatter(ctx, area);
            } else {
                Debug.LogError("Unknown AlgorithmType: " + loopType.ToString());
                return false;
            }
        }

        static bool Start_GridEdge(CTX ctx, GridGenAreaEntity area) {
            // Erode: start cell
            ref var option = ref area.option;
            GridGenAlgorithm.Pos_GetRandomPointOnEdge(ctx.random, ctx.gridOption.width, ctx.gridOption.height, option.FROM_DIR, out int start_x, out int start_y);
            int startIndex = GridGenAlgorithm.Index_GetByPos(start_x, start_y, ctx.gridOption.width);
            ctx.Grid_Set(startIndex, option.typeID);
            area.Add(startIndex);
            --area.option.count;
            return true;
        }

        static bool Start_AwayFromCell(CTX ctx, GridGenAreaEntity area) {

            // Flood: start cell
            int start_index;
            bool succ = Pos_GetAwayFrom(ctx, area, out start_index);
            if (!succ) {
                Debug.LogError("Gen Flood failed");
                return false;
            }

            ref var option = ref area.option;

            area.Add(start_index);
            ctx.Grid_Set(start_index, option.typeID);
            --option.count;
            return true;

        }

        static bool Start_Random(CTX ctx, GridGenAreaEntity area) {
            // Scatter: start cell
            ref var option = ref area.option;
            ctx.GetRandomCell(ctx.random, option.baseOnCellTypeID, out int start_index);
            ctx.Grid_Set(start_index, option.typeID);
            area.Add(start_index);
            --option.count;
            return true;
        }

        static bool Loop_Fill(CTX ctx, GridGenAreaEntity area) {

            var option = area.option;
            Action<int> onHandle = (int index) => {
                ctx.Grid_Set(index, option.typeID);
                area.Add(index);
            };

            // Fill: Loop
            return GridGenAlgorithm.Alg_FillAll(ctx.grid,
                                        option.typeID,
                                        onHandle);

        }

        static bool Loop_Erode(CTX ctx, GridGenAreaEntity area) {

            var option = area.option;
            int value = option.typeID;
            int count = option.count;

            Action<int> onHandle = (int index) => {
                ctx.Grid_Set(index, option.typeID);
                area.Add(index);
            };

            // Erode: Loop
            return GridGenAlgorithm.Alg_Erode_Loop(ctx.grid,
                                        area.indices,
                                        area.set,
                                        ctx.random,
                                        ctx.gridOption.width,
                                        ctx.gridOption.height,
                                        option.count,
                                        option.erodeRate,
                                        option.typeID,
                                        option.FROM_DIR,
                                        ctx.GetCellTypeValues(option.baseOnCellTypeID),
                                        ctx.GetCellTypeValues(option.awayFromCellTypeID),
                                        onHandle);

        }

        static bool Loop_Flood(CTX ctx, GridGenAreaEntity area) {

            int width = ctx.gridOption.width;
            int height = ctx.gridOption.height;
            RD random = ctx.random;

            var option = area.option;
            int count = option.count;
            int value = option.typeID;

            Action<int> onHandle = (int index) => {
                area.Add(index);
                ctx.Grid_Set(index, option.typeID);
            };

            bool hasBaseOn = ctx.TryGetArea(option.baseOnCellTypeID, out GridGenAreaEntity baseOnArea);
            bool hasAwayFrom = ctx.TryGetArea(option.awayFromCellTypeID, out GridGenAreaEntity awayFromArea);

            // Flood: loop
            return GridGenAlgorithm.Alg_Flood_Loop(ctx.grid,
                                        area.indices,
                                        area.set,
                                        random,
                                        width,
                                        height,
                                        count,
                                        value,
                                        baseOnArea.set,
                                        awayFromArea.set,
                                        onHandle);

        }

        static bool Loop_Scatter(CTX ctx, GridGenAreaEntity area) {

            var option = area.option;
            int count = option.count;
            int value = option.typeID;
            int width = ctx.gridOption.width;
            int height = ctx.gridOption.height;
            RD random = ctx.random;

            Action<int> onHandle = (int index) => {
                area.Add(index);
                ctx.Grid_Set(index, value);
            };

            // Scatter: loop
            bool hasBaseOn = ctx.TryGetArea(option.baseOnCellTypeID, out GridGenAreaEntity baseOnArea);
            bool hasAwayFrom = ctx.TryGetArea(option.awayFromCellTypeID, out GridGenAreaEntity awayFromArea);

            return GridGenAlgorithm.Alg_Scatter_Loop(ctx.grid,
                                            area.indices,
                                            area.set,
                                            random,
                                            width,
                                            height,
                                            count,
                                            option.typeID,
                                            option.scatterMinMax,
                                            baseOnArea.set,
                                            awayFromArea.set,
                                            onHandle);

        }

        static bool Pos_GetAwayFrom(CTX ctx, GridGenAreaEntity area, out int start_index) {

            int width = ctx.gridOption.width;
            int height = ctx.gridOption.height;

            RD random = ctx.random;

            var option = area.option;

            bool hasAwayFrom = ctx.TryGetArea(option.awayFromCellTypeID, out GridGenAreaEntity awayFromArea);
            HashSet<int> awayFromValues = awayFromArea.set;
        
            Vector4Int edgeOffset = new Vector4Int();
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
                ctx.GetRandomCell(random, option.baseOnCellTypeID, out start_index);
                if (awayFromValues.Contains(ctx.grid[start_index])) {
                    continue;
                }

                start_x = start_index % width;
                start_y = start_index / width;

                edgeOffset = Pos_DetectAwayFrom(ctx.grid, width, height, start_x, start_y, awayFromValues, option.awayFromManhattanDis);

            } while (edgeOffset.Min() < option.awayFromManhattanDis);

            if (failedTimes <= 0) {
                Debug.LogError("Pos_GetAwayFrom failed");
                return false;
            }
            return true;
        }

        static Vector4Int Pos_DetectAwayFrom(int[] cells, int width, int height, int x, int y, HashSet<int> awayValues, int awayFromSize) {
            // up down left right, walk awayFromSize steps
            // if found awayValue, return awayFromSize
            bool findLeft, findRight, findTop, findBottom;
            findTop = false; // x
            findRight = false; // y
            findBottom = false; // w
            findLeft = false; // z
            Vector4Int edgeOffset = new Vector4Int();
            for (int i = 0; i <= awayFromSize; i += 1) {
                if (!findTop) {
                    int upIndex = GridGenAlgorithm.Index_GetByPos(x, y + i, width);
                    if (upIndex != -1 && awayValues.Contains(cells[upIndex])) {
                        edgeOffset.x = i;
                        findTop = true;
                    }
                }

                if (!findRight) {
                    int rightIndex = GridGenAlgorithm.Index_GetByPos(x + i, y, width);
                    if (rightIndex != -1 && awayValues.Contains(cells[rightIndex])) {
                        edgeOffset.y = i;
                        findRight = true;
                    }
                }

                if (!findBottom) {
                    int downIndex = GridGenAlgorithm.Index_GetByPos(x, y - i, width);
                    if (downIndex != -1 && awayValues.Contains(cells[downIndex])) {
                        edgeOffset.w = i;
                        findBottom = true;
                    }
                }

                if (!findLeft) {
                    int leftIndex = GridGenAlgorithm.Index_GetByPos(x - i, y, width);
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