﻿//
// Copyright (c) 2014 Fabio Biselli - fabio.biselli.80@gmail.com
//
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
//    1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
//
//    2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
//
//    3. This notice may not be removed or altered from any source
//    distribution.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSearch;

namespace BoardGameCore
{
    /// <summary>
    /// This class implements a board game strategy for the
    /// Connect Four game. The strategy is based the Alpha-Beta
    /// pruning MiniMax variant.
    /// 
    /// Because the Alpha-Beta algorithm assumes that both players are
    /// optimal (and this is not the case), we have to avoid the case
    /// where the opponent (Min) moves into a game state discared from
    /// the tree by the algorithm. To fix this behavior, once the player
    /// make such a move, we discard the current tree and build a new one
    /// from the new game state.
    /// </summary>
    class Connect4AlphaBetaStrategy : BoardGameStrategy
    {
        // The decision tree for the strategy.
        private DecisionTree<Connect4Board> decisionTree;

        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy based on the given AI turn.
        /// if <code>turn</code> = 1 the IA is the first to play,
        /// if <code>turn</code> = 2 the AI is the second to play, otherwise
        /// an Argument exception is raised..
        /// </summary>
        /// <param name="turn">The AI turn.</param>
        public Connect4AlphaBetaStrategy(int turn)
        {
            if (turn == 1)
            {
                decisionTree = new DecisionTree<Connect4Board>(
                    new Connect4Board(),
                    new MinMaxDecision(null, MiniMax.Max)
                    );
                buildTree(decisionTree.GetRoot(), MiniMax.Max);
            }
            else if (turn == 2)
            {
                decisionTree = new DecisionTree<Connect4Board>(
                    new Connect4Board(),
                    new MinMaxDecision(null, MiniMax.Min));
                buildTree(decisionTree.GetRoot(), MiniMax.Min);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// This method build the given node, and recursively all the
        /// children using depth-first traversal with pre-order visit.
        /// </summary>
        /// <param name="node">The node to be built.</param>
        /// <param name="type">The MiniMax type of this node.</param>
        private void buildTree(
            DecisionTreeNode<Connect4Board> node,
            MiniMax type)
        {
            int alpha = Int32.MinValue;
            int beta = Int32.MaxValue;
            if (type.Equals(MiniMax.Max))
            {
                buildMaxNode(node, ref alpha,  ref beta);
            }
            else
            {
                buildMinNode(node, ref alpha, ref beta);
            }
        }

        /// <summary>
        /// Builds a Min node.
        /// </summary>
        /// <param name="node">The node to build.</param>
        /// <param name="alpha">The best Max value.</param>
        /// <param name="beta">The best Min value.</param>
        private int buildMinNode(
            DecisionTreeNode<Connect4Board> node,
            ref int alpha,
            ref int beta)
        {
            int nodeValue = Int32.MaxValue;
            if (node.Value.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                nodeValue = Fitness(node.Value);
            }
            else
            {
                foreach (int column in node.Value.GetFreeColumns().ToList())
                {
                    Connect4Board childBoard = new Connect4Board(node.Value);
                    // Max moves filling the board using integer value 1.
                    int squareMovedTo = childBoard.Move(column, 1);
                    MinMaxDecision decision =
                        new MinMaxDecision(column, MiniMax.Max);
                    DecisionTreeNode<Connect4Board> childNode = 
                        new DecisionTreeNode<Connect4Board>(childBoard, decision);
                    if (childBoard.CheckVictory(squareMovedTo, 1))
                    {
                        // Base: GameOver, leaf node, evaluate the MiniMax value.
                        ((MinMaxDecision)childNode.LastMove).SetValue(Fitness(
                            childBoard));
                    }
                    else
                    {
                        // Recursive: build the child.
                        int value = buildMaxNode(childNode, ref alpha, ref beta);
                        nodeValue = min(nodeValue, value);
                        if (nodeValue <= alpha)
                        {
                            // Pruned branch.
                            ((MinMaxDecision)node.LastMove).SetValue(value);
                            return value;
                        }
                        else
                        {
                            beta = min(beta, nodeValue);
                        }
                    }
                }
            }
            ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
            return nodeValue;
        }

        private int min(int a, int b)
        {
            if (a <= b)
            {
                return a;
            }
            else
	        {
                return b;
	        }
        }

        /// <summary>
        /// Build a Max node.
        /// </summary>
        /// <param name="node">The node to build.</param>
        /// <param name="alpha">The best Max value.</param>
        /// <param name="beta">The best Min value.</param>
        private int buildMaxNode(
            DecisionTreeNode<Connect4Board> node,
            ref int alpha,
            ref int beta)
        {
            if (node.Value.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                ((MinMaxDecision)node.LastMove).SetValue(Fitness(node.Value));
                return;
            }
            else
            {

            }
        }

        /// <summary>
        /// Evaluates the MinMax value related to the given board state.
        /// </summary>
        /// <param name="state">The board state.</param>
        /// <returns>The fitness (MinMax) value.</returns>
        private int Fitness(Connect4Board state)
        {
            // TODO
            throw new NotImplementedException();
        }

        // TODO
        public override int OwnMove()
        {
            throw new NotImplementedException();
        }

        // TODO
        public override void OpponentMove(int square)
        {
            throw new NotImplementedException();
        }

        // TODO
        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
