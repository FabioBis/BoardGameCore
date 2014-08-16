﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSearch;

namespace BoardGameCore
{
    public class Connect4AlphaBetaLimitedStrategy : Connect4AlphaBetaStrategy
    {
        int depth;

        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy based on the given AI turn.
        /// if <code>turn</code> = 1 the IA is the first to play,
        /// if <code>turn</code> = 2 the AI is the second to play, otherwise
        /// an Argument exception is raised..
        /// </summary>
        /// <param name="turn">The AI turn.</param>
        public Connect4AlphaBetaLimitedStrategy(int turn)
        {
            depth = 13;
            strategyBoardState = new Connect4Core();
            realBoardState = new Connect4Core();
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
            depth--;
            int nodeValue = Int32.MaxValue;
            if (strategyBoardState.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                nodeValue = Fitness(strategyBoardState.GetBoard());
            }
            else if (depth == 0)
            {
                // Base: max depth reached.
                nodeValue = EvaluateState(strategyBoardState.GetBoard(), -1*aiTurn);
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
                        ((MinMaxDecision)node.LastMove).SetValue(value);
                        strategyBoardState.UndoLastMove();
                        depth++;
                        return value;
                    }
                    else
                    {
                        beta = Decision.Min(beta, nodeValue);
                    }
                    strategyBoardState.UndoLastMove();
                }
            }
            ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
            depth++;
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
            depth--;
            int nodeValue = Int32.MinValue;
            if (strategyBoardState.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                nodeValue = Fitness(strategyBoardState.GetBoard());
            }
            else if (depth == 0)
            {
                // Base: max depth reached.
                nodeValue = EvaluateState(strategyBoardState.GetBoard(), aiTurn);
            }
            else if (strategyBoardState.GetBoard().Evaluate(aiTurn) >= 0)
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
                        ((MinMaxDecision)node.LastMove).SetValue(value);
                        strategyBoardState.UndoLastMove();
                        depth++;
                        return value;
                    }
                    else
                    {
                        alpha = Decision.Max(alpha, nodeValue);
                    }
                    strategyBoardState.UndoLastMove();
                }
            }
            else
            {
                // Cut: the opponent will win in one move. Here we assume that the
                // opponent is an optimal player, so the tree is pruned  to allow
                // a more depth search.
                nodeValue = EvaluateState(strategyBoardState.GetBoard(), aiTurn);
            }
            ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
            depth++;
            return nodeValue;
        }

        /// <summary>
        /// Evaluates the MinMax value related to the given board (non-final) state.
        /// </summary>
        private int EvaluateState(Connect4Board state, int turn)
        {
            int coef = state.Evaluate(turn);
            if (coef == 0)
            {
                return coef;
            }
            else
            {
                coef = coef / Math.Abs(coef);
                return coef * state.GetTurnLeft();
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
                strategyBoardState = new Connect4Core(realBoardState);
                aiTurn = strategyBoardState.GetNextTurn();
                decisionTree = new DecisionTree(
                    new MinMaxDecision(column, MiniMax.Max)
                    );
                buildTree(decisionTree.GetRoot(), MiniMax.Max);
            }
        }
    }
}
