using System;
using UnityEngine;

namespace GameFunctions {

    // 刚体 36
    public class GFRB2DEntity {

        public int id;

        public bool isActive;

        public Vector2 pos; // 8
        public float rot; // 4 旋转角度
        public Vector2 velocity; // 8

        public float mass; // 4

        public GFShapeType shapeType; // 4
        public Vector2 shapeSize; // 8

    }

}