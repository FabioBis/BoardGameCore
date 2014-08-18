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
    /// The Tic Tac Toe board representation.
    /// </summary>
    public class TicTacToeBoard : Board
    {

    /*   _________________
    /// |     |     |     |
    /// |  0  |  1  |  2  |
    /// |_____|_____|_____|
    /// |     |     |     |
    /// |  3  |  4  |  5  |
    /// |_____|_____|_____|
    /// |     |     |     |
    /// |  6  |  7  |  8  |
    /// |_____|_____|_____|
    */   

        int turnLeft;

        public List<int> freeSquare;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public TicTacToeBoard()
        {
            freeSquare = new List<int>();
            turnLeft = 9;
            board = new int[9];
            for (int i = 0; i < 9; i++)
            {
                freeSquare.Add(i);
                board[i] = 0;
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public TicTacToeBoard(TicTacToeBoard source)
        {
            turnLeft = source.turnLeft;
            board = new int[9];
            freeSquare = new List<int>(source.freeSquare);
            for (int i = 0; i < 9; i++)
            {
                board[i] = source.board[i];
            }
        }

        /// <summary>
        /// Returns the maximum number of turns until the game end.
        /// </summary>
        /// <returns></returns>
        public int GetTurnLeft()
        {
            return turnLeft;
        }

        /// <summary>
        /// Checks if a given move is valid.
        /// </summary>
        /// <param name="square">The next move index.</param>
        /// <returns><code>true</code> if the move is valid, <code>false</code>
        ///  otherwhise.</returns>
        public bool IsValidMove(int square)
        {
            return (turnLeft > 0 && freeSquare.Contains(square));
        }

        /// <summary>
        /// Allows to complete a move.
        /// </summary>
        /// <param name="square">The move index.</param>
        /// <param name="turn">The player turn: -1 represents the first player,
        /// 1 represents the second player.</param>
        public void Move(int square, int turn)
        {
            // Assuming the move is correct.
            board[square] = turn;
            freeSquare.Remove(square);
            turnLeft -= 1;
            if (turnLeft == 0)
            {
                gameOver = true;
            }
        }

        /// <summary>
        /// This method check the board to look for a winner.
        /// Returns -2 if the first player won, 2 if the second player won,
        /// 0 if the match is not yet finished.
        /// </summary>
        /// <returns>
        /// -2 if the first player won, 2 if the second player won,
        /// 0 if the match is not yet finished.
        /// </returns>
        public int CheckForWinner()
        {
            List<int> rows = new List<int>() {
                board[0] + board[1] + board[2],
                board[3] + board[4] + board[5],
                board[6] + board[7] + board[8],
                board[0] + board[3] + board[6],
                board[1] + board[4] + board[7],
                board[2] + board[5] + board[8],
                board[0] + board[4] + board[8],
                board[2] + board[4] + board[6]
            };

            foreach (int row in rows)
            {
                if (row == -3)
                {
                    gameOver = true;
                    return -2;
                }
                else if (row == 3)
                {
                    gameOver = true;
                    return 2;
                }
            }
            return 0;
        }

        /// <summary>
        /// Checks if the game board is empty.
        /// </summary>
        public bool IsEmpty()
        {
            int sum = 0;
            for (int i = 0; i < board.Length; i++)
            {
                sum += board[i];
            }
            return sum == 0;
        }

        /// <summary>
        /// Checks if the game board is full.
        /// </summary>
        public bool IsFull()
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if the game is over.
        /// </summary>
        /// <returns></returns>
        public bool Ended()
        {
            CheckForWinner();
            return gameOver;
        }

        /// <summary>
        /// This method perform a roll-back to the board state before
        /// the last move done. Each call roll back of one move at time.
        /// </summary>
        /// <param name="column">The square index of the move.</param>
        internal void UndoMove(int square)
        {
            if (turnLeft == 42)
            {
                // The board is empty, nothing to be done.
                return;
            }
            else
            {
                gameOver = false;
                board[square] = 0;
                freeSquare.Add(square);
                turnLeft += 1;
            }
        }
    }
}
