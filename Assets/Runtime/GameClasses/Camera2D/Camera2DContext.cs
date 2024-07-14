using System.Collections.Generic;

namespace GameClasses.Camera2DLib.Internal {

    public class Camera2DContext {

        public int activeVirtualID;
        public Camera2DVirtualRepo virtualRepo;

        public int idRecord;

        public Camera2DContext() { }

        public Camera2DVirtualEntity GetActiveVirtualEntity() {
            return virtualRepo.Get(activeVirtualID);
        }

    }

}