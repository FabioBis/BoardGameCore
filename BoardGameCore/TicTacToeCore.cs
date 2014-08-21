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
    public class TicTacToeCore
    {
        // The Tic Tac Toe board representation.
        private TicTacToeBoard board;

        // The player turn (-1 or 1).
        private int turn;

        // The current move number.
        private int moves;

        // The array of game moves.
        // At even indexes are stored first player moves, at odd indexes are
        // stored the second player moves, both in chronological order.
        private int[] movesDone;

        /// <summary>
        /// Default constructor. Initialise the game board (all square to 0),
        /// the first player turn and the turn left (the maximum turn for this game).
        /// </summary>
        public TicTacToeCore()
        {
            board = new TicTacToeBoard();
            turn = -1;
            moves = 0;
            movesDone = new int[9];
            for (int i = 0; i < 9; i++)
            {
                movesDone[i] = -1;
            }
        }

        public TicTacToeCore(TicTacToeBoard boardIn)
        {
            board = new TicTacToeBoard(boardIn);
            turn = -1;
            moves = 0;
            movesDone = new int[9];
            for (int i = 0; i < 9; i++)
            {
                movesDone[i] = -1;
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public TicTacToeCore(TicTacToeCore src)
        {
            moves = src.moves;
            movesDone = new int[9];
            for (int i = 0; i < 9; i++)
            {
                if (i < src.movesDone.Length)
                {
                    movesDone[i] = src.movesDone[i];
                }
                else
                {
                    movesDone[i] = -1;
                }
                board = new TicTacToeBoard(src.board);
                turn = src.turn;
            }
        }

        /// <summary>
        /// Getter method for the current player turn.
        /// </summary>
        /// <returns>The player turn.</returns>
        public int GetTurn()
        {
            return turn;
        }

        /// <summary>
        /// Getter method for the game board.
        /// </summary>
        /// <returns></returns>
        public TicTacToeBoard GetBoard()
        {
            return board;
        }

        /// <summary>
        /// This method implements the player move.
        /// The method returns true if the move of the current player
        /// is valid (the board square is empthy) and than sets the player
        /// turn value into the board square.
        /// </summary>
        /// <param name="square">The index of the board square representation.</param>
        /// <returns>true if the move is valid, false otherwise.</returns>
        public bool Move(int square)
        {
            if (!board.IsValidMove(square))
            {
                return false;
            }
            else
            {
                board.Move(square, turn);
                movesDone[moves] = square;
                moves++;
                turn *= -1;
                return true;
            }
        }

        /// <summary>
        /// This method check the board to look for a winner.
        /// </summary>
        /// <returns>
        /// -2 if the first player won, 2 if the second player won,
        /// 0 if the match is not yet finished, 1 if there is a draw.
        /// </returns>
        public int CheckForWinner()
        {
            int result = board.CheckForWinner();
            if (board.GetTurnLeft() > 0)
            {
                return result;
            }
            else
            {
                if (result == 0)
                {
                    return 1;
                }
                else
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// Returns <code>true</code> if the game is over.
        /// </summary>
        public bool GameOver()
        {
            if (board.GetTurnLeft() <= 0)
            {
                return true;
            }
            return board.Ended();
        }

        /// <summary>
        /// This method perform a roll-back to the board state before
        /// the last move done. Each call go back of one move at time.
        /// </summary>
        internal void UndoLastMove()
        {
            if (moves > 0 && moves <= 9)
            {
                int square = movesDone[moves - 1];
                board.UndoMove(square);
                turn *= -1;
                moves--;
                movesDone[moves] = -1;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
