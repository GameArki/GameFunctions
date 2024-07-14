using System;

namespace GameFunctions.BehaviourTreeTK.Domains {

    // 构建树
    // 执行树
    public static class RoleAIDomain {

        public static void Tick(GameContext ctx, RoleEntity entity, float fixdt) {
            var ai = entity.aiCom;
            var root = ai.root;
            if (root == null) {
                return;
            }
            var res = ExecuteNode(ctx, entity, root, fixdt);
            if (res == RoleAINodeExecuteType.Done) {
                root.Reset();
            }
        }

        #region BTTree Implementation
        static RoleAINodeExecuteType ExecuteNode(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            RoleAINodeType nodeType = node.nodeType;
            if (nodeType == RoleAINodeType.Action) {
                node.executeType = ExecuteAction(ctx, entity, node, fixdt);
            } else if (nodeType == RoleAINodeType.Sequence) {
                node.executeType = ExecuteSequence(ctx, entity, node, fixdt);
            } else if (nodeType == RoleAINodeType.SelectorSeq) {
                node.executeType = ExecuteSelectorSeq(ctx, entity, node, fixdt);
            } else if (nodeType == RoleAINodeType.SelectorRandom) {
                node.executeType = ExecuteSelectorRandom(ctx, entity, node, fixdt);
            } else if (nodeType == RoleAINodeType.ParallelAnd) {
                node.executeType = ExecuteParallelAnd(ctx, entity, node, fixdt);
            } else if (nodeType == RoleAINodeType.ParallelOr) {
                node.executeType = ExecuteParallelOr(ctx, entity, node, fixdt);
            }
            return node.executeType;
        }

        // Action 节点, 即行为(非容器)
        static RoleAINodeExecuteType ExecuteAction(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            if (node.executeType == RoleAINodeExecuteType.NotEntered) {
                if (!CheckPrecondition(ctx, entity, node.preconditionModel, fixdt)) {
                    // Failed precondition means done
                    node.executeType = RoleAINodeExecuteType.Done;
                } else {
                    node.executeType = RoleAINodeExecuteType.Running;
                    Action_Enter(ctx, entity, node, fixdt);
                }
            } else if (node.executeType == RoleAINodeExecuteType.Running) {
                node.executeType = Action_Update(ctx, entity, node, fixdt);
            } else {
                // Done, do nothing
            }
            return node.executeType;
        }

        // 同时只执行一个子节点, 顺序执行非Done子节点, 所有子节点Done, 就算容器Done
        static RoleAINodeExecuteType ExecuteSequence(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            ref var exeType = ref node.executeType;
            if (exeType == RoleAINodeExecuteType.NotEntered) {
                if (!CheckPrecondition(ctx, entity, node.preconditionModel, fixdt)) {
                    // Failed precondition means done
                    exeType = RoleAINodeExecuteType.Done;
                } else {
                    exeType = RoleAINodeExecuteType.Running;
                }
            } else if (exeType == RoleAINodeExecuteType.Running) {
                int totalCount = node.children.Count;
                int doneCount = 0;
                foreach (RoleAINodeModel child in node.children) {
                    RoleAINodeExecuteType res = ExecuteNode(ctx, entity, child, fixdt);
                    if (res == RoleAINodeExecuteType.Done) {
                        doneCount++;
                    } else {
                        break;
                    }
                }
                if (doneCount == totalCount) {
                    exeType = RoleAINodeExecuteType.Done;
                }
            } else {
                // Done, do nothing
            }
            return exeType;
        }

        // 同时只执行一个子节点, 首次按顺序选中一个可进入的子节点, 只要该子节点Done, 就算容器Done
        static RoleAINodeExecuteType ExecuteSelectorSeq(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            ref var exeType = ref node.executeType;
            if (exeType == RoleAINodeExecuteType.NotEntered) {
                if (!CheckPrecondition(ctx, entity, node.preconditionModel, fixdt)) {
                    // Failed precondition means done
                    exeType = RoleAINodeExecuteType.Done;
                } else {
                    exeType = RoleAINodeExecuteType.Running;
                }
            } else if (exeType == RoleAINodeExecuteType.Running) {
                if (node.activeChild != null) {
                    exeType = ExecuteNode(ctx, entity, node.activeChild, fixdt);
                    if (exeType == RoleAINodeExecuteType.Done) {
                        node.activeChild = null;
                    }
                } else {
                    int doneCount = 0;
                    foreach (RoleAINodeModel child in node.children) {
                        RoleAINodeExecuteType res = ExecuteNode(ctx, entity, child, fixdt);
                        if (res == RoleAINodeExecuteType.Done) {
                            doneCount++;
                            break;
                        } else if (res == RoleAINodeExecuteType.Running) {
                            node.activeChild = child;
                            break;
                        }
                    }
                    if (doneCount > 0) {
                        exeType = RoleAINodeExecuteType.Done;
                    }
                }
            } else {
                // Done, do nothing
            }
            return exeType;
        }

