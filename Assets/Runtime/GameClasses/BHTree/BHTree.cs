using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClasses.BehaviourTree {

    public class BHTree {

        public BHTreeNode root;
        public bool isPause;

        public BHTree() {}

        public void InitRoot(BHTreeNode root) {
            this.root = root;
        }

        public void Pause() {
            isPause = true;
        }

        public void Resume() {
            isPause = false;
        }

        public void Reset() {
            root.Reset();
        }

        public void Execute(float dt) {
            if (isPause) {
                return;
            }
            var res = root.Execute(dt);
            if (res == BHTreeNodeExecuteType.Done) {
                root.Reset();
            }
        }

    }

}
