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