        // 同时只执行一个子节点, 首次随机选中一个可进入的子节点, 只要该子节点Done, 就算容器Done
        static RoleAINodeExecuteType ExecuteSelectorRandom(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            ref var exeType = ref node.executeType;
            if (exeType == RoleAINodeExecuteType.NotEntered) {
                if (!CheckPrecondition(ctx, entity, node.preconditionModel, fixdt)) {
                    // Failed precondition means done
                    exeType = RoleAINodeExecuteType.Done;
                } else {
                    exeType = RoleAINodeExecuteType.Running;
                    var children = node.children;
                    // Shuffle
                    for (int i = 0; i < children.Count; i++) {
                        int j = UnityEngine.Random.Range(i, children.Count);
                        RoleAINodeModel tmp = children[i];
                        children[i] = children[j];
                        children[j] = tmp;
                    }
                }
            } else if (exeType == RoleAINodeExecuteType.Running) {
                if (node.activeChild != null) {
                    exeType = ExecuteNode(ctx, entity, node.activeChild, fixdt);
                    if (exeType == RoleAINodeExecuteType.Done) {
                        node.activeChild = null;
                    }
                } else {
                    int doneCount = 0;
                    foreach (RoleAINodeModel child in node.children) {
                        RoleAINodeExecuteType res = ExecuteNode(ctx, entity, child, fixdt);
                        if (res == RoleAINodeExecuteType.Done) {
                            doneCount++;
                            break;
                        } else if (res == RoleAINodeExecuteType.Running) {
                            node.activeChild = child;
                            break;
                        }
                    }
                    if (doneCount > 0) {
                        exeType = RoleAINodeExecuteType.Done;
                        node.activeChild = null;
                    }
                }
            } else {
                // Done, do nothing
            }
            return exeType;
        }

        // 同时执行所有子节点, 只要一个子节点Done, 就算容器Done
        static RoleAINodeExecuteType ExecuteParallelAnd(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            ref var exeType = ref node.executeType;
            if (exeType == RoleAINodeExecuteType.NotEntered) {
                if (!CheckPrecondition(ctx, entity, node.preconditionModel, fixdt)) {
                    // Failed precondition means done
                    exeType = RoleAINodeExecuteType.Done;
                } else {
                    exeType = RoleAINodeExecuteType.Running;
                }
            } else if (exeType == RoleAINodeExecuteType.Running) {
                int totalCount = node.children.Count;
                int doneCount = 0;
                foreach (RoleAINodeModel child in node.children) {
                    RoleAINodeExecuteType res = ExecuteNode(ctx, entity, child, fixdt);
                    if (res == RoleAINodeExecuteType.Done) {
                        doneCount++;
                    }
                }
                if (doneCount == totalCount) {
                    exeType = RoleAINodeExecuteType.Done;
                }
            } else {
                // Done, do nothing
            }
            return exeType;
        }

        // 同时执行所有子节点, 所有子节点Done, 就算容器Done
        static RoleAINodeExecuteType ExecuteParallelOr(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            ref var exeType = ref node.executeType;
            if (exeType == RoleAINodeExecuteType.NotEntered) {
                if (!CheckPrecondition(ctx, entity, node.preconditionModel, fixdt)) {
                    // Failed precondition means done
                    exeType = RoleAINodeExecuteType.Done;
                } else {
                    exeType = RoleAINodeExecuteType.Running;
                }
            } else if (exeType == RoleAINodeExecuteType.Running) {
                int doneCount = 0;
                foreach (RoleAINodeModel child in node.children) {
                    RoleAINodeExecuteType res = ExecuteNode(ctx, entity, child, fixdt);
                    if (res == RoleAINodeExecuteType.Done) {
                        doneCount++;
                        break;
                    }
                }
                if (doneCount > 0) {
                    exeType = RoleAINodeExecuteType.Done;
                }
            } else {
                // Done, do nothing
            }
            return exeType;
        }
        #endregion

        #region Precondition
        static bool CheckPrecondition(GameContext ctx, RoleEntity entity, RoleAIPreconditionModel precondition, float fixdt) {
            if (precondition == null) {
                return true;
            }
            // TODO: check precondition
            return true;
        }
        #endregion

        #region Action Enter/Update
        static void Action_Enter(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {

        }

        static RoleAINodeExecuteType Action_Update(GameContext ctx, RoleEntity entity, RoleAINodeModel node, float fixdt) {
            return RoleAINodeExecuteType.Done;
        }
        #endregion

    }

}