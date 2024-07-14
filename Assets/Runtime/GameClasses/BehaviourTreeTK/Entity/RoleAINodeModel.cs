using System;
using System.Collections.Generic;

namespace GameFunctions.BehaviourTreeTK {

    public class RoleAINodeModel {

        public RoleAINodeType nodeType;
        public RoleAIPreconditionModel preconditionModel;
        public RoleAINodeExecuteType executeType;

        public List<RoleAINodeModel> children;
        public RoleAINodeModel activeChild;

        public RoleAINodeModel() {}

        public void InitAsAction(RoleAIPreconditionModel preconditionModel) {
            this.nodeType = RoleAINodeType.Action;
            this.preconditionModel = preconditionModel;
            this.executeType = RoleAINodeExecuteType.NotEntered;
            this.children = null;
        }

        public void InitAsContainer(RoleAINodeType nodeType, RoleAIPreconditionModel preconditionModel) {
            if (nodeType == RoleAINodeType.Action) {
                throw new Exception("RoleAINodeModel.InitAsContainer: nodeType cannot be Action");
            }
            this.nodeType = nodeType;
            this.preconditionModel = preconditionModel;
            this.executeType = RoleAINodeExecuteType.NotEntered;
            this.children = new List<RoleAINodeModel>();
        }

        public void Reset() {
            this.executeType = RoleAINodeExecuteType.NotEntered;
            if (this.children != null) {
                foreach (RoleAINodeModel child in this.children) {
                    child.Reset();
                }
            }
        }

    }

}