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


        public int GetTurnLeft()
        {
            return turnLeft;
        }


        internal bool IsValidMove(int square)
        {
            return (turnLeft > 0 && freeSquare.Contains(square));
        }


        internal void Move(int square, int turn)
        {
            // Assuming the move is sound.
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
        internal int CheckForWinner()
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


        internal bool IsEmpty()
        {
            int sum = 0;
            for (int i = 0; i < board.Length; i++)
            {
                sum += board[i];
            }
            return sum == 0;
        }


        internal bool isFull()
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


        internal bool End()
        {
            CheckForWinner();
            return gameOver;
        }
    }
}
