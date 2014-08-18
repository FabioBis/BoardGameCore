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
    public class TicTacToeMinMaxStrategy : BoardGameStrategy
    {
        // The decision tree for the strategy.
        protected DecisionTree decisionTree;

        protected TicTacToeCore strategyBoardState;

        // Integer value representing the player order:
        // if iaTurn = -1 the IA is the first to play, second to play otherwise.
        private int aiTurn;


        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy from a board state and the last
        /// decision that contributed to the given state.
        /// </summary>
        /// <param name="boardState">The state of the board.</param>
        /// <param name="decision">The decision that has produced the board state.</param>
        public TicTacToeMinMaxStrategy(TicTacToeBoard boardState, MinMaxDecision decision)
        {
            strategyBoardState = new TicTacToeCore(boardState);
            decisionTree = new DecisionTree(decision);
            if (boardState.IsEmpty())
            {
                aiTurn = -1;
            }
            else
            {
                aiTurn = 1;
            }
            DecisionTreeNode rootNode = decisionTree.GetRoot();
            populateTree(rootNode, aiTurn);
        }

        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy from the turn of the first to play.
        /// The decision tree is built entirely from the empty board.
        /// </summary>
        /// <param name="turn"></param>
        public TicTacToeMinMaxStrategy(int turn)
        {
            aiTurn = turn;
            strategyBoardState = new TicTacToeCore();
            if (aiTurn == -1)
            {
                // The AI is the first to play.
                decisionTree = new DecisionTree(
                    new MinMaxDecision(null, MiniMax.Max));
                // Populate the tree.
                buildNode(decisionTree.GetRoot(), -1, MiniMax.Max);
            }
            else
            {
                // The AI is the second to play.
                decisionTree = new DecisionTree(
                    new MinMaxDecision(null, MiniMax.Min));
                // Populate the tree.
                buildNode(decisionTree.GetRoot(), -1, MiniMax.Min);
            }
        }

        /// <summary>
        /// This method populates the decision tree for the current game.
        /// </summary>
        /// <param name="node">The root node.</param>
        /// <param name="turn">The IA turn.</param>
        private void populateTree(
            DecisionTreeNode root,
            int turn)
        {
            buildNode(root, turn, MiniMax.Max);
        }

        /// <summary>
        /// Returns the opposite MiniMax type of <code>source</code>.
        /// </summary>
        private MiniMax swapType(MiniMax source)
        {
            if (source.Equals(MiniMax.Max))
            {
                return MiniMax.Min;
            }
            else
            {
                return MiniMax.Max;
            }
        }
        
        /// <summary>
        /// This method build the given node, and recursively all the
        /// children using depth-first traversal with pre-order visit.
        /// </summary>
        /// <param name="node">The node to be built.</param>
        /// <param name="turn">The current turn.</param>
        /// <param name="type">The MiniMax type of this node.</param>
        private void buildNode(
            DecisionTreeNode node,
            int turn,
            MiniMax type)
        {
            if (strategyBoardState.GameOver())
            {
                // Base: leaf node, evaluate the MiniMax value.
                ((MinMaxDecision)node.LastMove).SetValue(Fitness(strategyBoardState.GetBoard()));
                return;
            }
            // Recursive: build the children.
            MiniMax childrenType = swapType(type);
            foreach (int square in strategyBoardState.GetBoard().freeSquare.ToList())
            {
                strategyBoardState.Move(square);
                MinMaxDecision decision = new MinMaxDecision(square, childrenType);
                DecisionTreeNode childNode =
                    new DecisionTreeNode(decision);
                node.AddChild(childNode);
                int next = turn * -1;
                if (strategyBoardState.CheckForWinner() == 0)
                {
                    // Recursive: build the child.
                    buildNode(childNode, next, childrenType);
                }
                else
                {
                    // Base: leaf node, evaluate the MiniMax value.
                    ((MinMaxDecision)childNode.LastMove).SetValue(Fitness(
                        strategyBoardState.GetBoard()));
                }
                strategyBoardState.UndoLastMove();
            }
            int nodeValue = 0;
            if (type.Equals(MiniMax.Max))
            {
                nodeValue = Int32.MinValue;
                foreach (DecisionTreeNode child in node.Children.ToList())
                {
                    int currentValue = ((MinMaxDecision)child.LastMove).GetValue();
                    if (nodeValue < currentValue)
                    {
                        nodeValue = currentValue; 
                    }
                }
            }
            else
            {
                nodeValue = Int32.MaxValue;
                foreach (DecisionTreeNode child in node.Children.ToList())
                {
                    int currentValue = ((MinMaxDecision)child.LastMove).GetValue();
                    if (nodeValue > currentValue)
                    {
                        nodeValue = currentValue;
                    }
                }
            }
            ((MinMaxDecision)node.LastMove).SetValue(nodeValue);
        }


        /// <summary>
        /// Evaluates the MinMax value related to the given board state.
        /// </summary>
        /// <param name="state">The board state.</param>
        /// <returns>The fitness (MinMax) value.</returns>
        public int Fitness(TicTacToeBoard state)
        {
            int result = state.CheckForWinner() * aiTurn;
            if (result < 0)
            {
                return result -state.GetTurnLeft();
            }
            else
            {
                return result +state.GetTurnLeft();
            }
        }

        override public int OwnMove()
        {
            return makeDecision();
        }

        private int makeDecision()
        {
            return makeDecisionRec(decisionTree.GetRoot());
        }

        private int makeDecisionRec(
            DecisionTreeNode node)
        {
            if (node.GetBranches() == 0)
            {
                // Base: this is a leaf only one decision expected.
                return (int) node.LastMove.GetImpl();
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
                foreach (DecisionTreeNode child in node.Children)
                {
                    int tmpMinMax = ((MinMaxDecision)child.LastMove).GetValue();
                    if (tmpMinMax > minMax)
                    {
                        minMax = tmpMinMax;
                        index = node.Children.IndexOf(child);
                        decision = (int) child.LastMove.GetImpl();
                    }
                }
                decisionTree.MakeDecision(index);
                return decision;
            }
        }

        override public void OpponentMove(int square)
        {
            decisionTree.ExternalDecisionMade(
                new MinMaxDecision(square, MiniMax.Min));
        }

        /// <summary>
        /// Resets recursively all the decision taken in the tree.
        /// </summary>
        public void ResetDecisions()
        {
            decisionTree.ResetDecisions();
        }


        public override void Reset()
        {
            decisionTree.ResetDecisions();
        }
    }
}
