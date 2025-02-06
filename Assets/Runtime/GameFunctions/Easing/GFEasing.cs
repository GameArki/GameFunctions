using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    // Attentions:
    // 1. duration must be greater than 0
    // 2. duration must greater than passed time
    public static class GFEasing {

        [ThreadStatic] static Dictionary<GFEasingEnum, Func<float, float>> easingFuncs;

        static GFEasing() {
            easingFuncs = new Dictionary<GFEasingEnum, Func<float, float>>();
            easingFuncs.Add(GFEasingEnum.Immediate, (float timePercent) => EasingFunctionHelper.EaseImmediate(timePercent));
            easingFuncs.Add(GFEasingEnum.Linear, (float timePercent) => EasingFunctionHelper.EaseLinear(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainLinear, (float timePercent) => EasingFunctionHelper.EaseMountainLinear(timePercent));
            easingFuncs.Add(GFEasingEnum.InQuad, (float timePercent) => EasingFunctionHelper.EaseInQuad(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainInQuad, (float timePercent) => EasingFunctionHelper.EaseMountainInQuad(timePercent));
            easingFuncs.Add(GFEasingEnum.OutQuad, (float timePercent) => EasingFunctionHelper.EaseOutQuad(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainOutQuad, (float timePercent) => EasingFunctionHelper.EaseMountainOutQuad(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutQuad, (float timePercent) => EasingFunctionHelper.EaseInOutQuad(timePercent));
            easingFuncs.Add(GFEasingEnum.InCubic, (float timePercent) => EasingFunctionHelper.EaseInCubic(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainInCubic, (float timePercent) => EasingFunctionHelper.EaseMountainInCubic(timePercent));
            easingFuncs.Add(GFEasingEnum.OutCubic, (float timePercent) => EasingFunctionHelper.EaseOutCubic(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainOutCubic, (float timePercent) => EasingFunctionHelper.EaseMountainOutCubic(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutCubic, (float timePercent) => EasingFunctionHelper.EaseInOutCubic(timePercent));
            easingFuncs.Add(GFEasingEnum.InQuart, (float timePercent) => EasingFunctionHelper.EaseInQuart(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainInQuart, (float timePercent) => EasingFunctionHelper.EaseMountainInQuart(timePercent));
            easingFuncs.Add(GFEasingEnum.OutQuart, (float timePercent) => EasingFunctionHelper.EaseOutQuart(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainOutQuart, (float timePercent) => EasingFunctionHelper.EaseMountainOutQuart(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutQuart, (float timePercent) => EasingFunctionHelper.EaseInOutQuart(timePercent));
            easingFuncs.Add(GFEasingEnum.InQuint, (float timePercent) => EasingFunctionHelper.EaseInQuint(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainInQuint, (float timePercent) => EasingFunctionHelper.EaseMountainInQuint(timePercent));
            easingFuncs.Add(GFEasingEnum.OutQuint, (float timePercent) => EasingFunctionHelper.EaseOutQuint(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainOutQuint, (float timePercent) => EasingFunctionHelper.EaseMountainOutQuint(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutQuint, (float timePercent) => EasingFunctionHelper.EaseInOutQuint(timePercent));
            easingFuncs.Add(GFEasingEnum.InSine, (float timePercent) => EasingFunctionHelper.EaseInSine(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainInSine, (float timePercent) => EasingFunctionHelper.EaseMountainInSine(timePercent));
            easingFuncs.Add(GFEasingEnum.OutSine, (float timePercent) => EasingFunctionHelper.EaseOutSine(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainOutSine, (float timePercent) => EasingFunctionHelper.EaseMountainOutSine(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutSine, (float timePercent) => EasingFunctionHelper.EaseInOutSine(timePercent));
            easingFuncs.Add(GFEasingEnum.InExpo, (float timePercent) => EasingFunctionHelper.EaseInExpo(timePercent));
            easingFuncs.Add(GFEasingEnum.OutExpo, (float timePercent) => EasingFunctionHelper.EaseOutExpo(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutExpo, (float timePercent) => EasingFunctionHelper.EaseInOutExpo(timePercent));
            easingFuncs.Add(GFEasingEnum.InCirc, (float timePercent) => EasingFunctionHelper.EaseInCirc(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainInCirc, (float timePercent) => EasingFunctionHelper.EaseMountainInCirc(timePercent));
            easingFuncs.Add(GFEasingEnum.OutCirc, (float timePercent) => EasingFunctionHelper.EaseOutCirc(timePercent));
            easingFuncs.Add(GFEasingEnum.MountainOutCirc, (float timePercent) => EasingFunctionHelper.EaseMountainOutCirc(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutCirc, (float timePercent) => EasingFunctionHelper.EaseInOutCirc(timePercent));
            easingFuncs.Add(GFEasingEnum.InElastic, (float timePercent) => EasingFunctionHelper.EaseInElastic(timePercent));
            easingFuncs.Add(GFEasingEnum.OutElastic, (float timePercent) => EasingFunctionHelper.EaseOutElastic(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutElastic, (float timePercent) => EasingFunctionHelper.EaseInOutElastic(timePercent));
            easingFuncs.Add(GFEasingEnum.InBack, (float timePercent) => EasingFunctionHelper.EaseInBack(timePercent));
            easingFuncs.Add(GFEasingEnum.OutBack, (float timePercent) => EasingFunctionHelper.EaseOutBack(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutBack, (float timePercent) => EasingFunctionHelper.EaseInOutBack(timePercent));
            easingFuncs.Add(GFEasingEnum.InBounce, (float timePercent) => EasingFunctionHelper.EaseInBounce(timePercent));
            easingFuncs.Add(GFEasingEnum.OutBounce, (float timePercent) => EasingFunctionHelper.EaseOutBounce(timePercent));
            easingFuncs.Add(GFEasingEnum.InOutBounce, (float timePercent) => EasingFunctionHelper.EaseInOutBounce(timePercent));
        }

        public static void SetupBackValue(float backValue) {
            EasingFunctionHelper.SetupBackValue(backValue);
        }

        public static float Ease1D(GFEasingEnum type, float passTime, float duration, float startValue, float endValue) {
            float timePercent = passTime / duration;
            if (timePercent > 1) {
                timePercent = 1;
            }
            float valuePercent = GetValuePercent(type, timePercent);
            return startValue + (endValue - startValue) * valuePercent;
        }

        public static Vector2 Ease2D(GFEasingEnum type, float passTime, float duration, Vector2 startValue, Vector2 endValue) {
            float timePercent = passTime / duration;
            if (timePercent > 1) {
                timePercent = 1;
            }
            float valuePercent = GetValuePercent(type, timePercent);
            return startValue + (endValue - startValue) * valuePercent;
        }

        public static Vector3 Ease3D(GFEasingEnum type, float passTime, float duration, Vector3 startValue, Vector3 endValue) {
            float timePercent = passTime / duration;
            if (timePercent > 1) {
                timePercent = 1;
            }
            float valuePercent = GetValuePercent(type, timePercent);
            return startValue + (endValue - startValue) * valuePercent;
        }

        public static Color EaseColor(GFEasingEnum type, float passTime, float duration, Color startValue, Color endValue) {
            float timePercent = passTime / duration;
            if (timePercent > 1) {
                timePercent = 1;
            }
            float valuePercent = GetValuePercent(type, timePercent);
            return Color.Lerp(startValue, endValue, valuePercent);
        }

        /// <summary>
        /// 3D 球型环绕移动函数.
        /// <para>解释: 以中心点为球心, 球面上的起始点, 球面上的终点, (起始点-中心点).magnitude 为半径, 环绕移动.</para>
        /// <returns>返回当前时间在球面上的位置</returns>
        /// <para name="type">type: 缓动类型</para>
        /// <para>passTime: 经过的时间</para>
        /// <para>duration: 总时间</para>
        /// <para>centerValue: 绕柱的中心点</para>
        /// <para>startValue: 起始点</para>
        /// <para>endValue: 终点</para>
        /// </summary>
        public static Vector3 Ease3DSphereRound(GFEasingEnum type, float passTime, float duration, Vector3 center, Vector3 startValue, Vector3 endValue, bool isFallbackClockwise = false) {
            float timePercent = passTime / duration;
            if (timePercent > 1) {
                timePercent = 1;
            }
            float valuePercent = GetValuePercent(type, timePercent);
            Vector3 endDiff = (endValue - center);
            Vector3 startDiff = (startValue - center);
            float radius = (startValue - center).magnitude;
            // 1. 计算起始点和终点的夹角 angle
            // 2. angle = valuePercent * angle
            // 3. 以 center 为中心, radius 为半径, angle 为角度, 计算出 pos
            float angle = Vector3.Angle(startDiff, endDiff);
            if (isFallbackClockwise) {
                angle = -angle;
            }
            Vector3 pos = Quaternion.AngleAxis(angle * valuePercent, Vector3.Cross(startDiff, endDiff)) * startDiff;
            pos = pos.normalized * radius;

            return center + pos;
        }

        public static Quaternion EaseQuaternion(GFEasingEnum type, float passTime, float duration, Quaternion startValue, Quaternion endValue) {
            float timePercent = passTime / duration;
            if (timePercent > 1) {
                timePercent = 1;
            }
            float valuePercent = GetValuePercent(type, timePercent);
            return Quaternion.Lerp(startValue, endValue, valuePercent);
        }

        public static float GetValuePercent(GFEasingEnum type, float timePercent) {
            easingFuncs.TryGetValue(type, out Func<float, float> func);
            if (func != null) {
                return func.Invoke(timePercent);
            } else {
                throw new System.ArgumentException("Invalid GFEasingEnum" + type.ToString());
            }
        }

    }

}