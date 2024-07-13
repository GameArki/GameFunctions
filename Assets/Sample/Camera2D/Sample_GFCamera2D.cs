using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_GFCamera2D : MonoBehaviour {

        [SerializeField] Camera cam;

        [SerializeField] Vector2 confinerMin;
        [SerializeField] Vector2 confinerMax;
        [SerializeField] float moveSpeed;

        void Awake() {

        }

        void Update() {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 cameraPos = cam.transform.position;
            cameraPos += input * Time.deltaTime * moveSpeed;
            cameraPos = GFCamera2DHelper.CalcConfinePos(cameraPos, confinerMin, confinerMax, cam.orthographicSize, cam.aspect);
            cam.transform.position = new Vector3(cameraPos.x, cameraPos.y, cam.transform.position.z);
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((confinerMin + confinerMax) / 2, confinerMax - confinerMin);
        }

    }

}