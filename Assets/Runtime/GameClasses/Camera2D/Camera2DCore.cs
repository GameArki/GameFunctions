using UnityEngine;
using GameFunctions;
using GameClasses.Camera2DLib.Internal;

namespace GameClasses.Camera2DLib {

    public class Camera2DCore {

        Camera2DContext ctx;

        public Camera2DCore() {
            ctx = new Camera2DContext();
        }

        /// <summary>
        /// return Handle ID
        /// </summary>
        public int Init(Vector2 pos, float orthographicSize, float aspect) {
            int id = Spawn(pos, orthographicSize, aspect);
            ActivateCamera(id);
            return id;
        }

        public int Spawn(Vector2 pos, float orthographicSize, float aspect) {
            Camera2DVirtualEntity entity = new Camera2DVirtualEntity();
            entity.id = ++ctx.idRecord;
            entity.pos = pos;
            entity.orthographicSize = orthographicSize;
            entity.aspect = aspect;
            ctx.virtualRepo.Add(entity);
            return entity.id;
        }

        public void ActivateCamera(int id) {
            ctx.activeVirtualID = id;
        }

        public Camera2DExecuteResultModel Tick(float dt) {
            var activeEntity = ctx.GetActiveVirtualEntity();
            Camera2DExecuteResultModel result = Camera2DApplyDomain.Process(ctx, activeEntity, dt);
            return result;
        }

        #region Base
        public void OrthographicSize_Set(int id, float orthographicSize, float aspect) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.orthographicSize = orthographicSize;
            entity.aspect = aspect;
        }
        #endregion

        #region Follow
        public void Follow_Enable(int id, bool isEnable) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DFollowModel followModel = entity.followModel;
            followModel.isEnable = isEnable;
        }

        public void Follow_Set(int id, Vector2 targetPos, Vector2 offset, float dampingX, float dampingY) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DFollowModel followModel = entity.followModel;
            followModel.followTargetPos = targetPos;
            followModel.followOffset = offset;
            followModel.followDampingX = 0;
            followModel.followDampingY = 0;
            followModel.followDampingXOrigin = dampingX;
            followModel.followDampingYOrigin = dampingY;
        }

        public void Follow_Update(int id, Vector2 targetPos) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DFollowModel followModel = entity.followModel;
            followModel.followTargetPos = targetPos;
        }
        #endregion

        #region Confine
        public void Confine_Enable(int id, bool isEnable) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DConfineModel confineModel = entity.confineModel;
            confineModel.isEnable = isEnable;
        }

        public void Confine_Set(int id, Vector2 min, Vector2 max) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DConfineModel confineModel = entity.confineModel;
            confineModel.minMaxBounds = new Vector4(min.x, min.y, max.x, max.y);
        }
        #endregion

        #region Effect: Shake
        public void Effect_Shake_Begin(int id, Vector2 amplitude, float frequency, float duration) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DEffectShakeModel shakeModel = entity.shakeModel;
            shakeModel.isEnable = true;
            shakeModel.amplitude = amplitude;
            shakeModel.frequency = frequency;
            shakeModel.duration = duration;
            shakeModel.timer = duration;
        }

        public void Effect_Shake_Stop(int id) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DEffectShakeModel shakeModel = entity.shakeModel;
            shakeModel.isEnable = false;
        }
        #endregion

        #region Effect: ZoomIn
        public void Effect_ZoomIn_Begin(int id, GFEasingEnum easingType, float zoomInMultiply, float duration) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            zoomInModel.zoomStage = 0;
            zoomInModel.isEnable = true;
            zoomInModel.easingType = easingType;
            zoomInModel.targetMultiply = zoomInMultiply;
            zoomInModel.duration = duration;
            zoomInModel.timer = duration;
        }

        public void Effect_ZoomIn_BeginAndAutoRestore(int id, GFEasingEnum easingType, float zoomInMultiply, float duration, GFEasingEnum restoreEasingType, float restoreDuration, float restoreDelaySec) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            zoomInModel.zoomStage = 0;
            zoomInModel.isEnable = true;
            zoomInModel.easingType = easingType;
            zoomInModel.targetMultiply = zoomInMultiply;
            zoomInModel.duration = duration;
            zoomInModel.timer = duration;

            zoomInModel.isAutoRestore = true;
            zoomInModel.restoreDelaySec = restoreDelaySec;
            zoomInModel.restoreDelayTimer = restoreDelaySec;
            zoomInModel.restoreDuration = restoreDuration;
            zoomInModel.restoreEasingType = restoreEasingType;
        }

        public void Effect_ZoomIn_RestoreBegin(int id, GFEasingEnum easingType, float duration) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            zoomInModel.zoomStage = 1;
            zoomInModel.isEnable = true;
            zoomInModel.easingType = easingType;
            zoomInModel.targetMultiply = 1;
            zoomInModel.duration = duration;
            zoomInModel.timer = duration;
        }

        public void Effect_ZoomIn_Stop(int id) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            Camera2DEffectZoomInModel zoomInModel = entity.zoomInModel;
            zoomInModel.isEnable = false;
        }
        #endregion
    }

}