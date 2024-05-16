using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    [ExecuteInEditMode]
    public class Sample_Camera3D : MonoBehaviour {

        [SerializeField] GameObject role;
        [SerializeField] Camera cam;

        [SerializeField] Vector3 followOffset;
        [SerializeField] Vector2 rotateDegreeOffset;

        void Start() {

        }

        void Update() {
            
            Vector3 camForward = cam.transform.forward;
            Vector3 moveAxis = GetMoveAxis(camForward);
            Role_Move(moveAxis);

            ApplyCamera();

        }

        void ApplyCamera() {
            Vector3 cameraForward = cam.transform.forward;
            Vector3 forward = GFCamera3D.GetRotateForward(Vector3.forward, rotateDegreeOffset);
            cam.transform.forward = forward;

            Vector3 cameraPos = GFCamera3D.GetFollowPos(cam.transform.forward, role.transform.position, followOffset);
            cam.transform.position = cameraPos;
        }

        Vector3 GetMoveAxis(Vector3 cameraForward) {
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            cameraForward.y = 0;
            move = cameraForward * move.z + cam.transform.right * move.x;
            move.Normalize();
            return move;
        }

        void Role_Move(Vector3 moveAxis) {
            if (moveAxis == Vector3.zero) return;
            role.transform.position += moveAxis * Time.deltaTime;
            role.transform.forward = moveAxis;
        }

    }

}
