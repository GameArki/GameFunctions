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
            if (ctx.activeVirtualID == 0) {
                ctx.activeVirtualID = id;
            }
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

        public Camera2DExecuteResultModel Tick(float dt) {

            var activeEntity = ctx.GetActiveVirtualEntity();
            Camera2DApplyDomain.Process(ctx, activeEntity, dt);

            Camera2DExecuteResultModel result = new Camera2DExecuteResultModel();
            result.pos = activeEntity.pos;

            return result;

        }

        // Follow
        #region Follow
        public void Follow_Enable(int id, bool isEnable) {
            var entity = ctx.virtualRepo.Get(id);
            entity.isFollow = isEnable;
        }

        public void Follow_Set(int id, Vector2 targetPos, Vector2 offset, float dampingX, float dampingY) {
            var entity = ctx.virtualRepo.Get(id);
            entity.followTargetPos = targetPos;
            entity.followOffset = offset;
            entity.followDampingX = 0;
            entity.followDampingY = 0;
            entity.followDampingXOrigin = dampingX;
            entity.followDampingYOrigin = dampingY;
        }

        public void Follow_Update(int id, Vector2 targetPos) {
            var entity = ctx.virtualRepo.Get(id);
            entity.followTargetPos = targetPos;
        }
        #endregion

        // Confine
        #region Confine
        public void Confine_Enable(int id, bool isEnable) {
            var entity = ctx.virtualRepo.Get(id);
            entity.isConfine = isEnable;
        }

        public void Confine_Set(int id, float orthographicSize, float aspect, Vector2 min, Vector2 max) {
            var entity = ctx.virtualRepo.Get(id);
            entity.orthographicSize = orthographicSize;
            entity.aspect = aspect;
            entity.minMaxBounds = new Vector4(min.x, min.y, max.x, max.y);
        }
        #endregion

    }

}