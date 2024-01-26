using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_GFPhysicsIntersection2D : MonoBehaviour {

        [SerializeField] GameObject line1_start;
        [SerializeField] GameObject line1_end;
        [SerializeField] GameObject line2_start;
        [SerializeField] GameObject line2_end;

        int mode = 0;

        void Update() {

        }

        void OnDrawGizmos() {
            if (mode == 0) {
                LineLine();
            }
        }

        void LineLine() {
            Vector2 aStart = line1_start.transform.position;
            Vector2 aEnd = line1_end.transform.position;
            Vector2 bStart = line2_start.transform.position;
            Vector2 bEnd = line2_end.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(aStart, aEnd);
            Gizmos.DrawLine(bStart, bEnd);
            bool isIntersect = GFPhysicsIntersection2D.IsLineXLine(aStart, aEnd, bStart, bEnd, out Vector2 intersection);
            if (isIntersect) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(aStart, aEnd);
                Gizmos.DrawLine(bStart, bEnd);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(intersection, 0.1f);
            }
        }

    }

}