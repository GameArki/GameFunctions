using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_GFPhysicsIntersection2D : MonoBehaviour {

        // ==== LINE X LINE ====
        [Header("Line X Line")]
        [SerializeField] GameObject line1_start;
        [SerializeField] GameObject line1_end;
        [SerializeField] GameObject line2_start;
        [SerializeField] GameObject line2_end;

        // ==== CIRCLE X CIRCLE ====
        [Header("Circle X Circle")]
        [SerializeField] GameObject circle1_center;
        [SerializeField] float circle1_radius;
        [SerializeField] GameObject circle2_center;
        [SerializeField] float circle2_radius;

        int mode = 0;

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                mode++;
                if (mode > 1) {
                    mode = 0;
                }
            }
        }

        void OnDrawGizmos() {
            const int MODE_LINE_LINE = 0;
            const int MODE_CIRCLE_CIRCLE = 1;
            if (mode == MODE_LINE_LINE) {
                LineLine();
            } else if (mode == MODE_CIRCLE_CIRCLE) {
                CircleCircle();
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

        void CircleCircle() {
            Vector2 aCenter = circle1_center.transform.position;
            Vector2 bCenter = circle2_center.transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(aCenter, circle1_radius);
            Gizmos.DrawWireSphere(bCenter, circle2_radius);
            int intersectMode = GFPhysicsIntersection2D.IsCircleXCircleOutPoints(aCenter, circle1_radius, bCenter, circle2_radius, out Vector2 intersection1, out Vector2 intersection2);
            if (intersectMode != -1) {
                if (intersectMode == 3) {
                    Gizmos.color = Color.blue;
                } else {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(aCenter, circle1_radius);
                Gizmos.DrawWireSphere(bCenter, circle2_radius);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(intersection1, 0.1f);
                Gizmos.DrawSphere(intersection2, 0.1f);
            }
        }

    }

}