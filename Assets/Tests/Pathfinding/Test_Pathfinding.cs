using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace GameFunctions.Tests {

    public class Test_Pathfinding {

        [Test]
        public void Test_8Dir() {

            var blocks = new HashSet<Vector2Int>();
            blocks.Add(new Vector2Int(4, 11));
            blocks.Add(new Vector2Int(5, 12));
            blocks.Add(new Vector2Int(6, 13));
            blocks.Add(new Vector2Int(7, 13));
            blocks.Add(new Vector2Int(8, 12));
            blocks.Add(new Vector2Int(9, 11));
            blocks.Add(new Vector2Int(10, 10));
            blocks.Add(new Vector2Int(11, 11));
            blocks.Add(new Vector2Int(12, 12));
            blocks.Add(new Vector2Int(13, 11));
            blocks.Add(new Vector2Int(14, 12));
            blocks.Add(new Vector2Int(15, 11));
            blocks.Add(new Vector2Int(16, 12));
            blocks.Add(new Vector2Int(17, 11));
            blocks.Add(new Vector2Int(18, 12));
            blocks.Add(new Vector2Int(18, 13));
            blocks.Add(new Vector2Int(18, 14));
            blocks.Add(new Vector2Int(19, 15));

            Vector2Int start = new Vector2Int(14, 13);
            Vector2Int end = new Vector2Int(14, 9);

            Vector2Int[] result = new Vector2Int[500];

            int count = GFPathfinding2D.AStar(true, start, end, 500, (pos) => {
                bool isWalkable = !blocks.Contains(pos);
                if (!isWalkable) {
                    Debug.Log("Block: " + pos);
                }
                return isWalkable;
            }, result, false);

            Debug.Log("Count: " + count + " visited: " + GFPathfinding2D.openSet.Count);
            for (int i = 0; i < count; i++) {
                Debug.Log(result[i]);
            }

        }

    }

}
