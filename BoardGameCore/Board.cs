using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameCore
{
    /// <summary>
    /// This class implements a board representation for a two-player
    /// boardgame.
    /// </summary>
    public class Board
    {
        // Board squares.
        protected int[] board;
        // A flag to check the game status (ongoing or ended).
        protected bool gameOver = false;

        /// <summary>
        /// This method copy the current board status to the given
        /// array.
        /// </summary>
        /// <param name="dest">The reference for the output array.</param>
        public void BoardToArray(ref int[] dest)
        {
            dest = new int[board.Length];
            for (int i = 0; i < board.Length; i++)
            {
                dest[i] = board[i];
            }
        }

    }
}
