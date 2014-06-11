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

        // The winner: -1, 1, or 0 (no winner).
        int winner = 0;

        // List of free square.
        List<int> freeColumns;

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

        /* Checkable diagonals. */
        // "diagDLtoUR" + "X" where X is congruent square mod 6.
        static List<int> diagDLtoUR3 = new List<int>() { 3, 9, 15, 21 };
        static List<int> diagDLtoUR4 = new List<int>() { 4, 10, 16, 22, 28 };
        static List<int> diagDLtoUR5 = new List<int>() { 5, 11, 17, 23, 29, 35 };
        static List<int> diagDLtoUR0 = new List<int>() { 6, 12, 18, 24, 30, 36 };
        static List<int> diagDLtoUR1 = new List<int>() { 13, 19, 25, 31, 37 };
        static List<int> diagDLtoUR2 = new List<int>() { 20, 26, 32, 38 };
        // "diagULtoDR" + "X" where X is congruent square mod 8.  
        static List<int> diagULtoDR3 = new List<int>() { 3, 11, 19, 27 };
        static List<int> diagULtoDR2 = new List<int>() { 2, 10, 18, 26, 34 };
        static List<int> diagULtoDR1 = new List<int>() { 1, 9, 17, 25, 33, 41 };
        static List<int> diagULtoDR0 = new List<int>() { 0, 8, 16, 24, 32, 40 };
        static List<int> diagULtoDR7 = new List<int>() { 7, 15, 23, 31, 39 };
        static List<int> diagULtoDR6 = new List<int>() { 14, 22, 30, 38 };

        Dictionary<int, List<int>> diagsDLtoUR = new Dictionary<int,List<int>>() {
            {3, diagDLtoUR3}, {4, diagDLtoUR4}, {5, diagDLtoUR5},
            {0, diagDLtoUR0}, {1, diagDLtoUR1}, {2, diagDLtoUR2}
        };

        Dictionary<int, List<int>> diagsULtoDR = new Dictionary<int, List<int>>() {
            {3, diagULtoDR3}, {2, diagULtoDR2}, {1, diagULtoDR1},
            {0, diagULtoDR0}, {7, diagULtoDR7}, {6, diagULtoDR6}
        };

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Connect4Board()
        {
            freeColumns = new List<int>();
            freeByColumn = new List<Stack<int>>();
            for (int i = 0; i < 7; i++)
            {
                freeByColumn.Add(new Stack<int>());
                freeColumns.Add(i);
            }
            turnLeft = 42;
            board = new int[42];
            for (int i = 0; i < 42; i++)
            {
                // Fill the stack in position (i modulo 7) with i.
                // i.e. last column in position 6 will contains (i = 7*k + i mod 7):
                // 41 = 7*5 + 6, 34 = 7*4 + 6, 27 = 7*3 + 6, etc...
                freeByColumn.ElementAt((i % 7)).Push(i);                
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
            freeColumns = new List<int>(source.freeColumns);
            //freeByColumn = new List<Stack<int>>(source.freeByColumn);
            freeByColumn = new List<Stack<int>>();
            foreach (Stack<int> column in source.freeByColumn.ToList())
            {
                freeByColumn.Add(new Stack<int>(new Stack<int>(column)));
            }
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
        /// Returns a list of the avaliable column indexes.
        /// </summary>
        /// <returns></returns>
        public List<int> GetFreeColumns()
        {
            return freeColumns;
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
            if (turn == 1 || turn == -1)
	        {
                // Assuming the move is sound.
                int square = freeByColumn.ElementAt(column).Pop();
                if (freeByColumn.ElementAt(column).Count == 0)
                {
                    // The column is full, no more moves available.
                    freeColumns.Remove(column);
                }
                board[square] = turn;
                turnLeft -= 1;
                return square;
	        }
            // turn value must be equal 1 or -1.
            throw new ArgumentException();
        }

        /// <summary>
        /// This method perform a roll-back to the board state before
        /// the last move done. Each call go back of one move at time.
        /// </summary>
        /// <param name="column">The column index of the move.</param>
        public void UndoMove(int column)
        {
            if (turnLeft == 42)
            {
                // The board is empty, nothing to be done.
                return;
            }
            else
            {
                int square = -1;
                if (freeByColumn.ElementAt(column).Count == 0)
                {
                    square = column;
                }
                else
                {
                    int freeSquareInColumn = freeByColumn.ElementAt(column).Peek();
                    if (freeSquareInColumn == (35 + column))
                    {
                        // The top of the stack contains a free square, invalid operation.
                        throw new InvalidOperationException();
                    }
                    square = freeSquareInColumn + 7;
                }
                freeByColumn.ElementAt(column).Push(square);
                board[square] = 0;
                turnLeft += 1;
            }
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
            int x = square % 8;
            List<int> diag;
            if (diagsULtoDR.TryGetValue(x, out diag))
            {
                int lowerBound = diag.ElementAt(0);
                int upperBound = diag.ElementAt(diag.Count - 1);
                return checkConnect4Winner(lowerBound, upperBound, 8, turn);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the givn player (turn) won by the down-left to up-right
        /// diagonal of the given square.
        /// </summary>
        private bool checkDiagDLtoUR(int square, int turn)
        {
            int x = square % 6;
            List<int> diag;
            if (diagsDLtoUR.TryGetValue(x, out diag))
            {
                int lowerBound = diag.ElementAt(0);
                int upperBound = diag.ElementAt(diag.Count - 1);
                return checkConnect4Winner(lowerBound, upperBound, 6, turn);
            }
            else
            {
                return false;
            }
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
            int upperBound = square + Math.Min(6 - bias, 3);
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
                if (i < upperBound + step)
                {
                    if (board[i] == turn)
                    {
                        four++;
                        if (four == 4)
                        {
                            // Game is over.
                            winner = turn;
                            gameOver = true;
                            return true;
                        }
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
            int maxRange = 4;
            int lowerBoundCheck = square;
            int lowerBound = -1;
            while (lowerBoundCheck >= 0 && maxRange > 0)
            {
                lowerBound = lowerBoundCheck;
                maxRange--;
                lowerBoundCheck -= step;
            }
            return lowerBound;
        }

        /// <summary>
        /// Computes the upper bound from a square ad a given step.
        /// </summary>
        private int findUB(int square, int step)
        {
            int maxRange = 4;
            int upperBoundCheck = square;
            int upperBound = -1;
            while (upperBoundCheck < 42 && maxRange > 0)
            {
                upperBound = upperBoundCheck;
                maxRange--;
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

        /// <summary>
        /// Return the winner value: 1, -1 or 0 if no one won (yet).
        /// </summary>
        public int GetWinner()
        {
            return winner;
        }
    }
}
