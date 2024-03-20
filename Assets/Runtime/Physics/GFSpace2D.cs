using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    // 2D 物理空间
    // 空间内的所有刚体参与运算
    public class GFSpace2D {

        GFEnvironment2D env;

        List<GFRB2DEntity> allRB;

        // 存已交叉事件 (a & b)

        public Action<GFRB2DEntity, GFRB2DEntity> OnTriggerEnterHandle;

        public GFSpace2D() {
            env = new GFEnvironment2D();

            allRB = new List<GFRB2DEntity>();
            GFRB2DEntity[] array = new GFRB2DEntity[10]; // 8
            array[0] = new GFRB2DEntity();

        }

        public void Initialize(Vector2 gravity) {
            env.gravity = gravity;
        }

        // 添加 / 移除
        public GFRB2DEntity Create() {
            GFRB2DEntity rb = new GFRB2DEntity();
            rb.id = allRB.Count;
            rb.isActive = true; // 基于其他程序员大致默认的情况
            allRB.Add(rb);
            return rb;
        }

        public void AddRigidBody(GFRB2DEntity rb) {
            allRB.Add(rb);
        }

        public void RemoveRigidBody(GFRB2DEntity rb) {
            allRB.Remove(rb);
        }

        // 物理引擎阶段
        public void Tick(float dt) {

            for (int i = 0; i < allRB.Count; i++) {

                GFRB2DEntity rb = allRB[i];

                if (!rb.isActive) {
                    continue;
                }

                // 1. 引力叠加
                rb.velocity += env.gravity * dt;

                // 2. 根据速度计算坐标
                rb.pos += rb.velocity * dt;

            }

            // 3. 交叉检测(两两检测)
            for (int i = 0; i < allRB.Count; i++) {
                GFRB2DEntity a = allRB[i];
                if (!a.isActive) {
                    continue;
                }
                for (int j = i + 1; j < allRB.Count; j++) {
                    GFRB2DEntity b = allRB[j];
                    if (!b.isActive) {
                        continue;
                    }

                    // 交叉检测

                }
            }

            // 4. 触发交叉事件 Trigger
            // for 已交叉列表

            // 5. 穿透恢复(实物)
            // for 已交叉列表
            
            // 6. 触发穿透事件 Collision

        }

        static bool IsIntersect(GFRB2DEntity a, GFRB2DEntity b) {
            // if shape
            return false;
        }

    }

}