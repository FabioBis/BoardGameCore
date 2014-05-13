using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSearch;

namespace BoardGameCore
{
    public class TicTacToeMinMaxStrategy : TicTacToeStrategy
    {
        // The decision tree for the strategy.
        private DecisionTree<TicTacToeBoard> _decisionTree;

        // Integer value representing the player order:
        // if iaTurn = -1 the IA is the first to play, second to play otherwise.
        private int aiTurn;


        public DecisionTree<TicTacToeBoard> decisionTree
        {
            get
            {
                return _decisionTree;
            }
            set
            {
                _decisionTree = value;
            }
        }


        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy from a board state and the last
        /// decision that contributed to the given state.
        /// </summary>
        /// <param name="boardState">The state of the board.</param>
        /// <param name="decision">The decision that has produced the board state.</param>
        public TicTacToeMinMaxStrategy(TicTacToeBoard boardState, MinMaxDecision decision)
        {
            _decisionTree = new DecisionTree<TicTacToeBoard>(boardState, decision);
            if (boardState.IsEmpty())
            {
                aiTurn = -1;
            }
            else
            {
                aiTurn = 1;
            }
            DecisionTreeNode<TicTacToeBoard> rootNode = _decisionTree.GetRoot();
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
            if (aiTurn == -1)
            {
                // The AI is the first to play.
                _decisionTree = new DecisionTree<TicTacToeBoard>(
                    new TicTacToeBoard(),
                    new MinMaxDecision(null, MiniMax.Max)
                );
                // Populate the tree.
                buildNode(_decisionTree.GetRoot(), aiTurn, MiniMax.Max);
            }
            else
            {
                // The AI is the second to play.
                _decisionTree = new DecisionTree<TicTacToeBoard>(
                    new TicTacToeBoard(),
                    new MinMaxDecision(null, MiniMax.Min)
                );
                // Populate the tree.
                buildNode(_decisionTree.GetRoot(), aiTurn, MiniMax.Min);
            }
        }




        /// <summary>
        /// This method populates the decision tree for the current game.
        /// </summary>
        /// <param name="node">The root node.</param>
        /// <param name="turn">The IA turn.</param>
        private void populateTree(
            DecisionTreeNode<TicTacToeBoard> root,
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
            DecisionTreeNode<TicTacToeBoard> node,
            int turn,
            MiniMax type)
        {
            TicTacToeBoard nodeBoard = new TicTacToeBoard(node.Value);
            if (nodeBoard.isFull())
            {
                // Base: leaf node, evaluate the MiniMax value.
                ((MinMaxDecision)node.LastMove).SetValue(Fitness(nodeBoard));
                return;
            }
            MiniMax childrenType = swapType(type);
            foreach (int square in nodeBoard.freeSquare.ToList())
            {
                TicTacToeBoard childBoard = new TicTacToeBoard(nodeBoard);
                childBoard.Move(square, turn);
                MinMaxDecision decision = new MinMaxDecision(square, childrenType);
                DecisionTreeNode<TicTacToeBoard> childNode =
                    new DecisionTreeNode<TicTacToeBoard>(childBoard, decision);
                node.AddChild(childNode);
                int next = turn * -1;
                if (childBoard.CheckForWinner() == 0)
                {
                    buildNode(childNode, next, childrenType);
                }
                else
                {
                    // Base: leaf node, evaluate the MiniMax value.
                    ((MinMaxDecision)childNode.LastMove).SetValue(Fitness(
                        childBoard));
                }
            }
            // Select the MiniMax value for non-leaf node.
            int nodeValue = 0;
            if (type.Equals(MiniMax.Max))
            {
                nodeValue = Int32.MinValue;
                foreach (DecisionTreeNode<TicTacToeBoard> child in node.Children.ToList())
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
                foreach (DecisionTreeNode<TicTacToeBoard> child in node.Children.ToList())
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
            //Decision decision =
            //    decisionTree.MakeDecision(Heuristic.MAX);
            return makeDecision();
        }


        private int makeDecision()
        {
            return makeDecisionRec(decisionTree.GetRoot());
        }


        private int makeDecisionRec(
            DecisionTreeNode<TicTacToeBoard> node)
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
                foreach (DecisionTreeNode<TicTacToeBoard> child in node.Children)
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
