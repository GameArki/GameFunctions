using UnityEngine;
using GameClasses;
using GameClasses.Camera2DLib;

namespace GameFunctions.Sample {

    public class Sample_Camera2D : MonoBehaviour {

        [SerializeField] Camera cam;
        Camera2DCore core;
        int handleID;

        GameObject role;

        [SerializeField] Vector2 confinerMin;
        [SerializeField] Vector2 confinerMax;
        [SerializeField] float moveSpeed;

        [SerializeField] Vector2 shakeAmplitude = new Vector2(0.1f, 0.1f);
        [SerializeField] float shakeDuration = 0.1f;
        [SerializeField] float shakeFrequency = 20f;

        void Awake() {

            role = GameObject.CreatePrimitive(PrimitiveType.Cube);
            role.transform.position = Vector3.zero;

            core = new Camera2DCore();
            float orthographicSize = cam.orthographicSize;
            float aspect = cam.aspect;
            handleID = core.Init(Vector2.zero, orthographicSize, aspect);

            core.Confine_Enable(handleID, true);
            core.Confine_Set(handleID, confinerMin, confinerMax);

            core.Follow_Enable(handleID, true);
            core.Follow_Set(handleID, role.transform.position, Vector2.zero, 1.5f, 2.5f);
        }

        void Update() {
            float dt = Time.deltaTime;

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 rolePos = role.transform.position;
            rolePos += input * dt * moveSpeed;
            role.transform.position = rolePos;

            core.Follow_Update(handleID, role.transform.position);

            if (Input.GetKeyDown(KeyCode.Space)) {
                core.Effect_Shake_Begin(handleID, shakeAmplitude, shakeFrequency, shakeDuration);
            }
            var res = core.Tick(dt);
            cam.transform.position = new Vector3(res.pos.x, res.pos.y, cam.transform.position.z);
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((confinerMin + confinerMax) / 2, confinerMax - confinerMin);
        }

    }

}