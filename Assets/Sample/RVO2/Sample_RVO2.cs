using System;
using System.Collections.Generic;
using UnityEngine;
using RVO;

namespace GameFunctions.Sample {

    public class Sample_RVO2 : MonoBehaviour {

        Simulator simulator;
        Dictionary<int, Agent> agents;

        void Awake() {
            simulator = new Simulator();
            agents = new Dictionary<int, Agent>();
        }

        void Update() {
            try {
                if (Input.GetMouseButtonDown(0)) {
                    Agent agent = SpawnAgent(Input.mousePosition);
                    agents.Add(agent.ID, agent);
                }
                foreach (Agent agent in agents.Values) {
                    agent.SetPreferVelocity(agent.targetPos - agent.Pos);
                }
                simulator.doStep(Time.deltaTime);
            } catch {
                Debug.LogError("RVO2 Simulator is not initialized.");
            }
        }

        Agent SpawnAgent(Vector2 screen) {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screen);
            float moveSpeed = 5f;
            var agent = simulator.AddAgent(worldPos, 55f, 4, 5, 5f, 2f, moveSpeed, Vector2.zero);
            agent.targetPos = -worldPos;
            return agent;
        }

        void OnDrawGizmos() {
            if (agents == null) return;
            foreach (Agent agent in agents.Values) {
                DrawAgent(agent);
            }
        }

        void DrawAgent(Agent agent) {
            Vector2 pos = agent.Pos;
            float radius = agent.Radius;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pos, radius);

            Vector2 velocity = agent.Velocity;
            Vector2 end = pos + velocity;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, end);
        }

    }

}