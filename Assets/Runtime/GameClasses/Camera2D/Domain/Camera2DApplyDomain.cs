using UnityEngine;
using GameFunctions;

namespace GameClasses.Camera2DLib.Internal {

    public static class Camera2DApplyDomain {

        public static Camera2DExecuteResultModel Process(Camera2DContext ctx, Camera2DVirtualEntity entity, float dt) {

            // ==== Normal ====
            // Follow
            Vector2 pos = Follow_Process(ctx, entity, dt);

            // Zoom
            float orthographicSize_true = entity.orthographicSize;

            // Effect: ZoomIn
            float orthographicSize_final = Effect_ZoomIn_Process(ctx, entity, dt);

            // Confine
            pos = Confine_Calculate(ctx, entity, orthographicSize_final, pos);

            // True: Pos, OrthographicSize
            entity.pos = pos;
            entity.orthographicSize = orthographicSize_true;

            // ==== Effect ====
            // Effect: Shake
            pos = Effect_Shake_Process(ctx, entity, pos, dt);

            // ==== Final ====
            Camera2DExecuteResultModel res;
            res.pos = pos;
            res.orthographicSize = orthographicSize_final;
            res.aspect = entity.aspect;
            return res;

        }

        #region Follow
        static Vector2 Follow_Process(Camera2DContext ctx, Camera2DVirtualEntity et, float dt) {

            Camera2DFollowModel followModel = et.followModel;
            Vector2 truePos = et.pos;
            if (!followModel.isEnable) {
                return truePos;
            }

            // - DeadZone
            Vector2 followPos = truePos;
            Vector2 targetPos = followModel.followTargetPos;
            Vector2 deadZoneSize = followModel.deadZoneSize;
            Vector2 posDiff = targetPos - truePos;
            bool isOverDeadSize = false;
            if (posDiff.x >= deadZoneSize.x || posDiff.x <= -deadZoneSize.x) {
                followPos.x = targetPos.x;
                isOverDeadSize = true;
            }
            if (posDiff.y >= deadZoneSize.y || posDiff.y <= -deadZoneSize.y) {
                followPos.y = targetPos.y;
                isOverDeadSize = true;
            }

            if (!isOverDeadSize) {
                return truePos;
            }

            targetPos = followPos + followModel.followOffset;

            float xDir = Mathf.Sign(targetPos.x - truePos.x);
            if (xDir != 0) {
                if (followModel.lastDampingDirX != xDir) {
                    followModel.lastDampingDirX = xDir;
                    followModel.followDampingX = 0;
                }
            }

            float yDir = Mathf.Sign(targetPos.y - truePos.y);
            if (yDir != 0) {
                if (followModel.lastDampingDirY != yDir) {
                    followModel.lastDampingDirY = yDir;
                    followModel.followDampingY = 0;
                }
            }

            // Damping
            float x;
            if (followModel.followDampingXOrigin == 0) {
                x = targetPos.x;
            } else {
                ref float xDamping = ref followModel.followDampingX;
                xDamping += dt;
                xDamping = Mathf.Min(xDamping, followModel.followDampingXOrigin);
                float percent = xDamping / followModel.followDampingXOrigin;
                x = Mathf.Lerp(truePos.x, targetPos.x, percent);
            }

            float y;
            if (followModel.followDampingYOrigin == 0) {
                y = targetPos.y;
            } else {
                ref float yDamping = ref followModel.followDampingY;
                yDamping += dt;
                yDamping = Mathf.Min(yDamping, followModel.followDampingYOrigin);
                float percent = yDamping / followModel.followDampingYOrigin;
                y = Mathf.Lerp(truePos.y, targetPos.y, percent);
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
        static Vector2 Confine_Calculate(Camera2DContext ctx, Camera2DVirtualEntity entity, float orthographicSize, Vector2 pos) {
            Camera2DConfineModel confineModel = entity.confineModel;
            if (!confineModel.isEnable) {
                return pos;
            }
            Vector2 min = new Vector2(confineModel.minMaxBounds.x, confineModel.minMaxBounds.y);
            Vector2 max = new Vector2(confineModel.minMaxBounds.z, confineModel.minMaxBounds.w);
            pos = GFCamera2DHelper.CalcConfinePos(pos, min, max, orthographicSize, entity.aspect);
            return pos;
        }
        #endregion

        #region Effect: Shake
        static Vector2 Effect_Shake_Process(Camera2DContext ctx, Camera2DVirtualEntity entity, Vector2 pos, float dt) {
            Camera2DEffectShakeModel shakeModel = entity.shakeModel;
            if (!shakeModel.isEnable) {
                return pos;
            }
            ref float timer = ref shakeModel.timer;
            float duration = shakeModel.duration;
            float frequency = shakeModel.frequency;
            Vector2 amplitude = shakeModel.amplitude;
            timer -= dt;
            if (timer <= 0) {
                shakeModel.isEnable = false;
                timer = 0;
                return pos;
            }
            float percent = (duration - timer) / duration;
            amplitude *= timer / duration;
            float x = Mathf.Sin(timer * frequency) * amplitude.x;
            float y = Mathf.Sin(timer * frequency) * amplitude.y;
            return pos + new Vector2(x, y);
        }
        #endregion

        #region Effect: ZoomIn
        static float Effect_ZoomIn_Process(Camera2DContext ctx, Camera2DVirtualEntity entity, float dt) {
            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            float res = entity.orthographicSize;
            if (!zoomInModel.isEnable) {
                return res;
            }
            if (zoomInModel.zoomStage == 0) {
                res = Effect_ZoomIn_Normal_Process(ctx, entity, dt);
            } else if (zoomInModel.zoomStage == 1) {
                res = Effect_ZoomIn_Restore_Process(ctx, entity, dt);
            }
            return res;
        }

        static float Effect_ZoomIn_Normal_Process(Camera2DContext ctx, Camera2DVirtualEntity entity, float dt) {
            float res = entity.orthographicSize;

            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            float orthographicSize_true = entity.orthographicSize;
            ref float timer = ref zoomInModel.timer;
            float duration = zoomInModel.duration;
            GFEasingEnum easingType = zoomInModel.easingType;
            float zoomInMultiply = zoomInModel.targetMultiply;

            float passTime = duration - timer;
            if (timer > 0) {
                timer -= dt;
            } else {
                timer = 0;
                // - Auto Restore
                if (zoomInModel.isAutoRestore) {
                    if (zoomInModel.restoreDelayTimer > 0) {
                        zoomInModel.restoreDelayTimer -= dt;
                    } else {
                        zoomInModel.restoreDelayTimer = 0;
                        zoomInModel.isAutoRestore = false;
                        zoomInModel.targetMultiply = 1;
                        zoomInModel.easingType = zoomInModel.restoreEasingType;
                        zoomInModel.duration = zoomInModel.restoreDuration;
                        zoomInModel.timer = zoomInModel.duration;
                        zoomInModel.zoomStage = 1;
                    }
                }
            }

            // ZoomIn 2x Means: 0.5 * orthographicSize, then show less scene
            float rate = 1 / zoomInMultiply;
            float start = orthographicSize_true;
            float end = orthographicSize_true * rate;
            res = GFEasing.Ease1D(easingType, passTime, duration, start, end);

            zoomInModel.lastFinalOrthographicSize = res;
            return res;
        }
        static float Effect_ZoomIn_Restore_Process(Camera2DContext ctx, Camera2DVirtualEntity entity, float dt) {

            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            float orthographicSize_true = entity.orthographicSize;
            ref float timer = ref zoomInModel.timer;
            float duration = zoomInModel.duration;
            GFEasingEnum easingType = zoomInModel.easingType;
            float start = zoomInModel.lastFinalOrthographicSize;
            float end = orthographicSize_true;

            float passTime = duration - timer;
            if (timer > 0) {
                timer -= dt;
            } else {
                timer = 0;
                zoomInModel.isEnable = false;
                zoomInModel.zoomStage = 0;
            }

            float res = GFEasing.Ease1D(easingType, passTime, duration, start, end);
            return res;
        }
        #endregion

    }

}