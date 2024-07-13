using System;
using System.Collections.Generic;

namespace GameClasses.BehaviourTree {

    public class BHTreeNode {

        // Whether is a container or action
        public BHTreeContainerType containerType;

        // PreEnterCondition for container and action
        public Func<bool> OnPreEnterConditionHandle;

        // Only for action
        public Action<float> OnActionEnterHandle;
        public Func<float, BHTreeActionNodeExecuteType> OnActionUpdateHandle;
        internal BHTreeActionNodeExecuteType actionNodeExecuteType;

        // Only for container
        public List<BHTreeNode> containerChildren;

        // TODO: Owner, Self Data

        public BHTreeNode() {
            containerChildren = new List<BHTreeNode>();
        }

        public BHTreeNode InitAsContainer(BHTreeContainerType containerType, Func<bool> OnPreEnterConditionHandle) {
            if (containerType == BHTreeContainerType.None) {
                throw new Exception("BHTreeNode InitAsContainer Error: containerType can't be None");
            }
            this.containerType = containerType;
            this.OnPreEnterConditionHandle = OnPreEnterConditionHandle;
            return this;
        }

        public BHTreeNode InitAsAction(Func<bool> OnPreEnterConditionHandle, Action<float> OnActionEnterHandle, Func<float, BHTreeActionNodeExecuteType> OnActionUpdateHandle) {
            this.containerType = BHTreeContainerType.None;
            this.OnPreEnterConditionHandle = OnPreEnterConditionHandle;
            this.OnActionEnterHandle = OnActionEnterHandle;
            this.OnActionUpdateHandle = OnActionUpdateHandle;
            return this;
        }

        public void AddChild(BHTreeNode child) {
            containerChildren.Add(child);
        }

        // If root done, reset all nodes
        public void Reset() {
            actionNodeExecuteType = BHTreeActionNodeExecuteType.NotEntered;
            foreach (var child in containerChildren) {
                child.Reset();
            }
        }

        public BHTreeContainerNodeExecuteType Execute(float dt, Random rd = null) {
            BHTreeContainerNodeExecuteType containerNodeExecuteType = BHTreeContainerNodeExecuteType.NotEntered;
            if (containerType == BHTreeContainerType.None) {
                // Action
                var actionRes = ExecuteAction(dt);
                if (actionRes == BHTreeActionNodeExecuteType.EnterFailed) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                } else if (actionRes == BHTreeActionNodeExecuteType.Done) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                } else {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Running;
                }
            } else if (containerType == BHTreeContainerType.Sequence) {
                // Sequence
                int totalCount = containerChildren.Count;
                int successCount = 0;
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeContainerNodeExecuteType.Done) {
                        successCount += 1;
                    } else if (childResult == BHTreeContainerNodeExecuteType.Running) {
                        break;
                    }
                }
                if (successCount == totalCount) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                } else {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Running;
                }
            } else if (containerType == BHTreeContainerType.SelectorSeq) {
                // Selector Sequence Mode
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeContainerNodeExecuteType.Done) {
                        containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                        break;
                    } else if (childResult == BHTreeContainerNodeExecuteType.Running) {
                        containerNodeExecuteType = BHTreeContainerNodeExecuteType.Running;
                        break;
                    }
                }
            } else if (containerType == BHTreeContainerType.SelectorRandom) {
                // Selector Random Mode
                if (rd == null) {
                    throw new Exception("BHTreeNode Execute Error: SelectorRandom need Random instance");
                }
                var randomIndex = rd.Next(0, containerChildren.Count);
                var child = containerChildren[randomIndex];
                var childResult = child.Execute(dt);
                if (childResult == BHTreeContainerNodeExecuteType.Done) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                } else if (childResult == BHTreeContainerNodeExecuteType.Running) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Running;
                }
            } else if (containerType == BHTreeContainerType.ParallelOr) {
                // Parallel Or Mode
                int successCount = 0;
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeContainerNodeExecuteType.Done) {
                        successCount += 1;
                    }
                }
                if (successCount > 0) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                } else {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Running;
                }
            } else if (containerType == BHTreeContainerType.ParallelAnd) {
                // Parallel And Mode
                int successCount = 0;
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeContainerNodeExecuteType.Done) {
                        successCount += 1;
                    }
                }
                if (successCount == containerChildren.Count) {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Done;
                } else {
                    containerNodeExecuteType = BHTreeContainerNodeExecuteType.Running;
                }
            }

            return containerNodeExecuteType;
        }

        BHTreeActionNodeExecuteType ExecuteAction(float dt) {
            if (actionNodeExecuteType == BHTreeActionNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    actionNodeExecuteType = BHTreeActionNodeExecuteType.Running;
                    OnActionEnterHandle.Invoke(dt);
                } else {
                    actionNodeExecuteType = BHTreeActionNodeExecuteType.EnterFailed;
                }
            } else if (actionNodeExecuteType == BHTreeActionNodeExecuteType.Running) {
                actionNodeExecuteType = OnActionUpdateHandle.Invoke(dt);
                if (actionNodeExecuteType == BHTreeActionNodeExecuteType.NotEntered || actionNodeExecuteType == BHTreeActionNodeExecuteType.EnterFailed) {
                    throw new Exception("BHTreeNode ExecuteAction Error: actionNodeExecuteType can't be NotEntered or EnterFailed");
                }
            } else if (actionNodeExecuteType == BHTreeActionNodeExecuteType.Done) {
                // Do nothing
            }
            return actionNodeExecuteType;
        }

    }

}