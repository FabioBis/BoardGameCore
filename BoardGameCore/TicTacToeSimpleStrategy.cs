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

namespace BoardGameCore
{
    /// <summary>
    /// This class implements a simple strategy for the TicTacToe board game.
    /// </summary>
    public class TicTacToeSimpleStrategy : BoardGameStrategy
    {
        // The game board internal representation.
        protected TicTacToeBoard board;
        // The turn: -1 this strategy move first, 1 otherwise.
        protected int aiTurn;
        // Auxiliary array to compute the next move.
        protected int[] boardArray = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="board">The initial board state.</param>
        /// <param name="turn">The AI strategy turn: -1 this strategy is the
        /// first to move, 1 otherwise.</param>
        public TicTacToeSimpleStrategy(TicTacToeBoard board, int turn)
        {
            this.board = new TicTacToeBoard(board);
            aiTurn = turn;
        }

        /// <summary>
        /// Implements the next own move for this strategy.
        /// </summary>
        /// <returns>The index of the square to move in.</returns>
        public override int OwnMove()
        {   
            int result = -1;
            board.BoardToArray(ref boardArray);
            List<int> rows = new List<int>() {
                boardArray[0] + boardArray[1] + boardArray[2],
                boardArray[3] + boardArray[4] + boardArray[5],
                boardArray[6] + boardArray[7] + boardArray[8],
                boardArray[0] + boardArray[3] + boardArray[6],
                boardArray[1] + boardArray[4] + boardArray[7],
                boardArray[2] + boardArray[5] + boardArray[8],
                boardArray[0] + boardArray[4] + boardArray[8],
                boardArray[2] + boardArray[4] + boardArray[6]
            };
            int i = 0;
            // Order matter: first avoid to lose, than try to win.
            if (aiTurn > 0)
            {
                foreach (int row in rows)
                {
                    if (row == -2)
                    {
                        // Do not loose.
                        result = selectFreeSquare(i);
                    }
                    else if (row == 2)
                    {
                        // Win.
                        result = selectFreeSquare(i);
                    }
                    i++;
                }
            }
            else
            {
                foreach (int row in rows)
                {
                    if (row == 2)
                    {
                        // Do not loose.
                        result = selectFreeSquare(i);
                    }
                    else if (row == -2)
                    {
                        // Win.
                        result = selectFreeSquare(i);
                    }
                    i++;
                }
            }
            if (result < 0)
            {
                Random random = new Random();
                int index = random.Next(0, board.GetTurnLeft());
                result = board.freeSquare.ElementAt(index);
            }
            board.Move(result, aiTurn);
            return result;
        }

        /// <summary>
        /// Selects the only free square among the given indexes.
        /// This method assumes that exactly one among p1, p2 and p3
        /// is a free square on the board. The given indexes represent
        /// a row, a column or a diagonal of the board.
        /// </summary>
        /// <param name="p1">Index 1.</param>
        /// <param name="p2">Index 2</param>
        /// <param name="p3">Index 3</param>
        /// <returns>The free square index.</returns>
        protected int selectFreeSquare(int p1, int p2, int p3)
        {
            if (boardArray[p1] == 0)
            {
                return p1;
            }
            else if (boardArray[p2] == 0)
            {
                return p2;
            }
            else
            {
                return p3;
            }
        }

        /// <summary>
        /// Selects a free square from a given pattern (rows, columns and
        /// diagolans) indexed by <code>i</code>.
        /// </summary>
        /// <param name="i">The index of a given pattern.</param>
        /// <returns>The index of the free square within the pattern.</returns>
        protected int selectFreeSquare(int i)
        {
            int result = -1;
            switch (i)
            {
                case 0:
                    result = selectFreeSquare(0, 1, 2);
                    break;
                case 1:
                    result = selectFreeSquare(3, 4, 5);
                    break;
                case 2:
                    result = selectFreeSquare(6, 7, 8);
                    break;
                case 3:
                    result = selectFreeSquare(0, 3, 6);
                    break;
                case 4:
                    result = selectFreeSquare(1, 4, 7);
                    break;
                case 5:
                    result = selectFreeSquare(2, 5, 8);
                    break;
                case 6:
                    result = selectFreeSquare(0, 4, 8);
                    break;
                case 7:
                    result = selectFreeSquare(2, 4, 6);
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Updates the board with the last opponent move.
        /// </summary>
        /// <param name="square">The last opponent move.</param>
        public override void OpponentMove(int square)
        {
            board.Move(square, -1*aiTurn);
        }

        /// <summary>
        /// Resets the internal board representation.
        /// </summary>
        public override void Reset()
        {
            this.board = new TicTacToeBoard(new TicTacToeBoard()); 
        }

    }

    /// <summary>
    /// This class implements a better simple strategy for the TicTacToe board
    /// game, slightly improving the first one.
    /// </summary>
    public class TicTacToeBetterSimpleStrategy : TicTacToeSimpleStrategy
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="board">The initial board state.</param>
        /// <param name="turn">The AI strategy turn: -1 this strategy is the
        /// first to move, 1 otherwise.</param>
        public TicTacToeBetterSimpleStrategy(TicTacToeBoard board, int turn)
            : base(board, turn)
        {
        }

        /// <summary>
        /// Implements the next own move for this strategy.
        /// </summary>
        /// <returns>The index of the square to move in.</returns>
        public override int OwnMove()
        {
            int result = -1;
            board.BoardToArray(ref boardArray);
            List<int> rows = new List<int>() {
                boardArray[0] + boardArray[1] + boardArray[2],
                boardArray[3] + boardArray[4] + boardArray[5],
                boardArray[6] + boardArray[7] + boardArray[8],
                boardArray[0] + boardArray[3] + boardArray[6],
                boardArray[1] + boardArray[4] + boardArray[7],
                boardArray[2] + boardArray[5] + boardArray[8],
                boardArray[0] + boardArray[4] + boardArray[8],
                boardArray[2] + boardArray[4] + boardArray[6]
            };

            int i = 0;
            // Order matter: first try to win, than avoid to lose.
            if (aiTurn > 0)
            {
                foreach (int row in rows)
                {
                    if (row == 2)
                    {
                        // Do not loose.
                        result = selectFreeSquare(i);
                    }
                    else if (row == -2)
                    {
                        // Win.
                        result = selectFreeSquare(i);
                    }
                    i++;
                }
            }
            else
            {
                foreach (int row in rows)
                {
                    if (row == -2)
                    {
                        // Do not loose.
                        result = selectFreeSquare(i);
                    }
                    else if (row == 2)
                    {
                        // Win.
                        result = selectFreeSquare(i);
                    }
                    i++;
                }
            }
            if (result < 0)
            {
                Random random = new Random();
                int index = random.Next(0, board.GetTurnLeft());
                result = board.freeSquare.ElementAt(index);
            }
            board.Move(result, aiTurn);
            return result;
        }
    }

}
