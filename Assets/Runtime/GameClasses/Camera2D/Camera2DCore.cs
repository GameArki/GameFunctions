using UnityEngine;
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
            entity.pos_true = pos;
            entity.pos_final = pos;
            entity.orthographicSize_true = orthographicSize;
            entity.orthographicSize_final = orthographicSize;
            entity.aspect_true = aspect;
            entity.aspect_final = aspect;
            ctx.virtualRepo.Add(entity);
            return entity.id;
        }

        public void ActivateCamera(int id) {
            ctx.activeVirtualID = id;
        }

        public Camera2DExecuteResultModel Tick(float dt) {

            var activeEntity = ctx.GetActiveVirtualEntity();
            Camera2DApplyDomain.Process(ctx, activeEntity, dt);

            Camera2DExecuteResultModel result;
            result.pos = activeEntity.pos_final;
            result.orthographicSize = activeEntity.orthographicSize_final;
            result.aspect = activeEntity.aspect_final;

            return result;
        }

        #region Base
        public void OrthographicSize_Set(int id, float orthographicSize, float aspect) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.orthographicSize_true = orthographicSize;
            entity.orthographicSize_final = orthographicSize;
            entity.aspect_true = aspect;
            entity.aspect_final = aspect;
            Debug.Log($"CameraHandleID: {id} OrthographicSize: {orthographicSize} Aspect: {aspect}");
        }
        #endregion

        #region Follow
        public void Follow_Enable(int id, bool isEnable) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.isFollow = isEnable;
        }

        public void Follow_Set(int id, Vector2 targetPos, Vector2 offset, float dampingX, float dampingY) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.followTargetPos = targetPos;
            entity.followOffset = offset;
            entity.followDampingX = 0;
            entity.followDampingY = 0;
            entity.followDampingXOrigin = dampingX;
            entity.followDampingYOrigin = dampingY;
        }

        public void Follow_Update(int id, Vector2 targetPos) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.followTargetPos = targetPos;
        }
        #endregion

        #region Confine
        public void Confine_Enable(int id, bool isEnable) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.isConfine = isEnable;
        }

        public void Confine_Set(int id, Vector2 min, Vector2 max) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.minMaxBounds = new Vector4(min.x, min.y, max.x, max.y);
        }
        #endregion

        #region Shake
        public void Effect_Shake_Begin(int id, Vector2 amplitude, float frequency, float duration) {
            var entity = ctx.virtualRepo.Get(id);
            if (entity == null) {
                Debug.LogError($"CameraHandleID: {id} not found");
                return;
            }
            entity.isShake = true;
            entity.shakeAmplitude = amplitude;
            entity.shakeFrequency = frequency;
            entity.shakeDuration = duration;
            entity.shakeTimer = duration;
        }
        #endregion
    }

}