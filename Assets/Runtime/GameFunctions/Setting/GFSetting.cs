using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    public static class GFSetting {

        /// <summary>
        /// Intro: 该函数保证一屏不会裁剪掉任何东西, 但会留下黑边
        /// Input: 假设一屏最小值是 40 x 22.5 米, 传入 40, 22.5
        /// return: 正交相机大小 OrthographicSize
        /// </summary>
        public static float CalculateCameraOrthographicSize(float minWidth, float minHeight) {
            float curAspect = (float)Screen.width / Screen.height;
            float targetAspect = minWidth / minHeight;
            float targetOrthographicSize = minHeight * 0.5f;
            if (curAspect < targetAspect) {
                // 竖屏
                targetOrthographicSize = minWidth * 0.5f / curAspect;
            } else if (curAspect > targetAspect) {
                // 带鱼屏
            }
            return targetOrthographicSize;
        }

    }

}