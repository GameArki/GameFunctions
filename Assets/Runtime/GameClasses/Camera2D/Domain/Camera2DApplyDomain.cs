using System;
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
            if (!followModel.isEnable) {
                return et.pos;
            }

            // x
            float xCenterPos = et.pos.x;
            float xTargetPos = followModel.followTargetPos.x;
            float xMoveDir = Math.Sign(xTargetPos - followModel.lastFollowTargetPos.x);
            float xDiff = xTargetPos - xCenterPos;
            float xDeadHalfSize = followModel.deadZoneHalfSize.x;
            float xDiffAbs = Mathf.Abs(xDiff);
            ref float xDamping = ref followModel.followDampingX;
            if (xDeadHalfSize != 0) {
                if (xDiff > xDeadHalfSize || xDiff < -xDeadHalfSize) {
                    float xSign = Mathf.Sign(xDiff);
                    float xCenterOffset = (xDiffAbs - xDeadHalfSize) * xSign;
                    xTargetPos = xCenterPos + followModel.followOffset.x + xCenterOffset;
                } else {
                    xTargetPos = xCenterPos;
                    xDamping = 0;
                }
            }

            float x = xTargetPos;
            if (followModel.followDampingXOrigin == 0) {
                x = xTargetPos;
            } else {
                xDamping += dt;
                xDamping = Mathf.Clamp(xDamping, 0, followModel.followDampingXOrigin);
                float percent = xDamping / followModel.followDampingXOrigin;
                x = Mathf.Lerp(xCenterPos, xTargetPos, percent);
                Debug.Log("xDamping: " + x + " percent" + percent);
            }

            ref float xLastDir = ref followModel.followDampingLastDirX;
            if (xLastDir != xMoveDir) {
                xDamping = 0;
                Debug.Log("Reset");
            }
            xLastDir = xMoveDir;
            if (Math.Abs(x - xTargetPos) <= float.Epsilon) {

            }

            // y
            float yCenterPos = et.pos.y;
            float yTargetPos = followModel.followTargetPos.y;
            float yMoveDir = Math.Sign(yTargetPos - followModel.lastFollowTargetPos.y);
            float yDiff = yTargetPos - yCenterPos;
            float yDeadHalfSize = followModel.deadZoneHalfSize.y;
            float yDiffAbs = Mathf.Abs(yDiff);
            ref float yDamping = ref followModel.followDampingY;
            if (yDeadHalfSize != 0) {
                if (yDiff > yDeadHalfSize || yDiff < -yDeadHalfSize) {
                    float ySign = Mathf.Sign(yDiff);
                    float yCenterOffset = (yDiffAbs - yDeadHalfSize) * ySign;
                    yTargetPos = yCenterPos + followModel.followOffset.y + yCenterOffset;
                } else {
                    yTargetPos = yCenterPos;
                    yDamping = 0;
                }
            }

            float y = yTargetPos;
            if (followModel.followDampingYOrigin == 0) {
                y = yTargetPos;
            } else {
                yDamping += dt;
                yDamping = Mathf.Clamp(yDamping, 0, followModel.followDampingYOrigin);
                float percent = yDamping / followModel.followDampingYOrigin;
                y = Mathf.Lerp(yCenterPos, yTargetPos, percent);
            }

            ref float yLastDir = ref followModel.followDampingLastDirY;
            if (yLastDir != yMoveDir) {
                yDamping = 0;
            }
            yLastDir = yMoveDir;
            if (Math.Abs(y - yTargetPos) <= float.Epsilon) {

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