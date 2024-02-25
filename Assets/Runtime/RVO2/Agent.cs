/*
 * Agent.cs
 * RVO2 Library C#
 *
 * SPDX-FileCopyrightText: 2008 University of North Carolina at Chapel Hill
 * SPDX-License-Identifier: Apache-2.0
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Please send all bug reports to <geom@cs.unc.edu>.
 *
 * The authors may be contacted via:
 *
 * Jur van den Berg, Stephen J. Guy, Jamie Snape, Ming C. Lin, Dinesh Manocha
 * Dept. of Computer Science
 * 201 S. Columbia St.
 * Frederick P. Brooks, Jr. Computer Science Bldg.
 * Chapel Hill, N.C. 27599-3175
 * United States of America
 *
 * <http://gamma.cs.unc.edu/RVO2/>
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RVO {
    /**
     * <summary>Defines an agent in the simulation.</summary>
     */
    public class Agent {

        internal IList<KeyValuePair<float, Agent>> agentNeighbors_;
        internal IList<Line> orcaLines_;
        internal Vector2 position_;
        public Vector2 targetPos;
        public Vector2 Pos => position_;
        internal Vector2 prefVelocity_;
        internal Vector2 velocity_;
        public Vector2 Velocity => velocity_;
        internal int id_ = 0;
        public int ID => id_;
        internal int maxNeighbors_ = 0;
        internal float maxSpeed_ = 0.0f;
        internal float neighborDist_ = 0.0f;
        internal float radius_ = 0.0f;
        public float Radius => radius_;
        internal float timeHorizon_ = 0.0f;
        internal float timeHorizonObst_ = 0.0f;

        Vector2 newVelocity_;

        public Agent() {
            agentNeighbors_ = new List<KeyValuePair<float, Agent>>();
            orcaLines_ = new List<Line>();
        }

        public void SetPos(Vector2 pos) {
            position_ = pos;
        }

        public void SetPreferVelocity(Vector2 prefVelocity) {
            prefVelocity_ = prefVelocity;
        }

        /**
         * <summary>Computes the neighbors of this agent.</summary>
         */
        internal void computeNeighbors(KdTree kdTree) {
            float rangeSq = RVOMath.sqr(timeHorizonObst_ * maxSpeed_ + radius_);

            agentNeighbors_.Clear();

            if (maxNeighbors_ > 0) {
                rangeSq = RVOMath.sqr(neighborDist_);
                kdTree.computeAgentNeighbors(this, ref rangeSq);
            }
        }

        /**
         * <summary>Computes the new velocity of this agent.</summary>
         */
        internal void computeNewVelocity(float dt) {
            orcaLines_.Clear();

            float invTimeHorizonObst = 1.0f / timeHorizonObst_;

            int numObstLines = orcaLines_.Count;

            float invTimeHorizon = 1.0f / timeHorizon_;

            /* Create agent ORCA lines. */
            for (int i = 0; i < agentNeighbors_.Count; ++i) {
                Agent other = agentNeighbors_[i].Value;

                Vector2 relativePosition = other.position_ - position_;
                Vector2 relativeVelocity = velocity_ - other.velocity_;
                float distSq = RVOMath.absSq(relativePosition);
                float combinedRadius = radius_ + other.radius_;
                float combinedRadiusSq = RVOMath.sqr(combinedRadius);

                Line line;
                Vector2 u;

                if (distSq > combinedRadiusSq) {
                    /* No collision. */
                    Vector2 w = relativeVelocity - invTimeHorizon * relativePosition;

                    /* Vector from cutoff center to relative velocity. */
                    float wLengthSq = RVOMath.absSq(w);
                    float dotProduct1 = Vector2.Dot(w, relativePosition);

                    if (dotProduct1 < 0.0f && RVOMath.sqr(dotProduct1) > combinedRadiusSq * wLengthSq) {
                        /* Project on cut-off circle. */
                        float wLength = RVOMath.sqrt(wLengthSq);
                        Vector2 unitW = w / wLength;

                        line.direction = new Vector2(unitW.y, -unitW.x);
                        u = (combinedRadius * invTimeHorizon - wLength) * unitW;
                    } else {
                        /* Project on legs. */
                        float leg = RVOMath.sqrt(distSq - combinedRadiusSq);

                        if (RVOMath.det(relativePosition, w) > 0.0f) {
                            /* Project on left leg. */
                            line.direction = new Vector2(relativePosition.x * leg - relativePosition.y * combinedRadius, relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
                        } else {
                            /* Project on right leg. */
                            line.direction = -new Vector2(relativePosition.x * leg + relativePosition.y * combinedRadius, -relativePosition.x * combinedRadius + relativePosition.y * leg) / distSq;
                        }

                        float dotProduct2 = Vector2.Dot(relativeVelocity, line.direction);
                        u = dotProduct2 * line.direction - relativeVelocity;
                    }
                } else {
                    /* Collision. Project on cut-off circle of time timeStep. */
                    float invTimeStep = 1.0f / dt;

                    /* Vector from cutoff center to relative velocity. */
                    Vector2 w = relativeVelocity - invTimeStep * relativePosition;

                    float wLength = RVOMath.abs(w);
                    Vector2 unitW = w / wLength;

                    line.direction = new Vector2(unitW.y, -unitW.x);
                    u = (combinedRadius * invTimeStep - wLength) * unitW;
                }

                line.point = velocity_ + 0.5f * u;
                orcaLines_.Add(line);
            }

            int lineFail = linearProgram2(orcaLines_, maxSpeed_, prefVelocity_, false, ref newVelocity_);

            if (lineFail < orcaLines_.Count) {
                linearProgram3(orcaLines_, numObstLines, lineFail, maxSpeed_, ref newVelocity_);
            }
        }

        /**
         * <summary>Inserts an agent neighbor into the set of neighbors of this
         * agent.</summary>
         *
         * <param name="agent">A pointer to the agent to be inserted.</param>
         * <param name="rangeSq">The squared range around this agent.</param>
         */
        internal void insertAgentNeighbor(Agent agent, ref float rangeSq) {
            if (this != agent) {
                float distSq = RVOMath.absSq(position_ - agent.position_);

                if (distSq < rangeSq) {
                    if (agentNeighbors_.Count < maxNeighbors_) {
                        agentNeighbors_.Add(new KeyValuePair<float, Agent>(distSq, agent));
                    }

                    int i = agentNeighbors_.Count - 1;

                    while (i != 0 && distSq < agentNeighbors_[i - 1].Key) {
                        agentNeighbors_[i] = agentNeighbors_[i - 1];
                        --i;
                    }

                    agentNeighbors_[i] = new KeyValuePair<float, Agent>(distSq, agent);

                    if (agentNeighbors_.Count == maxNeighbors_) {
                        rangeSq = agentNeighbors_[agentNeighbors_.Count - 1].Key;
                    }
                }
            }
        }

        /**
         * <summary>Updates the two-dimensional position and two-dimensional
         * velocity of this agent.</summary>
         */
        internal void update(float dt) {
            velocity_ = newVelocity_;
            position_ += velocity_ * dt;
        }

        /**
         * <summary>Solves a one-dimensional linear program on a specified line
         * subject to linear constraints defined by lines and a circular
         * constraint.</summary>
         *
         * <returns>True if successful.</returns>
         *
         * <param name="lines">Lines defining the linear constraints.</param>
         * <param name="lineNo">The specified line constraint.</param>
         * <param name="radius">The radius of the circular constraint.</param>
         * <param name="optVelocity">The optimization velocity.</param>
         * <param name="directionOpt">True if the direction should be optimized.
         * </param>
         * <param name="result">A reference to the result of the linear program.
         * </param>
         */
        bool linearProgram1(IList<Line> lines, int lineNo, float radius, Vector2 optVelocity, bool directionOpt, ref Vector2 result) {
            float dotProduct = Vector2.Dot(lines[lineNo].point, lines[lineNo].direction);
            float discriminant = RVOMath.sqr(dotProduct) + RVOMath.sqr(radius) - RVOMath.absSq(lines[lineNo].point);

            if (discriminant < 0.0f) {
                /* Max speed circle fully invalidates line lineNo. */
                return false;
            }

            float sqrtDiscriminant = RVOMath.sqrt(discriminant);
            float tLeft = -dotProduct - sqrtDiscriminant;
            float tRight = -dotProduct + sqrtDiscriminant;

            for (int i = 0; i < lineNo; ++i) {
                float denominator = RVOMath.det(lines[lineNo].direction, lines[i].direction);
                float numerator = RVOMath.det(lines[i].direction, lines[lineNo].point - lines[i].point);

                if (RVOMath.fabs(denominator) <= RVOMath.RVO_EPSILON) {
                    /* Lines lineNo and i are (almost) parallel. */
                    if (numerator < 0.0f) {
                        return false;
                    }

                    continue;
                }

                float t = numerator / denominator;

                if (denominator >= 0.0f) {
                    /* Line i bounds line lineNo on the right. */
                    tRight = Math.Min(tRight, t);
                } else {
                    /* Line i bounds line lineNo on the left. */
                    tLeft = Math.Max(tLeft, t);
                }

                if (tLeft > tRight) {
                    return false;
                }
            }

            if (directionOpt) {
                /* Optimize direction. */
                if (Vector2.Dot(optVelocity, lines[lineNo].direction) > 0.0f) {
                    /* Take right extreme. */
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                } else {
                    /* Take left extreme. */
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                }
            } else {
                /* Optimize closest point. */
                float t = Vector2.Dot(lines[lineNo].direction, (optVelocity - lines[lineNo].point));

                if (t < tLeft) {
                    result = lines[lineNo].point + tLeft * lines[lineNo].direction;
                } else if (t > tRight) {
                    result = lines[lineNo].point + tRight * lines[lineNo].direction;
                } else {
                    result = lines[lineNo].point + t * lines[lineNo].direction;
                }
            }

            return true;
        }

        /**
         * <summary>Solves a two-dimensional linear program subject to linear
         * constraints defined by lines and a circular constraint.</summary>
         *
         * <returns>The number of the line it fails on, and the number of lines
         * if successful.</returns>
         *
         * <param name="lines">Lines defining the linear constraints.</param>
         * <param name="radius">The radius of the circular constraint.</param>
         * <param name="optVelocity">The optimization velocity.</param>
         * <param name="directionOpt">True if the direction should be optimized.
         * </param>
         * <param name="result">A reference to the result of the linear program.
         * </param>
         */
        int linearProgram2(IList<Line> lines, float radius, Vector2 optVelocity, bool directionOpt, ref Vector2 result) {
            if (directionOpt) {
                /*
                 * Optimize direction. Note that the optimization velocity is of
                 * unit length in this case.
                 */
                result = optVelocity * radius;
            } else if (RVOMath.absSq(optVelocity) > RVOMath.sqr(radius)) {
                /* Optimize closest point and outside circle. */
                result = RVOMath.normalize(optVelocity) * radius;
            } else {
                /* Optimize closest point and inside circle. */
                result = optVelocity;
            }

            for (int i = 0; i < lines.Count; ++i) {
                if (RVOMath.det(lines[i].direction, lines[i].point - result) > 0.0f) {
                    /* Result does not satisfy constraint i. Compute new optimal result. */
                    Vector2 tempResult = result;
                    if (!linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result)) {
                        result = tempResult;

                        return i;
                    }
                }
            }

            return lines.Count;
        }

        /**
         * <summary>Solves a two-dimensional linear program subject to linear
         * constraints defined by lines and a circular constraint.</summary>
         *
         * <param name="lines">Lines defining the linear constraints.</param>
         * <param name="numObstLines">Count of obstacle lines.</param>
         * <param name="beginLine">The line on which the 2-d linear program
         * failed.</param>
         * <param name="radius">The radius of the circular constraint.</param>
         * <param name="result">A reference to the result of the linear program.
         * </param>
         */
        void linearProgram3(IList<Line> lines, int numObstLines, int beginLine, float radius, ref Vector2 result) {
            float distance = 0.0f;

            for (int i = beginLine; i < lines.Count; ++i) {
                if (RVOMath.det(lines[i].direction, lines[i].point - result) > distance) {
                    /* Result does not satisfy constraint of line i. */
                    IList<Line> projLines = new List<Line>();
                    for (int ii = 0; ii < numObstLines; ++ii) {
                        projLines.Add(lines[ii]);
                    }

                    for (int j = numObstLines; j < i; ++j) {
                        Line line;

                        float determinant = RVOMath.det(lines[i].direction, lines[j].direction);

                        if (RVOMath.fabs(determinant) <= RVOMath.RVO_EPSILON) {
                            /* Line i and line j are parallel. */
                            if (Vector2.Dot(lines[i].direction, lines[j].direction) > 0.0f) {
                                /* Line i and line j point in the same direction. */
                                continue;
                            } else {
                                /* Line i and line j point in opposite direction. */
                                line.point = 0.5f * (lines[i].point + lines[j].point);
                            }
                        } else {
                            line.point = lines[i].point + (RVOMath.det(lines[j].direction, lines[i].point - lines[j].point) / determinant) * lines[i].direction;
                        }

                        line.direction = RVOMath.normalize(lines[j].direction - lines[i].direction);
                        projLines.Add(line);
                    }

                    Vector2 tempResult = result;
                    if (linearProgram2(projLines, radius, new Vector2(-lines[i].direction.y, lines[i].direction.x), true, ref result) < projLines.Count) {
                        /*
                         * This should in principle not happen. The result is by
                         * definition already in the feasible region of this
                         * linear program. If it fails, it is due to small
                         * floating point error, and the current result is kept.
                         */
                        result = tempResult;
                    }

                    distance = RVOMath.det(lines[i].direction, lines[i].point - result);
                }
            }
        }
    }
}
