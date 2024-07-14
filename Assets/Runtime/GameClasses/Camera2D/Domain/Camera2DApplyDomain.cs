using UnityEngine;
using GameFunctions;

namespace GameClasses.Camera2DLib.Internal {

    public static class Camera2DApplyDomain {

        public static void ApplyCamera2D(Camera2DContext ctx, Camera2DVirtualEntity entity) {

            // Follow
            Vector2 pos = CalculateFollow(ctx, entity);

            // Confine
            Vector2 min = new Vector2(entity.minMaxBounds.x, entity.minMaxBounds.y);
            Vector2 max = new Vector2(entity.minMaxBounds.z, entity.minMaxBounds.w);
            GFCamera2DHelper.CalcConfinePos(pos, min, max, entity.orthographicSize, entity.aspect);

            // Apply
            entity.pos = pos;
        }

        public static Vector2 CalculateFollow(Camera2DContext ctx, Camera2DVirtualEntity entity) {
            return entity.followTargetPos + entity.followOffset;
        }

    }

}