using System;
using System.Linq;
using UnityEngine;
using NUnit.Framework;

namespace GameFunctions.Tests {

    public class GFHexTests {

        [Test]
        public void Test_Distance() {
            // (x,y)
            // (列,行)
            {
                Vector2Int a = new Vector2Int(1, 1);
                Vector2Int b = new Vector2Int(0, 2);
                Assert.AreEqual(1, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(1, 1);
                Vector2Int b = new Vector2Int(1, 2);
                Assert.AreEqual(1, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(1, 1);
                Vector2Int b = new Vector2Int(1, 3);
                Assert.AreEqual(2, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(1, 1);
                Vector2Int b = new Vector2Int(0, 3);
                Assert.AreEqual(2, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(1, 1);
                Vector2Int b = new Vector2Int(2, 3);
                Assert.AreEqual(3, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(1, 5);
                Vector2Int b = new Vector2Int(2, 1);
                Assert.AreEqual(4, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(3, 5);
                Vector2Int b = new Vector2Int(2, 1);
                Assert.AreEqual(5, GFHex.Distance(a, b));
            }
            {
                Vector2Int a = new Vector2Int(0, 4);
                Vector2Int b = new Vector2Int(2, 1);
                Assert.AreEqual(3, GFHex.Distance(a, b));
            }
        }

    }

}