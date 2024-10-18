using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    public static class GFCamera2DHelper {

        /// <summary>
        /// Confine the camera in a rectangle area.
        /// <para>Return the new camera position.</para>
        /// <para>resolutionRadio: width / height </para>
        /// </summary>
        public static Vector2 CalcConfinePos(Vector2 cameraPos, Vector2 confinerMin, Vector2 confinerMax, float cameraOrthoSize, float resolutionRadio) {

            Vector2 confinerHalfSize = (confinerMax - confinerMin) / 2;
            Vector2 cameraHalfSize = new Vector2(cameraOrthoSize * resolutionRadio, cameraOrthoSize);

            Vector2 cameraMin = cameraPos - cameraHalfSize;
            Vector2 cameraMax = cameraPos + cameraHalfSize;

            Vector2 diff = Vector2.zero;
            if (cameraMin.x < confinerMin.x) {
                diff.x = confinerMin.x - cameraMin.x;
            } else if (cameraMax.x > confinerMax.x) {
                diff.x = confinerMax.x - cameraMax.x;
            }

            if (cameraMin.y < confinerMin.y) {
                diff.y = confinerMin.y - cameraMin.y;
            } else if (cameraMax.y > confinerMax.y) {
                diff.y = confinerMax.y - cameraMax.y;
            }

            return cameraPos + diff;

        }

    }
}
