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
                bool succ_land = Gen_Area(ctx, land);
                if (!succ_land) {
                    Debug.LogError("Gen_Land failed");
                }
            });

            // Gen: Sea
            ctx.Sea_Foreach(sea => {

                bool succ_sea = Gen_Area(ctx, sea);
                if (!succ_sea) {
                    Debug.LogError("Gen_Sea failed");
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

                bool succ_lake = Gen_Area(ctx, lake);
                if (!succ_lake) {
                    Debug.LogError("Gen_Land_Lake failed");
                }

                // Cache Update: Land
                // remove lake cells from land cells
                for (int j = 0; j < lake.set.Count; j += 1) {
                    int lakeIndex = lake.indices[j];
                    ctx.Land_Remove(lakeIndex);
                }
                ctx.Land_UpdateAll();

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
            } else if (type == AlgorithmType.Erode) {
                return Gen_Erode(ctx, area);
            } else if (type == AlgorithmType.Flood) {
                return Gen_Flood(ctx, area);
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
            Action<int> onHandle = (int index) => {
                ctx.Grid_Set(index, option.value);
                area.Add(index);
            };

            // Erode: Loop
            return Algorithm.Alg_Erode(ctx.grid,
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

        // ==== Land-Lake ====
        static bool Gen_Flood(CTX ctx, AreaEntity area) {

            int width = ctx.gridOption.width;
            int height = ctx.gridOption.height;
            RD random = ctx.random;

            int failedTimes = width * height * 10;

            var option = area.option;
            int count = option.count;
            int value = option.value;
            int awayFromDis = option.awayFromManhattanDis;
            HashSet<int> awayFromValues = ctx.GetAwayFromValues(option.awayFromCellType);

            // Flood: start cell
            GFVector4Int edgeOffset = new GFVector4Int();
            int start_x = 0;
            int start_y = 0;
            int start_index = 0;
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

                edgeOffset = Algorithm.Pos_DetectAwayFrom(ctx.grid, width, height, start_x, start_y, awayFromValues, awayFromDis);

            } while (edgeOffset.Min() < option.awayFromManhattanDis);

            if (failedTimes <= 0) {
                Debug.LogError("Gen_Land_Lake_Normal failed");
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
            return Algorithm.Alg_Flood(ctx.grid,
                                        area.indices,
                                        area.set,
                                        random,
                                        width,
                                        height,
                                        count,
                                        value,
                                        onHandle);

        }

    }

}