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
    /// <summary>
    /// This class implements a board game strategy for the
    /// Connect Four game. The strategy is based the alpha-beta
    /// pruning MiniMax variant.
    /// </summary>
    class Connect4AlphaBetaStrategy : BoardGameStrategy
    {
        // The decision tree for the strategy.
        private DecisionTree<Connect4Board> decisionTree;

        // Integer value representing the player order:
        //   if aiTurn = -1 the AI is the first to play, second to play
        //   otherwise.
        private int aiTurn;

        /// <summary>
        /// Constructor.
        /// This constructor builds a new strategy based on the given AI turn.
        /// if <code>turn</code> = -1 the IA is the first to play, second to
        /// play otherwise.
        /// </summary>
        /// <param name="turn">The AI turn.</param>
        public Connect4AlphaBetaStrategy(int turn)
        {
            if (turn == -1)
            {
                decisionTree = new DecisionTree<Connect4Board>(
                    new Connect4Board(),
                    new MinMaxDecision(null, MiniMax.Max)
                    );
                buildNode(decisionTree.GetRoot(), turn, MiniMax.Max);
            }
            else if (turn == 1)
            {
                decisionTree = new DecisionTree<Connect4Board>(
                    new Connect4Board(),
                    new MinMaxDecision(null, MiniMax.Min));
                buildNode(decisionTree.GetRoot(), turn, MiniMax.Min);
            }
            else
            {
                throw new ArgumentException();
            }
            aiTurn = turn;
        }

        /// <summary>
        /// This method build the given node, and recursively all the
        /// children using depth-first traversal with pre-order visit.
        /// </summary>
        /// <param name="node">The node to be built.</param>
        /// <param name="turn">The current turn.</param>
        /// <param name="type">The MiniMax type of this node.</param>
        private void buildNode(
            DecisionTreeNode<Connect4Board> node,
            int turn,
            MiniMax type)
        {
            // TODO
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
