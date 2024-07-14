using UnityEngine;
using GameClasses.Camera2DLib.Internal;

namespace GameClasses.Camera2DLib {

    public class Camera2DCore {

        Camera2DContext ctx;

        public Camera2DCore() {
            ctx = new Camera2DContext();
        }

        public void Init(Vector2 pos, float orthographicSize, float aspect) {
            int id = Spawn(pos, orthographicSize, aspect);
            if (ctx.activeVirtualID == 0) {
                ctx.activeVirtualID = id;
            }
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

            Camera2DExecuteResultModel result = new Camera2DExecuteResultModel();
            result.pos = activeEntity.pos;

            return result;

        }

        // Follow
        #region Follow
        public void Follow_Set(int id, Vector2 targetPos, Vector2 offset) {
            var entity = ctx.virtualRepo.Get(id);
            entity.followTargetPos = targetPos;
            entity.followOffset = offset;
        }

        public void Follow_Set(Vector2 targetPos, Vector2 offset) {
            Follow_Set(ctx.activeVirtualID, targetPos, offset);
        }

        public void Follow_Update(int id, Vector2 targetPos) {
            var entity = ctx.virtualRepo.Get(id);
            entity.followTargetPos = targetPos;
        }

        public void Follow_Update(Vector2 targetPos) {
            Follow_Update(ctx.activeVirtualID, targetPos);
        }
        #endregion

        // Confine
        #region Confine
        public void Confine_Set(int id, Vector2 min, Vector2 max) {
            var entity = ctx.virtualRepo.Get(id);
            entity.minMaxBounds = new Vector4(min.x, min.y, max.x, max.y);
        }

        public void Confine_Set(Vector2 min, Vector2 max) {
            Confine_Set(ctx.activeVirtualID, min, max);
        }
        #endregion

    }

}