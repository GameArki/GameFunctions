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
                pos = entity.pos_true;
            }

            // Zoom
            float orthographicSize;
            orthographicSize = Zoom_Calculate(ctx, entity, entity.orthographicSize_true);

            // Confine
            if (entity.isConfine) {
                pos = Confine_Calculate(ctx, entity, pos);
            }

            // True Pos
            entity.pos_true = pos;

            // Shake
            if (entity.isShake) {
                pos = Shake_Calculate(ctx, entity, pos, dt);
            }

            // Final Pos
            entity.pos_final = pos;
            entity.orthographicSize_final = orthographicSize;
            entity.aspect_final = entity.aspect_true;

        }

        #region Follow
        static Vector2 Follow_Process(Camera2DContext ctx, Camera2DVirtualEntity et, float dt) {

            Vector2 targetPos = et.followTargetPos + et.followOffset;
            Vector2 camTruePos = et.pos_true;

            // Damping
            float x;
            if (et.followDampingXOrigin == 0) {
                x = targetPos.x;
            } else {
                et.followDampingX += dt;
                float percent = et.followDampingX / et.followDampingXOrigin;
                x = Mathf.Lerp(camTruePos.x, targetPos.x, percent);
            }

            float y;
            if (et.followDampingYOrigin == 0) {
                y = targetPos.y;
            } else {
                et.followDampingY += dt;
                float percent = et.followDampingY / et.followDampingYOrigin;
                y = Mathf.Lerp(camTruePos.y, targetPos.y, percent);
            }

            if (Mathf.Approximately(x, camTruePos.x) && Mathf.Approximately(y, camTruePos.y)) {
                et.followDampingX = 0;
                et.followDampingY = 0;
            }

            return new Vector2(x, y);

        }
        #endregion

        #region Zoom
        static float Zoom_Calculate(Camera2DContext ctx, Camera2DVirtualEntity entity, float orthographicSize) {
            return orthographicSize;
        }
        #endregion

        #region Confine
        static Vector2 Confine_Calculate(Camera2DContext ctx, Camera2DVirtualEntity entity, Vector2 pos) {
            Vector2 min = new Vector2(entity.minMaxBounds.x, entity.minMaxBounds.y);
            Vector2 max = new Vector2(entity.minMaxBounds.z, entity.minMaxBounds.w);
            pos = GFCamera2DHelper.CalcConfinePos(pos, min, max, entity.orthographicSize_final, entity.aspect_final);
            return pos;
        }
        #endregion

        #region Shake
        static Vector2 Shake_Calculate(Camera2DContext ctx, Camera2DVirtualEntity entity, Vector2 pos, float dt) {
            ref float timer = ref entity.shakeTimer;
            float duration = entity.shakeDuration;
            float frequency = entity.shakeFrequency;
            Vector2 amplitude = entity.shakeAmplitude;
            timer -= dt;
            if (timer <= 0) {
                entity.isShake = false;
                timer = 0;
                return pos;
            }
            float percent = (duration - timer) / duration;
            float x = Mathf.Sin(percent * frequency) * amplitude.x;
            float y = Mathf.Sin(percent * frequency) * amplitude.y;
            return pos + new Vector2(x, y);
        }
        #endregion

    }

}