using UnityEngine;

namespace GameFunctions {

    public static class GFBoidsAlgorithm2D {

        public static Vector2 Separation(GFBoidsEntity2D cur, GFBoidsEntity2D[] all, int len, float separateRadius, float separateFactor) {
            Vector2 saparateVelocity = Vector2.zero;
            int count = 0;
            for (int i = 0; i < len; i++) {
                var other = all[i];
                if (cur.id == other.id) {
                    continue;
                }
                if (cur.groupID != other.groupID) {
                    continue;
                }
                float d = Vector2.Distance(cur.position, other.position);
                if (d > 0 && d < separateRadius) {
                    Vector2 diff = cur.position - other.position;
                    diff.Normalize();
                    diff /= d;
                    saparateVelocity += diff;
                    count++;
                }
            }
            if (count > 0) {
                saparateVelocity /= count;
            }
            if (saparateVelocity.magnitude > 0) {
                saparateVelocity.Normalize();
                saparateVelocity *= separateFactor;
            }
            return saparateVelocity;
        }

        public static Vector2 Alignment(GFBoidsEntity2D cur, GFBoidsEntity2D[] all, int len, float alignRadius, float alignFactor) {
            Vector2 alignVelocity = Vector2.zero;
            int count = 0;
            for (int i = 0; i < len; i++) {
                var other = all[i];
                if (cur.id == other.id) {
                    continue;
                }
                if (cur.groupID != other.groupID) {
                    continue;
                }
                float d = Vector2.Distance(cur.position, other.position);
                if (d > 0 && d < alignRadius) {
                    alignVelocity += other.velocity.normalized;
                    count++;
                }
            }
            if (count > 0) {
                alignVelocity /= count;
                alignVelocity *= alignFactor;
            }
            return alignVelocity;
        }

        public static Vector2 Cohesion(GFBoidsEntity2D cur, GFBoidsEntity2D[] all, int len, float cohesionRadius, float cohesionFactor) {
            Vector2 cohesionVelocity = Vector2.zero;
            int count = 0;
            for (int i = 0; i < len; i++) {
                var other = all[i];
                if (cur.id == other.id) {
                    continue;
                }
                if (cur.groupID != other.groupID) {
                    continue;
                }
                float d = Vector2.Distance(cur.position, other.position);
                if (d > 0 && d < cohesionRadius) {
                    cohesionVelocity += other.position;
                    count++;
                }
            }
            if (count > 0) {
                cohesionVelocity /= count;
                cohesionVelocity -= cur.position;
                cohesionVelocity.Normalize();
                cohesionVelocity *= cohesionFactor;
            }
            return cohesionVelocity;
        }

    }

}