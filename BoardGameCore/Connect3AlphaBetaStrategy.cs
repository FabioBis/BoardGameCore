//
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
    public class Connect3AlphaBetaStrategy : BoardGameStrategy
    {

        // The decision tree for the strategy.
        private DecisionTree decisionTree;

        private Connect3Core realBoardState;
        private Connect3Core strategyBoardState;

        // The AI turn. Used to evaluate the fitness function.
        int aiTurn;

        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy based on the given AI turn.
        /// if <code>turn</code> = 1 the IA is the first to play,
        /// if <code>turn</code> = 2 the AI is the second to play, otherwise
        /// an Argument exception is raised..
        /// </summary>
        /// <param name="turn">The AI turn.</param>
        public Connect3AlphaBetaStrategy(int turn)
        {
            strategyBoardState = new Connect3Core();
            realBoardState = new Connect3Core();
            if (turn == 1)
            {
                aiTurn = -1;
                decisionTree = new DecisionTree(
                    new MinMaxDecision(null, MiniMax.Max)
                    );
                buildTree(decisionTree.GetRoot(), MiniMax.Max);
            }
            else if (turn == 2)
            {
                aiTurn = 1;
                decisionTree = new DecisionTree(
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
            DecisionTreeNode node,
            MiniMax type)
        {
            int alpha = Int32.MinValue;
            int beta = Int32.MaxValue;
            if (type.Equals(MiniMax.Max))
            {
                buildMaxNode(node, alpha, beta);
            }
            else
            {
                buildMinNode(node, alpha, beta);
            }
        }

        /// <summary>
        /// Builds a Min node.
        /// </summary>
        /// <param name="node">The node to build.</param>
        /// <param name="alpha">The current best Max value.</param>
        /// <param name="beta">The current best Min value.</param>
        private int buildMinNode(
            DecisionTreeNode node,
            int alpha,
            int beta)
        {
            int nodeValue = int.MaxValue;
            if (strategyBoardState.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                nodeValue = Fitness(strategyBoardState.GetBoard());
            }
            else
            {
                foreach (int column in strategyBoardState.GetFreeColumns().ToList())
                {
                    // Max moves.
                    int squareMovedTo = strategyBoardState.Move(column);
                    MinMaxDecision decision =
                        new MinMaxDecision(column, MiniMax.Max);
                    DecisionTreeNode childNode =
                        new DecisionTreeNode(decision);
                    node.AddChild(childNode);
                    int value = buildMaxNode(childNode, alpha, beta);
                    nodeValue = Decision.Min(nodeValue, value);
                    if (nodeValue <= alpha)
                    {
                        // Pruned branch.
                        ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
                        strategyBoardState.UndoLastMove();
                        return nodeValue;
                    }
                    else
                    {
                        beta = Decision.Min(beta, nodeValue);
                    }
                    strategyBoardState.UndoLastMove();
                }
            }
            ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
            return nodeValue;
        }

        /// <summary>
        /// Build a Max node.
        /// </summary>
        /// <param name="node">The node to build.</param>
        /// <param name="alpha">The best Max value.</param>
        /// <param name="beta">The best Min value.</param>
        private int buildMaxNode(
            DecisionTreeNode node,
            int alpha,
            int beta)
        {
            int nodeValue = int.MinValue;
            if (strategyBoardState.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                nodeValue = Fitness(strategyBoardState.GetBoard());
            }
            else
            {
                foreach (int column in strategyBoardState.GetFreeColumns().ToList())
                {
                    // Min moves.
                    int squareMovedTo = strategyBoardState.Move(column);
                    MinMaxDecision decision =
                        new MinMaxDecision(column, MiniMax.Min);
                    DecisionTreeNode childNode =
                        new DecisionTreeNode(decision);
                    node.AddChild(childNode);
                    int value = buildMinNode(childNode, alpha, beta);
                    nodeValue = Decision.Max(nodeValue, value);
                    if (nodeValue >= beta)
                    {
                        // Pruned branch.
                        ((MinMaxDecision)node.LastMove).SetValue((nodeValue));
                        strategyBoardState.UndoLastMove();
                        return nodeValue;
                    }
                    else
                    {
                        alpha = Decision.Max(alpha, nodeValue);
                    }
                    strategyBoardState.UndoLastMove();
                }
            }
            ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
            return nodeValue;
        }

        /// <summary>
        /// Evaluates the MinMax value related to the given board state.
        /// </summary>
        /// <param name="state">The board state.</param>
        /// <returns>The fitness (MinMax) value.</returns>
        private int Fitness(Connect3Board state)
        {
            return aiTurn *
                (state.GetWinner() + state.GetWinner() * (state.GetTurnLeft()^2));
        }

        /// <summary>
        /// The AI player move.
        /// </summary>
        /// <returns>The column index of the game board to move in.</returns>
        public override int OwnMove()
        {
            int column = makeDecisionRec(decisionTree.GetRoot());
            realBoardState.Move(column);
            return column;
        }

        private int makeDecisionRec(
            DecisionTreeNode node)
        {
            if (node.GetBranches() == 0)
            {
                // Base: this is a leaf only one decision expected.
                return (int)node.LastMove.GetImpl();
            }
            else if (node.DecisionMade())
            {
                // Recursive: only child, no decision needed at this level.
                return makeDecisionRec(node.GetChoosenChildren());
            }
            else
            {
                // Base: multiple branches, the best decision is needed.
                int index = -1;
                int minMax = Int32.MinValue;
                int decision = -1;
                foreach (DecisionTreeNode child in node.Children.ToList())
                {
                    int tmpMinMax = ((MinMaxDecision)child.LastMove).GetValue();
                    // Keep the oldest best value (left-most).
                    if (tmpMinMax > minMax)
                    {
                        minMax = tmpMinMax;
                        index = node.Children.IndexOf(child);
                        decision = (int)child.LastMove.GetImpl();
                    }
                }
                decisionTree.MakeDecision(index);
                return decision;
            }
        }

        /// <summary>
        /// The opponent make his move.
        /// </summary>
        public override void OpponentMove(int column)
        {
            realBoardState.Move(column);
            MinMaxDecision decision = new MinMaxDecision(column, MiniMax.Min);
            if (decisionTree.NextDecisionPlanned(decision))
            {
                // The next decision tree state will be consistent.
                decisionTree.ExternalDecisionMade(decision);
            }
            else
            {
                // The next decision tree state will be inconsistent.
                // We have to rebuild the tree from the new state.
                strategyBoardState = new Connect3Core(realBoardState);
                aiTurn = strategyBoardState.GetNextTurn();
                decisionTree = new DecisionTree(
                    new MinMaxDecision(column, MiniMax.Max)
                    );
                buildTree(decisionTree.GetRoot(), MiniMax.Max);
            }
        }

        /// <summary>
        /// Resets recursively all the decision taken in the tree.
        /// </summary>
        public override void Reset()
        {
            decisionTree.ResetDecisions();
        }
    }
}
