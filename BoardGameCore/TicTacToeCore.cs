using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameCore
{
    public class TicTacToeCore
    {
        // The tic Tac Toe board representation.
        private static TicTacToeBoard board;

        // The player turn (-1 or 1).
        private static int turn;


        /// <summary>
        /// Default constructor. Initialise the game board (all square to 0),
        /// the first player turn and the turn left (the maximum turn for this game).
        /// </summary>
        public TicTacToeCore()
        {
            board = new TicTacToeBoard();
            turn = -1;
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


        public bool End()
        {
            if (board.GetTurnLeft() <= 0)
            {
                return true;
            }
            return board.Ended();
        }

    }
}
