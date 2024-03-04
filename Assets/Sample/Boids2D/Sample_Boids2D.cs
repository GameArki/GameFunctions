using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions.Sample {

    public class Sample_Boids2D : MonoBehaviour {

        Dictionary<int, GFBoidsEntity2D> all;
        GFBoidsManager manager;

        void Awake() {
            all = new Dictionary<int, GFBoidsEntity2D>();
            manager = new GFBoidsManager(new GFBoidsSettingModel() {
                maxBoids = 100,
                originWeight = 0.9f,
                separateRadius = 3f,
                separateFactor = .5f,
                alignRadius = 10f,
                alignFactor = .5f,
                cohesionRadius = 10f,
                cohesionFactor = .5f,
                leaderFactor = .5f
            });
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Vector2 velo = Vector2.right;
                int groupID = 0;
                if (Input.GetKey(KeyCode.LeftShift)) {
                    groupID = 1;
                }
                bool isLeader = false;
                if (Input.GetKey(KeyCode.LeftControl)) {
                    isLeader = true;
                }
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GFBoidsEntity2D boid = manager.Add(isLeader, groupID, worldPos, velo.normalized, 1f, 5f);
                if (boid != null) {
                    all.Add(boid.id, boid);
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                foreach (GFBoidsEntity2D boid in all.Values) {
                    if (boid.isLeader) {
                        boid.velocity = Random.insideUnitCircle * boid.moveSpeed;
                    }
                }
            }
            foreach (GFBoidsEntity2D boid in all.Values) {
                boid.velocity = Vector2.right * boid.moveSpeed;
            }
            manager.Simulate(Time.deltaTime);
        }

        Dictionary<int, Color> colors = new Dictionary<int, Color> {
            { 0, Color.red },
            { 1, Color.blue }
        };

        void OnDrawGizmos() {
            if (all == null) return;
            foreach (GFBoidsEntity2D boid in all.Values) {
                DrawBoid(boid);
            }
        }

        void DrawBoid(GFBoidsEntity2D boid) {

            bool hasColor = colors.TryGetValue(boid.groupID, out Color color);
            if (!hasColor) {
                color = Color.white;
            }

            Vector2 pos = boid.position;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(pos, 1f);

            // Draw velocity
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + boid.velocity);

        }

    }

}