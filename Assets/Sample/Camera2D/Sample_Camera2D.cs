using UnityEngine;
using GameClasses;
using GameClasses.Camera2DLib;

namespace GameFunctions.Sample {

    public class Sample_Camera2D : MonoBehaviour {

        [SerializeField] Camera cam;
        Camera2DCore core;
        int handleID;

        GameObject role;

        [Header("Confiner")]
        [SerializeField] Vector2 confinerMin;
        [SerializeField] Vector2 confinerMax;
        [SerializeField] float moveSpeed;

        [Header("Effect: Shake")]
        [SerializeField] Vector2 shakeAmplitude = new Vector2(0.1f, 0.1f);
        [SerializeField] float shakeDuration = 0.1f;
        [SerializeField] float shakeFrequency = 20f;

        [Header("Effect: ZoomIn")]
        [SerializeField] float zoomInDuration = 1f;
        [SerializeField] float zoomInMultiply = 2f;
        [SerializeField] GFEasingEnum zoomInEasingType = GFEasingEnum.Linear;
        [SerializeField] GFEasingEnum zoomInRestoreEasingType = GFEasingEnum.Linear;
        [SerializeField] float zoomInRestoreDelaySec = 1f;
        [SerializeField] float zoomInRestoreDuration = 1f;

        [Header("DeadZone")]
        [SerializeField] Vector2 followDamping = new Vector2(1.5f, 2.5f);
        [SerializeField] Vector2 deadZoneSize = new Vector2(5f, 5f);

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
            core.Follow_Set(handleID, role.transform.position, Vector2.zero, followDamping.x, followDamping.y);
        }

        void Update() {

            core.Follow_DeadZone_Set(handleID, deadZoneSize);

            float dt = Time.deltaTime;

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 rolePos = role.transform.position;
            rolePos += input * dt * moveSpeed;
            role.transform.position = rolePos;

            core.Follow_Update(handleID, role.transform.position);

            if (Input.GetKeyDown(KeyCode.Space)) {
                core.Effect_Shake_Begin(handleID, shakeAmplitude, shakeFrequency, shakeDuration);
            } else if (Input.GetKeyDown(KeyCode.Z)) {
                // core.Effect_ZoomIn_Begin(handleID, zoomInEasingType, zoomInMultiply, zoomInDuration);
                core.Effect_ZoomIn_BeginAndAutoRestore(handleID, zoomInEasingType, zoomInMultiply, zoomInDuration, zoomInRestoreEasingType, zoomInRestoreDuration, zoomInRestoreDelaySec);
            }
            var res = core.Tick(dt);
            cam.orthographicSize = res.orthographicSize;
            cam.transform.position = new Vector3(res.pos.x, res.pos.y, cam.transform.position.z);
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((confinerMin + confinerMax) / 2, confinerMax - confinerMin);

            if (core == null) return;
            core.Gizmos_Draw(handleID);
        }

    }

}