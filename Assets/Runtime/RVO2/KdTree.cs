/*
 * KdTree.cs
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
     * <summary>Defines k-D trees for agents and static obstacles in the
     * simulation.</summary>
     */
    internal class KdTree {
        /**
         * <summary>Defines a node of an agent k-D tree.</summary>
         */
        struct AgentTreeNode {
            internal int begin_;
            internal int end_;
            internal int left_;
            internal int right_;
            internal float maxX_;
            internal float maxY_;
            internal float minX_;
            internal float minY_;
        }

        /**
         * <summary>Defines a pair of scalar values.</summary>
         */
        struct FloatPair {
            readonly float a_;
            readonly float b_;

            /**
             * <summary>Constructs and initializes a pair of scalar
             * values.</summary>
             *
             * <param name="a">The first scalar value.</param>
             * <param name="b">The second scalar value.</param>
             */
            internal FloatPair(float a, float b) {
                a_ = a;
                b_ = b;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is less
             * than the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is less than the
             * second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator <(FloatPair pair1, FloatPair pair2) {
                return pair1.a_ < pair2.a_ || !(pair2.a_ < pair1.a_) && pair1.b_ < pair2.b_;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is less
             * than or equal to the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is less than or
             * equal to the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator <=(FloatPair pair1, FloatPair pair2) {
                return (pair1.a_ == pair2.a_ && pair1.b_ == pair2.b_) || pair1 < pair2;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is
             * greater than the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is greater than
             * the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator >(FloatPair pair1, FloatPair pair2) {
                return !(pair1 <= pair2);
            }

            /**
             * <summary>Returns true if the first pair of scalar values is
             * greater than or equal to the second pair of scalar values.
             * </summary>
             *
             * <returns>True if the first pair of scalar values is greater than
             * or equal to the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator >=(FloatPair pair1, FloatPair pair2) {
                return !(pair1 < pair2);
            }
        }

        /**
         * <summary>The maximum size of an agent k-D tree leaf.</summary>
         */
        const int MAX_LEAF_SIZE = 10;

        Agent[] agents_;
        AgentTreeNode[] agentTree_;

        /**
         * <summary>Builds an agent k-D tree.</summary>
         */
        internal void buildAgentTree(IList<Agent> agents) {
            if (agents_ == null || agents_.Length != agents.Count) {
                agents_ = new Agent[agents.Count];

                for (int i = 0; i < agents_.Length; ++i) {
                    agents_[i] = agents[i];
                }

                agentTree_ = new AgentTreeNode[2 * agents_.Length];

                for (int i = 0; i < agentTree_.Length; ++i) {
                    agentTree_[i] = new AgentTreeNode();
                }
            }

            if (agents_.Length != 0) {
                buildAgentTreeRecursive(0, agents_.Length, 0);
            }
        }

        /**
         * <summary>Computes the agent neighbors of the specified agent.
         * </summary>
         *
         * <param name="agent">The agent for which agent neighbors are to be
         * computed.</param>
         * <param name="rangeSq">The squared range around the agent.</param>
         */
        internal void computeAgentNeighbors(Agent agent, ref float rangeSq) {
            queryAgentTreeRecursive(agent, ref rangeSq, 0);
        }

        /**
         * <summary>Recursive method for building an agent k-D tree.</summary>
         *
         * <param name="begin">The beginning agent k-D tree node node index.
         * </param>
         * <param name="end">The ending agent k-D tree node index.</param>
         * <param name="node">The current agent k-D tree node index.</param>
         */
        void buildAgentTreeRecursive(int begin, int end, int node) {
            agentTree_[node].begin_ = begin;
            agentTree_[node].end_ = end;
            agentTree_[node].minX_ = agentTree_[node].maxX_ = agents_[begin].position_.x;
            agentTree_[node].minY_ = agentTree_[node].maxY_ = agents_[begin].position_.y;

            for (int i = begin + 1; i < end; ++i) {
                agentTree_[node].maxX_ = Math.Max(agentTree_[node].maxX_, agents_[i].position_.x);
                agentTree_[node].minX_ = Math.Min(agentTree_[node].minX_, agents_[i].position_.x);
                agentTree_[node].maxY_ = Math.Max(agentTree_[node].maxY_, agents_[i].position_.y);
                agentTree_[node].minY_ = Math.Min(agentTree_[node].minY_, agents_[i].position_.y);
            }

            if (end - begin > MAX_LEAF_SIZE) {
                /* No leaf node. */
                bool isVertical = agentTree_[node].maxX_ - agentTree_[node].minX_ > agentTree_[node].maxY_ - agentTree_[node].minY_;
                float splitValue = 0.5f * (isVertical ? agentTree_[node].maxX_ + agentTree_[node].minX_ : agentTree_[node].maxY_ + agentTree_[node].minY_);

                int left = begin;
                int right = end;

                while (left < right) {
                    while (left < right && (isVertical ? agents_[left].position_.x : agents_[left].position_.y) < splitValue) {
                        ++left;
                    }

                    while (right > left && (isVertical ? agents_[right - 1].position_.x : agents_[right - 1].position_.y) >= splitValue) {
                        --right;
                    }

                    if (left < right) {
                        Agent tempAgent = agents_[left];
                        agents_[left] = agents_[right - 1];
                        agents_[right - 1] = tempAgent;
                        ++left;
                        --right;
                    }
                }

                int leftSize = left - begin;

                if (leftSize == 0) {
                    ++leftSize;
                    ++left;
                }

                agentTree_[node].left_ = node + 1;
                agentTree_[node].right_ = node + 2 * leftSize;

                buildAgentTreeRecursive(begin, left, agentTree_[node].left_);
                buildAgentTreeRecursive(left, end, agentTree_[node].right_);
            }
        }


        /**
         * <summary>Recursive method for computing the agent neighbors of the
         * specified agent.</summary>
         *
         * <param name="agent">The agent for which agent neighbors are to be
         * computed.</param>
         * <param name="rangeSq">The squared range around the agent.</param>
         * <param name="node">The current agent k-D tree node index.</param>
         */
        void queryAgentTreeRecursive(Agent agent, ref float rangeSq, int node) {
            if (agentTree_[node].end_ - agentTree_[node].begin_ <= MAX_LEAF_SIZE) {
                for (int i = agentTree_[node].begin_; i < agentTree_[node].end_; ++i) {
                    agent.insertAgentNeighbor(agents_[i], ref rangeSq);
                }
            } else {
                float distSqLeft = RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].left_].minX_ - agent.position_.x)) + RVOMath.sqr(Math.Max(0.0f, agent.position_.x - agentTree_[agentTree_[node].left_].maxX_)) + RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].left_].minY_ - agent.position_.y)) + RVOMath.sqr(Math.Max(0.0f, agent.position_.y - agentTree_[agentTree_[node].left_].maxY_));
                float distSqRight = RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].right_].minX_ - agent.position_.x)) + RVOMath.sqr(Math.Max(0.0f, agent.position_.x - agentTree_[agentTree_[node].right_].maxX_)) + RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].right_].minY_ - agent.position_.y)) + RVOMath.sqr(Math.Max(0.0f, agent.position_.y - agentTree_[agentTree_[node].right_].maxY_));

                if (distSqLeft < distSqRight) {
                    if (distSqLeft < rangeSq) {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left_);

                        if (distSqRight < rangeSq) {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right_);
                        }
                    }
                } else {
                    if (distSqRight < rangeSq) {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right_);

                        if (distSqLeft < rangeSq) {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left_);
                        }
                    }
                }

            }
        }

    }
}
