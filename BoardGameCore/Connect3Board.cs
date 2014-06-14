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
    public class Connect3Board : Board
    {
        /// Array representation of the board.
        ///  ______ ______ ______ ______ ______ 
        /// |      |      |      |      |      |
        /// |  00  |  01  |  02  |  03  |  04  |
        /// |______|______|______|______|______|
        /// |      |      |      |      |      |
        /// |  05  |  06  |  07  |  08  |  09  |
        /// |______|______|______|______|______|
        /// |      |      |      |      |      |
        /// |  10  |  11  |  12  |  13  |  14  |
        /// |______|______|______|______|______|
        /// |      |      |      |      |      |
        /// |  15  |  16  |  17  |  18  |  19  |
        /// |______|______|______|______|______|
        

        // The number of turn to fill the board.
        int turnLeft;

        // The winner: -1 (first player), 1 (second player), or 0 (no winner).
        int winner = 0;

        // List of free square.
        List<int> freeColumns;

        // List of free square divided by column stack.
        // This will be usefull to track the next free square
        // for each column Ci.
        //
        // 15 16     19
        // 10 11     14
        // 05 06     09
        // 00 01     04
        // -- -- ... --
        // C0 C1 ... C4
        public List<Stack<int>> freeByColumn;

        /* Checkable diagonals. */
        // "diagDLtoUR" + "X" where X is congruent square mod 4.
        static List<int> diagDLtoUR2 = new List<int>() { 2, 6, 10 };
        static List<int> diagDLtoUR3 = new List<int>() { 3, 7, 11, 15 };
        static List<int> diagDLtoUR0 = new List<int>() { 4, 8, 12, 16 };
        static List<int> diagDLtoUR1 = new List<int>() { 9, 13, 17 };
        // "diagULtoDR" + "X" where X is congruent square mod 6.  
        static List<int> diagULtoDR5 = new List<int>() { 5, 11, 17 };
        static List<int> diagULtoDR0 = new List<int>() { 0, 6, 12, 18 };
        static List<int> diagULtoDR1 = new List<int>() { 1, 7, 13, 19 };
        static List<int> diagULtoDR2 = new List<int>() { 2, 8, 14 };

        Dictionary<int, List<int>> diagsDLtoUR = new Dictionary<int, List<int>>() {
            {3, diagDLtoUR3}, 
            {0, diagDLtoUR0}, {1, diagDLtoUR1}, {2, diagDLtoUR2}
        };

        Dictionary<int, List<int>> diagsULtoDR = new Dictionary<int, List<int>>() {
            {2, diagULtoDR2}, {1, diagULtoDR1},
            {0, diagULtoDR0}, {5, diagULtoDR5}
        };

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Connect3Board()
        {
            freeColumns = new List<int>();
            freeByColumn = new List<Stack<int>>();
            for (int i = 0; i < 5; i++)
            {
                freeByColumn.Add(new Stack<int>());
                freeColumns.Add(i);
            }

            turnLeft = 20;
            board = new int[20];
            for (int i = 0; i < 20; i++)
            {
                // Fill the stack in position (i modulo 5) with i.
                freeByColumn.ElementAt((i % 5)).Push(i);                
                board[i] = 0;
            }
        }
        
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        public Connect3Board(Connect3Board source)
        {
            turnLeft = source.turnLeft;
            board = new int[20];
            freeColumns = new List<int>(source.freeColumns);
            freeByColumn = new List<Stack<int>>();
            foreach (Stack<int> column in source.freeByColumn.ToList())
            {
                freeByColumn.Add(new Stack<int>(new Stack<int>(column)));
            }
            for (int i = 0; i < 20; i++)
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
                && freeByColumn.ElementAt(columnIndex).Count != 0
                && gameOver == false);
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
            if (gameOver)
            {
                throw new InvalidOperationException(); 
            }
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
                CheckVictory(square, turn);
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
            if (turnLeft == 20)
            {
                // The board is empty, nothing to be done.
                return;
            }
            else
            {
                gameOver = false;
                winner = 0;
                int square = -1;
                if (freeByColumn.ElementAt(column).Count == 0)
                {
                    freeColumns.Add(column);
                    square = column;
                }
                else
                {
                    int freeSquareInColumn = freeByColumn.ElementAt(column).Peek();
                    if (freeSquareInColumn == (15 + column))
                    {
                        // The top of the stack contains a free square, invalid operation.
                        throw new InvalidOperationException();
                    }
                    square = freeSquareInColumn + 5;
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
            int x = square % 6;
            List<int> diag;
            if (diagsULtoDR.TryGetValue(x, out diag))
            {
                int lowerBound = diag.ElementAt(0);
                int upperBound = diag.ElementAt(diag.Count - 1);
                return checkConnect3Winner(lowerBound, upperBound, 6, turn);
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
            int x = square % 4;
            List<int> diag;
            if (diagsDLtoUR.TryGetValue(x, out diag))
            {
                int lowerBound = diag.ElementAt(0);
                int upperBound = diag.ElementAt(diag.Count - 1);
                return checkConnect3Winner(lowerBound, upperBound, 4, turn);
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
            int lowerBound = findLB(square, 5);
            // Find the upperbound.
            int upperBound = findUB(square, 5);
            // Check for a connect 3 (step by 5).
            return checkConnect3Winner(lowerBound, upperBound, 5, turn);
        }

        /// <summary>
        /// Checks if the givn player (turn) won by the row of the
        /// given square.
        /// </summary>
        private bool checkRow(int square, int turn)
        {
            int bias = square % 5;
            int lowerBound = square - Math.Min(bias, 2);
            int upperBound = square + Math.Min(4 - bias, 2);
            // Check for a connect 3 (step by 1).
            return checkConnect3Winner(lowerBound, upperBound, 1, turn);
        }

        /// <summary>
        /// Returns <code>true</code> if and only if the given turn player won th game
        /// (three adiacent square vertically, horizontally, or diagonally).
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
        private bool checkConnect3Winner(int lowerBound, int upperBound, int step, int turn)
        {
            int three = 0;
            for (int i = lowerBound; i <= upperBound + step; )
            {
                if (i < upperBound + step)
                {
                    if (board[i] == turn)
                    {
                        three++;
                        if (three == 3)
                        {
                            // Game is over.
                            winner = turn;
                            gameOver = true;
                            return true;
                        }
                    }
                    else
                    {
                        three = 0;
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
            while (upperBoundCheck < 20)
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

        /// <summary>
        /// Return the winner value: 1, -1 or 0 if no one won (yet).
        /// </summary>
        public int GetWinner()
        {
            return winner;
        }
    }
}
