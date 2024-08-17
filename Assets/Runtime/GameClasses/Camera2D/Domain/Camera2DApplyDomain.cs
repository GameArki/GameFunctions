using UnityEngine;
using GameFunctions;

namespace GameClasses.Camera2DLib.Internal {

    public static class Camera2DApplyDomain {

        public static void Process(Camera2DContext ctx, Camera2DVirtualEntity entity, float dt) {

            // Follow
            Vector2 pos;
            if (entity.isFollow) {
                pos = Follow_Process(ctx, entity, dt);
            } else {
                pos = entity.pos;
            }

            // Confine
            if (entity.isConfine) {
                pos = Confine_Calculate(ctx, entity, pos);
            }

            // Apply
            entity.pos = pos;

        }

        static Vector2 Follow_Process(Camera2DContext ctx, Camera2DVirtualEntity et, float dt) {

            Vector2 targetPos = et.followTargetPos + et.followOffset;

            // Damping
            float x;
            if (et.followDampingXOrigin == 0) {
                x = targetPos.x;
            } else {
                et.followDampingX += dt;
                float percent = et.followDampingX / et.followDampingXOrigin;
                x = Mathf.Lerp(et.pos.x, targetPos.x, percent);
                if (percent >= 1) {
                    et.followDampingX = 0;
                }
            }

            float y;
            if (et.followDampingYOrigin == 0) {
                y = targetPos.y;
            } else {
                et.followDampingY += dt;
                float percent = et.followDampingY / et.followDampingYOrigin;
                y = Mathf.Lerp(et.pos.y, targetPos.y, percent);
                if (percent >= 1) {
                    et.followDampingY = 0;
                }
            }

            if (Mathf.Approximately(x, et.pos.x)) {
                et.followDampingX = 0;
            }

            if (Mathf.Approximately(y, et.pos.y)) {
                et.followDampingY = 0;
            }

            return new Vector2(x, y);

        }

        static Vector2 Confine_Calculate(Camera2DContext ctx, Camera2DVirtualEntity entity, Vector2 pos) {
            Vector2 min = new Vector2(entity.minMaxBounds.x, entity.minMaxBounds.y);
            Vector2 max = new Vector2(entity.minMaxBounds.z, entity.minMaxBounds.w);
            pos = GFCamera2DHelper.CalcConfinePos(pos, min, max, entity.orthographicSize, entity.aspect);
            return pos;
        }

    }

}