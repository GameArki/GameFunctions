using System.Collections.Generic;
using UnityEngine;

namespace GameFunctions {

    public class GFBoidsManager {

        GFBoidsSettingModel settingModel;

        int idRecord;
        GFBoidsEntity2D[] all;
        int count;

        Dictionary<int, GFBoidsEntity2D> leaderDict;

        public GFBoidsManager(GFBoidsSettingModel settingModel) {
            this.settingModel = settingModel;
            all = new GFBoidsEntity2D[settingModel.maxBoids];
            leaderDict = new Dictionary<int, GFBoidsEntity2D>(10);
            count = 0;
            idRecord = 0;
        }

        public GFBoidsEntity2D Add(bool isLeader, int groupID, Vector2 position, Vector2 velocity, float radius, float moveSpeed) {
            bool hasLeader = leaderDict.TryGetValue(groupID, out GFBoidsEntity2D leader);
            if (isLeader && hasLeader) {
                Debug.LogError("Leader already exists in group " + groupID);
                return null;
            }
            GFBoidsEntity2D boid = new GFBoidsEntity2D();
            boid.id = idRecord++;
            boid.isLeader = isLeader;
            boid.groupID = groupID;
            boid.position = position;
            boid.velocity = velocity * moveSpeed;
            boid.radius = radius;
            boid.moveSpeed = moveSpeed;
            all[count] = boid;
            count++;
            if (isLeader) {
                leaderDict.Add(boid.groupID, boid);
            }
            return boid;
        }

        public void Remove(int id) {
            for (int i = 0; i < count; i++) {
                var boid = all[i];
                if (boid.id == id) {
                    all[i] = all[count - 1];
                    count--;
                    if (boid.isLeader) {
                        leaderDict.Remove(boid.groupID);
                    }
                    break;
                }
            }
        }

        public void Simulate(float dt) {
            for (int i = 0; i < count; i++) {
                GFBoidsEntity2D cur = all[i];
                if (!cur.isLeader) {
                    Vector2 separate = GFBoidsAlgorithm2D.Separation(cur, all, count, settingModel.separateRadius, settingModel.separateFactor);
                    Vector2 align = GFBoidsAlgorithm2D.Alignment(cur, all, count, settingModel.alignRadius, settingModel.alignFactor);
                    Vector2 cohesion = GFBoidsAlgorithm2D.Cohesion(cur, all, count, settingModel.cohesionRadius, settingModel.cohesionFactor);
                    cur.velocity = cur.velocity + (separate + align + cohesion) * (1 - settingModel.originVelocityWeight);
                    bool hasLeader = leaderDict.TryGetValue(cur.groupID, out GFBoidsEntity2D leader);
                    if (hasLeader) {
                        cur.velocity += (leader.position - cur.position).normalized;
                        cur.isFollowingLeader = true;
                    } else {
                        cur.isFollowingLeader = false;
                    }
                    cur.velocity = Vector2.ClampMagnitude(cur.velocity, cur.moveSpeed * 1.2f);
                }
                cur.position += cur.velocity * dt;
            }
        }

    }

}