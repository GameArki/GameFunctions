using System.Collections.Generic;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DVirtualRepo {

        Dictionary<int, Camera2DVirtualEntity> all;

        public Camera2DVirtualRepo() {
            all = new  Dictionary<int, Camera2DVirtualEntity>();
        }

        public void Add(Camera2DVirtualEntity entity) {
            all.Add(entity.id, entity);
        }

        public void Remove(Camera2DVirtualEntity entity) {
            all.Remove(entity.id);
        }

        public Camera2DVirtualEntity Get(int id) {
            return all[id];
        }

    }

}