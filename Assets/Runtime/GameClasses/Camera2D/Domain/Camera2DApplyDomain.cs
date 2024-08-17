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
            // Damping
            Vector2 targetPos = et.followTargetPos + et.followOffset;
            float maxXSpeed = et.followDampingXOrigin;
            float maxYSpeed = et.followDampingYOrigin;
            float x;
            if (et.followDampingXOrigin == 0) {
                x = targetPos.x;
            } else {
                x = Mathf.SmoothDamp(et.pos.x, targetPos.x, ref et.followDampingX, et.followDampingXOrigin, maxXSpeed, dt);
            }

            float y;
            if (et.followDampingYOrigin == 0) {
                y = targetPos.y;
            } else {
                y = Mathf.SmoothDamp(et.pos.y, targetPos.y, ref et.followDampingY, et.followDampingYOrigin, maxYSpeed, dt);
            }

            if (x == et.pos.x) {
                et.followDampingX = 0;
            }
            if (y == et.pos.y) {
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