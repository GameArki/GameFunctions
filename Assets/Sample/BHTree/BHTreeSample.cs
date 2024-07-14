using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClasses.BehaviourTree;

namespace GameClasses.Sample {

    public class BHTreeSample : MonoBehaviour {

        BHTree tree;

        // Temp
        bool hasEnemy = false;
        float searchTime = 0;

        void Start() {

            // ==== Build Tree ====
            tree = new BHTree();

            BHTreeNode root = new BHTreeNode();
            root.InitAsContainer(BHTreeContainerType.Sequence, null);

            BHTreeNode stayContainer;
            {
                stayContainer = new BHTreeNode();
                stayContainer.InitAsContainer(BHTreeContainerType.ParallelOr, null);

                BHTreeNode stayAction = new BHTreeNode();
                stayAction.InitAsAction(() => {
                    return !hasEnemy;
                }, (dt) => {
                    Debug.Log("StayAction Enter");
                }, (dt) => {
                    return BHTreeNodeExecuteType.Running;
                });

                BHTreeNode searchAction = new BHTreeNode();
                searchAction.InitAsAction(() => {
                    return !hasEnemy;
                }, (dt) => {
                    Debug.Log("SearchAction Enter");
                }, (dt) => {
                    searchTime += dt;
                    if (searchTime > 5) {
                        searchTime = 0;
                        hasEnemy = true;
                        Debug.Log("SearchAction Done");
                        return BHTreeNodeExecuteType.Done;
                    }
                    return BHTreeNodeExecuteType.Running;
                });

                stayContainer.AddChild(stayAction);
                stayContainer.AddChild(searchAction);

                root.AddChild(stayContainer);

            }

            BHTreeNode attackContainer;
            {
                attackContainer = new BHTreeNode();
                attackContainer.InitAsContainer(BHTreeContainerType.ParallelAnd, null);

                BHTreeNode attackAction = new BHTreeNode();
                attackAction.InitAsAction(() => {
                    return hasEnemy;
                }, (dt) => {
                    Debug.Log("AttackAction Enter");
                }, (dt) => {
                    Debug.Log("AttackAction Done");
                    return BHTreeNodeExecuteType.Done;
                });

                BHTreeNode escapeAction = new BHTreeNode();
                escapeAction.InitAsAction(() => {
                    return hasEnemy;
                }, (dt) => {
                    Debug.Log("EscapeAction Enter");
                }, (dt) => {
                    searchTime += dt;
                    if (searchTime > 5) {
                        searchTime = 0;
                        hasEnemy = false;
                        Debug.Log("EscapeAction Done");
                        return BHTreeNodeExecuteType.Done;
                    }
                    return BHTreeNodeExecuteType.Running;
                });

                attackContainer.AddChild(attackAction);
                attackContainer.AddChild(escapeAction);

                root.AddChild(attackContainer);

            }

            tree.InitRoot(root);

        }

        // Update is called once per frame
        void Update() {
            float dt = Time.deltaTime;
            tree.Execute(dt);
        }
    }

}
