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
    public class Connect4Board : Board
    {
        /// Array representation of the board.
        ///  ______ ______ ______ ______ ______ ______ ______
        /// |      |      |      |      |      |      |      |
        /// |  00  |  01  |  02  |  03  |  04  |  05  |  06  |
        /// |______|______|______|______|______|______|______|
        /// |      |      |      |      |      |      |      |
        /// |  07  |  08  |  09  |  10  |  11  |  12  |  13  |
        /// |______|______|______|______|______|______|______|
        /// |      |      |      |      |      |      |      |
        /// |  14  |  15  |  16  |  17  |  18  |  19  |  20  |
        /// |______|______|______|______|______|______|______|
        /// |      |      |      |      |      |      |      |
        /// |  21  |  22  |  23  |  24  |  25  |  26  |  27  |
        /// |______|______|______|______|______|______|______|
        /// |      |      |      |      |      |      |      |
        /// |  28  |  29  |  30  |  31  |  32  |  33  |  34  |
        /// |______|______|______|______|______|______|______|
        /// |      |      |      |      |      |      |      |
        /// |  35  |  36  |  37  |  38  |  39  |  40  |  41  |
        /// |______|______|______|______|______|______|______|     

        // The number of turn to fill the board.
        int turnLeft;

        // List of free square.
        List<int> freeSquare;

        // List of free square divided by column stack.
        // This will be usefull to track the next free square
        // for each column Ci.
        //
        // 35 36     41
        // 28 29     34
        // 21 22     27
        // 14 15     20
        // 07 08     13
        // 00 01     06
        // -- -- ... --
        // C0 C1 ... C6
        public List<Stack<int>> freeByColumn;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Connect4Board()
        {
            freeSquare = new List<int>();
            freeByColumn = new List<Stack<int>>();
            for (int i = 0; i < 7; i++)
            {
                freeByColumn.Add(new Stack<int>());
            }

            turnLeft = 42;
            board = new int[42];
            for (int i = 0; i < 42; i++)
            {
                // Fill the stack in position (i modulo 7) with i.
                // i.e. last column in position 6 will contains (i = 7*k + i mod 7):
                // 41 = 7*5 + 6, 34 = 7*4 + 6, 27 = 7*3 + 6, etc...
                freeByColumn.ElementAt((i % 7)).Push(i);
                // Fill the list of all free square.
                freeSquare.Add(i);
                board[i] = 0;
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Connect4Board(Connect4Board source)
        {
            turnLeft = source.turnLeft;
            board = new int[42];
            freeSquare = new List<int>(source.freeSquare);
            freeByColumn = new List<Stack<int>>(source.freeByColumn);
            for (int i = 0; i < 42; i++)
            {
                board[i] = source.board[i];
            }
        }

        /// <summary>
        /// Returns the maximum number of turns until the game ends.
        /// </summary>
        public int GetTurnLeft()
        {
            return turnLeft;
        }

        /// <summary>
        /// Returns true if and only if the given column index represents a
        /// column with some free squaree.
        /// </summary>
        /// <param name="square">The move to be checked.</param>
        /// <returns><code>true</code> if the move is valid, false otherwise.</returns>
        internal bool IsValidMove(int columnIndex)
        {
            return (turnLeft > 0
                && freeByColumn.ElementAt(columnIndex).Count != 0);
        }

        /// <summary>
        /// Assuming the move is sound set the board[square]
        /// value to the one that represents the current player (1 or 2).
        /// Than remove the square from the list of free squares.
        /// </summary>
        /// <param name="column">The column index of the move.</param>
        /// <param name="turn">The current player turn.</param>
        /// <returns>The index of the square occupied on the board.</returns>
        internal int Move(int column, int turn)
        {
            // Assuming the move is sound.
            int square = freeByColumn.ElementAt(column % 7).Pop();
            board[square] = turn;
            freeSquare.Remove(square);
            turnLeft -= 1;
            return square;
        }

        /// <summary>
        /// This method check the board to look if the last player who moved won.
        /// </summary>
        /// <returns>
        /// true if the player won, false otherwise.
        /// </returns>
        internal bool CheckVictory(int square, int turn)
        {
            if (checkRow(square, turn) ||
                checkColumn(square, turn) ||
                checkDiagDLtoUR(square, turn) ||
                checkDiagULtoDR(square, turn))
            {
                return true;
            }
            else
	        {
                return false;
	        }
        }

        /// <summary>
        /// Checks if the givn player (turn) won by the up-left to down-right
        /// diagonal of the given square.
        /// </summary>
        private bool checkDiagULtoDR(int square, int turn)
        {
            // Find the lowerbound.
            int lowerBound = findLB(square, 8);
            // Find the upperbound.
            int upperBound = findUB(square, 8);
            // Check for a connect 4 (step by 8).
            return checkConnect4Winner(lowerBound, upperBound, 8, turn);
        }

        /// <summary>
        /// Checks if the givn player (turn) won by the down-left to up-right
        /// diagonal of the given square.
        /// </summary>
        private bool checkDiagDLtoUR(int square, int turn)
        {
            // Find the lowerbound.
            int lowerBound = findLB(square, 6);
            // Find the upperbound.
            int upperBound = findUB(square, 6);
            // Check for a connect 4 (step by 6).
            return checkConnect4Winner(lowerBound, upperBound, 6, turn);
        }

        /// <summary>
        /// Checks if the givn player (turn) won by the column of the
        /// given square.
        /// </summary>
        private bool checkColumn(int square, int turn)
        {
            // Find the lowerbound.
            int lowerBound = findLB(square, 7);
            // Find the upperbound.
            int upperBound = findUB(square, 7);
            // Check for a connect 4 (step by 7).
            return checkConnect4Winner(lowerBound, upperBound, 7, turn);
        }

        /// <summary>
        /// Checks if the givn player (turn) won by the row of the
        /// given square.
        /// </summary>
        private bool checkRow(int square, int turn)
        {
            int bias = square % 7;
            int lowerBound = square - Math.Min(bias, 3);
            int upperBound = square - Math.Min(6 - bias, 3);
            // Check for a connect 4 (step by 1).
            return checkConnect4Winner(lowerBound, upperBound, 1, turn);
        }

        /// <summary>
        /// Returns <code>true</code> if and only if the given turn player won th game
        /// (four adiacent square vertically, horizontally, or diagonally).
        /// 
        /// This method check the board array from an index (lowerBound) to
        /// an other one (upperBound) and all the square between them by a
        /// given step. I.e.
        /// </summary>
        /// <param name="lowerBound">The index from start to.</param>
        /// <param name="upperBound">The index where end to.</param>
        /// <param name="step">The checking step.</param>
        /// <param name="turn">The player turn to be checked.</param>
        /// <returns><code>true</code> if the player won, false otherwise.</returns>
        private bool checkConnect4Winner(int lowerBound, int upperBound, int step, int turn)
        {
            int four = 0;
            for (int i = lowerBound; i <= upperBound + step; )
            {
                if (four == 4)
                {
                    // Game is over.
                    gameOver = true;
                    return true;
                }
                else if (i < upperBound + step)
                {
                    if (board[i] == turn)
                    {
                        four++;
                    }
                    else
                    {
                        four = 0;
                    }
                }
                i += step;
            }
            return false;
        }

        /// <summary>
        /// Computes the lower bound from a square ad a given step.
        /// </summary>
        private int findLB(int square, int step)
        {
            int lowerBoundCheck = square;
            int lowerBound = -1;
            while (lowerBoundCheck >= 0)
            {
                lowerBound = lowerBoundCheck;
                lowerBoundCheck -= step;
            }
            return lowerBound;
        }

        /// <summary>
        /// Computes the upper bound from a square ad a given step.
        /// </summary>
        private int findUB(int square, int step)
        {
            int upperBoundCheck = square;
            int upperBound = -1;
            while (upperBoundCheck < 42)
            {
                upperBound = upperBoundCheck;
                upperBoundCheck += step;
            }
            return upperBound;
        }

        /// <summary>
        /// Returns <code>true</code> if the board is full or
        /// there are at least four connected squares for the same
        /// player.
        /// </summary>
        /// <returns></returns>
        internal bool GameOver()
        {
            return (turnLeft <= 0 || gameOver);
        }
    }
}
