using System;
using System.Collections.Generic;

namespace GameClasses.BehaviourTree {

    public class BHTreeNode {

        // Whether is a container or action
        public BHTreeContainerType containerType;

        // for container and action
        public Func<bool> OnPreEnterConditionHandle;
        internal BHTreeNodeExecuteType executeType;

        // Only for action
        public Action<float> OnActionEnterHandle;
        public Func<float, BHTreeNodeExecuteType> OnActionUpdateHandle;

        // Only for container
        public List<BHTreeNode> containerChildren;
        BHTreeNode activeChild;

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

        public BHTreeNode InitAsAction(Func<bool> OnPreEnterConditionHandle, Action<float> OnActionEnterHandle, Func<float, BHTreeNodeExecuteType> OnActionUpdateHandle) {
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
            executeType = BHTreeNodeExecuteType.NotEntered;
            foreach (var child in containerChildren) {
                child.Reset();
            }
        }

        public BHTreeNodeExecuteType Execute(float dt, Random rd = null) {
            if (containerType == BHTreeContainerType.None) {
                executeType = ExecuteAction(dt);
            } else if (containerType == BHTreeContainerType.Sequence) {
                executeType = ExecuteSequence(dt);
            } else if (containerType == BHTreeContainerType.SelectorSeq) {
                executeType = ExecuteSelectorSeq(dt);
            } else if (containerType == BHTreeContainerType.SelectorRandom) {
                executeType = ExecuteSelectorRandom(dt, rd);
            } else if (containerType == BHTreeContainerType.ParallelOr) {
                executeType = ExecuteParallelOr(dt);
            } else if (containerType == BHTreeContainerType.ParallelAnd) {
                executeType = ExecuteParallelAnd(dt);
            }
            return executeType;
        }

        BHTreeNodeExecuteType ExecuteAction(float dt) {
            if (executeType == BHTreeNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    executeType = BHTreeNodeExecuteType.Running;
                    OnActionEnterHandle.Invoke(dt);
                } else {
                    executeType = BHTreeNodeExecuteType.Done;
                }
            } else if (executeType == BHTreeNodeExecuteType.Running) {
                executeType = OnActionUpdateHandle.Invoke(dt);
                if (executeType == BHTreeNodeExecuteType.NotEntered) {
                    throw new Exception("BHTreeNode ExecuteAction Error: actionNodeExecuteType can't be NotEntered or EnterFailed");
                }
            } else if (executeType == BHTreeNodeExecuteType.Done) {
                // Do nothing
            }
            return executeType;
        }

        BHTreeNodeExecuteType ExecuteSequence(float dt) {
            if (executeType == BHTreeNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    executeType = BHTreeNodeExecuteType.Running;
                } else {
                    executeType = BHTreeNodeExecuteType.Done;
                }
            } else if (executeType == BHTreeNodeExecuteType.Running) {
                int totalCount = containerChildren.Count;
                int successCount = 0;
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeNodeExecuteType.Done) {
                        successCount += 1;
                    } else if (childResult == BHTreeNodeExecuteType.Running) {
                        break;
                    }
                }
                if (successCount == totalCount) {
                    executeType = BHTreeNodeExecuteType.Done;
                } else {
                    executeType = BHTreeNodeExecuteType.Running;
                }
            } else if (executeType == BHTreeNodeExecuteType.Done) {
                // Do nothing
            }
            return executeType;
        }

        BHTreeNodeExecuteType ExecuteSelectorSeq(float dt) {
            if (executeType == BHTreeNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    executeType = BHTreeNodeExecuteType.Running;
                } else {
                    executeType = BHTreeNodeExecuteType.Done;
                }
            } else if (executeType == BHTreeNodeExecuteType.Running) {
                if (activeChild != null) {
                    executeType = activeChild.Execute(dt);
                    if (executeType == BHTreeNodeExecuteType.Done) {
                        activeChild = null;
                    }
                } else {
                    for (int i = 0; i < containerChildren.Count; i += 1) {
                        var child = containerChildren[i];
                        executeType = child.Execute(dt);
                        if (executeType == BHTreeNodeExecuteType.NotEntered) {
                            continue;
                        } else if (executeType == BHTreeNodeExecuteType.Done) {
                            break;
                        } else if (executeType == BHTreeNodeExecuteType.Running) {
                            activeChild = child;
                            break;
                        }
                    }
                }
            } else if (executeType == BHTreeNodeExecuteType.Done) {
                // Do nothing
            }
            return executeType;
        }

        static Random staticRD = new Random();
        BHTreeNodeExecuteType ExecuteSelectorRandom(float dt, Random rd = null) {
            if (executeType == BHTreeNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    executeType = BHTreeNodeExecuteType.Running;
                } else {
                    executeType = BHTreeNodeExecuteType.Done;
                }
            } else if (executeType == BHTreeNodeExecuteType.Running) {
                if (activeChild != null) {
                    executeType = activeChild.Execute(dt);
                    if (executeType == BHTreeNodeExecuteType.Done) {
                        activeChild = null;
                    }
                } else {
                    if (rd == null) {
                        rd = staticRD;
                    }
                    var randomIndex = rd.Next(0, containerChildren.Count);
                    var child = containerChildren[randomIndex];
                    executeType = child.Execute(dt);
                    if (executeType == BHTreeNodeExecuteType.Running) {
                        activeChild = child;
                    }
                }
            } else if (executeType == BHTreeNodeExecuteType.Done) {
                // Do nothing
            }
            return executeType;
        }

        BHTreeNodeExecuteType ExecuteParallelOr(float dt) {
            if (executeType == BHTreeNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    executeType = BHTreeNodeExecuteType.Running;
                } else {
                    executeType = BHTreeNodeExecuteType.Done;
                }
            } else if (executeType == BHTreeNodeExecuteType.Running) {
                // Parallel Or Mode
                int successCount = 0;
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeNodeExecuteType.Done) {
                        successCount += 1;
                    }
                }
                if (successCount > 0) {
                    executeType = BHTreeNodeExecuteType.Done;
                } else {
                    executeType = BHTreeNodeExecuteType.Running;
                }
            } else if (executeType == BHTreeNodeExecuteType.Done) {
                // Do nothing
            }
            return executeType;
        }

        BHTreeNodeExecuteType ExecuteParallelAnd(float dt) {
            if (executeType == BHTreeNodeExecuteType.NotEntered) {
                if (OnPreEnterConditionHandle == null || OnPreEnterConditionHandle.Invoke()) {
                    executeType = BHTreeNodeExecuteType.Running;
                } else {
                    executeType = BHTreeNodeExecuteType.Done;
                }
            } else if (executeType == BHTreeNodeExecuteType.Running) {
                // Parallel And Mode
                int successCount = 0;
                for (int i = 0; i < containerChildren.Count; i += 1) {
                    var child = containerChildren[i];
                    var childResult = child.Execute(dt);
                    if (childResult == BHTreeNodeExecuteType.Done) {
                        successCount += 1;
                    }
                }
                if (successCount == containerChildren.Count) {
                    executeType = BHTreeNodeExecuteType.Done;
                } else {
                    executeType = BHTreeNodeExecuteType.Running;
                }
            } else if (executeType == BHTreeNodeExecuteType.Done) {
                // Do nothing
            }
            return executeType;
        }

    }

}