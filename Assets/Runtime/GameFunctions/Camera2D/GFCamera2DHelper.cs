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
            
            Vector2 target = cameraPos;

            Vector2 confinerHalfSize = (confinerMax - confinerMin) / 2;
            Vector2 cameraHalfSize = new Vector2(cameraOrthoSize * resolutionRadio, cameraOrthoSize);
            cameraHalfSize = Vector2.Min(cameraHalfSize, confinerHalfSize);

            target.x = Mathf.Clamp(target.x, confinerMin.x + cameraHalfSize.x, confinerMax.x - cameraHalfSize.x);
            target.y = Mathf.Clamp(target.y, confinerMin.y + cameraHalfSize.y, confinerMax.y - cameraHalfSize.y);

            return target;

        }

    }
}
