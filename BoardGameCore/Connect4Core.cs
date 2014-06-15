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
    public class Connect4Core
    {
        // The Connect Four board representation.
        private Connect4Board board;

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
        public Connect4Core()
        {
            moves = 0;
            movesDone = new int[42];
            for (int i = 0; i < 42; i++)
            {
                movesDone[i] = -1;
            }
            board = new Connect4Board();
            turn = -1;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public Connect4Core(Connect4Core src)
        {
            moves = src.moves;
            movesDone = new int[42];
            for (int i = 0; i < 42; i++)
            {
                if (i < src.movesDone.Length)
                {
                    movesDone[i] = src.movesDone[i];
                }
                else
                {
                    movesDone[i] = -1;
                }
                board = new Connect4Board(src.board);
                turn = src.turn;
            }
        }

        /// <summary>
        /// Getter method for the current player turn.
        /// </summary>
        /// <returns>The player turn.</returns>
        public int GetNextTurn()
        {
            return turn;
        }

        /// <summary>
        /// Getter method for the last player turn.
        /// </summary>
        public int GetLastTurn()
        {
            return (turn * -1);
        }

        /// <summary>
        /// Getter method for the game board.
        /// </summary>
        /// <returns></returns>
        public Connect4Board GetBoard()
        {
            return board;
        }

        /// <summary>
        /// This method implements the player move.
        /// The method returns true if the move of the current player
        /// is valid (the board column is not full) and than sets the player
        /// turn value into the board square.
        /// </summary>
        /// <param name="square">The index of the board column.</param>
        /// <returns>true if the move is valid, false otherwise.</returns>
        public bool CheckAndMove(int columnIndex)
        {
            if (!board.IsValidMove(columnIndex))
            {
                return false;
            }
            else
            {
                movesDone[moves] = board.Move(columnIndex, turn);
                turn *= -1;
                moves++;
                return true;
            }
        }

        /// <summary>
        /// This method perform a roll-back to the board state before
        /// the last move done. Each call go back of one move at time.
        /// </summary>
        public void UndoLastMove()
        {
            if (moves > 0 && moves <= 42)
            {
                int column = movesDone[moves - 1] % 7;
                board.UndoMove(column);
                turn *= -1;
                moves--;
                movesDone[moves] = -1;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// This method implements the player move.
        /// </summary>
        /// <returns>The index of the square occupied on the board.</returns>
        public int Move(int columnIndex)
        {
            if (CheckAndMove(columnIndex))
            {
                return movesDone[moves - 1];
            }
            else
            {
                throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// Returns the index of the square of the last move.
        /// </summary>
        /// <returns></returns>
        public int GetLastSquareMove()
        {
            return movesDone[moves - 1];
        }

        /// <summary>
        /// Returns <code>true</code> if the last player won the game.
        /// </summary>
        public bool CheckVictory()
        {
            int lastTurn = turn * -1;
            return board.CheckVictory(movesDone[moves - 1], lastTurn);
        }

        /// <summary>
        /// Returns <code>true</code> if the last player represented by
        /// <code>turnIn</code> who just moved to <code>squareMovedTo</code>
        /// win the game.
        /// </summary>
        public bool CheckVictory(int squareMovedTo, int turnIn)
        {
            return board.CheckVictory(squareMovedTo, turnIn);
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
            bool thereIsAWinner = CheckVictory();
            if (board.GameOver())
            {
                if (thereIsAWinner)
                {
                    // The game is over.
                    return 2 * turn * -1;
                }
                else
                {
                    // The game is over without a winner.
                    return 1;
                }
            }
            else
            {
                // The game is not ended yet.
                return 0;
            }
        }

        /// <summary>
        /// Returns <code>true</code> if the game is over.
        /// </summary>
        public bool GameOver()
        {
            return board.GameOver();
        }

        /// <summary>
        /// Returns a list of the avaliable column indexes.
        /// </summary>
        public List<int> GetFreeColumns()
        {
            return board.GetFreeColumns();
        }
    }
}
