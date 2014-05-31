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
        /// |   0  |   1  |   2  |   3  |   4  |   5  |   6  |
        /// |______|______|______|______|______|______|______|
        /// |      |      |      |      |      |      |      |
        /// |   7  |   8  |   9  |  10  |  11  |  12  |  13  |
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
        public List<int> freeSquare;

        // List of free square divided by column stack.
        // This will be usefull to track the next free square
        // for each column Ci.
        //
        // 35 36     41
        // 28 29     34
        // 21 22     27
        // 14 15     20
        //  7  8     13
        //  0  1      6
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


        public int GetTurnLeft()
        {
            return turnLeft;
        }


        internal bool IsValidMove(int square)
        {
            return (turnLeft > 0
                && freeSquare.Contains(square)
                && freeByColumn.ElementAt(square % 7).Peek().Equals(square));
        }


        /// <summary>
        /// Assuming the move is sound set the board[square]
        /// value to the one that represent the current player (1 or 2).
        /// Than remove the square from the list of free squares.
        /// </summary>
        /// <param name="square"></param>
        /// <param name="turn"></param>
        internal void Move(int square, int turn)
        {
            // Assuming the move is sound.
            board[square] = turn;
            freeSquare.Remove(square);
            freeByColumn.ElementAt(square % 7).Pop();
            turnLeft -= 1;
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
                checkDiag1(square, turn) ||
                checkDiag2(square, turn))
            {
                return true;
            }
            else
	        {
                return false;
	        }
        }

        // Up-left to Down-right diagonal.
        private bool checkDiag2(int square, int turn)
        {
            // Find the lowerbound.
            int lowerBound = findLB(square, 8);
            // Find the upperbound.
            int upperBound = findUB(square, 8);
            // Check for a connect 4 (step by 8).
            return checkConnect4(lowerBound, upperBound, 8, turn);
        }

        // Down-left to Up-right diagonal.
        private bool checkDiag1(int square, int turn)
        {
            // Find the lowerbound.
            int lowerBound = findLB(square, 6);
            // Find the upperbound.
            int upperBound = findUB(square, 6);
            // Check for a connect 4 (step by 6).
            return checkConnect4(lowerBound, upperBound, 6, turn);
        }

        private bool checkColumn(int square, int turn)
        {
            // Find the lowerbound.
            int lowerBound = findLB(square, 7);
            // Find the upperbound.
            int upperBound = findUB(square, 7);
            // Check for a connect 4 (step by 7).
            return checkConnect4(lowerBound, upperBound, 7, turn);
        }

        private bool checkRow(int square, int turn)
        {
            int bias = square % 7;
            int lowerBound = square - Math.Min(bias, 3);
            int upperBound = square - Math.Min(6 - bias, 3);
            // Check for a connect 4 (step by 1).
            return checkConnect4(lowerBound, upperBound, 1, turn);
        }

        private bool checkConnect4(int lowerBound, int upperBound, int step, int turn)
        {
            int four = 0;
            for (int i = lowerBound; i <= upperBound + step; )
            {
                if (four == 4)
                {
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

    }
}
