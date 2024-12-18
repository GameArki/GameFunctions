using System;
using UnityEngine;
using UnityEngine.Rendering;
using GameRenderer;

namespace GameFunctions.Sample {

    public class Sample_GameRenderer : MonoBehaviour {

        [SerializeField] Volume volume_global;

        [SerializeField] float shakeScreenFrequency = 50f;
        [SerializeField] float shakeScreenDuration = 1f;
        [SerializeField] Vector2 shakeScreenAmplitude = new Vector2(0.1f, 0.1f);

        void Update() {
            float dt = Time.deltaTime;

            if (volume_global.profile.TryGet<PPShakeScreenVolume>(out var shakeScreenVolume)) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    shakeScreenVolume.isEnable.value = true;
                    shakeScreenVolume.frequency.value = shakeScreenFrequency;
                    shakeScreenVolume.duration.value = shakeScreenDuration;
                    shakeScreenVolume.timer.value = shakeScreenDuration;
                    shakeScreenVolume.amplitude.value = shakeScreenAmplitude;
                }
                shakeScreenVolume.timer.value -= dt;
                if (shakeScreenVolume.timer.value <= 0f) {
                    shakeScreenVolume.isEnable.value = false;
                }
            }
        }

    }

}