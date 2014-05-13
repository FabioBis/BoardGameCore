using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameCore
{
    public class TicTacToeSimpleStrategy2 : TicTacToeSimpleStrategy
    {

        public TicTacToeSimpleStrategy2(TicTacToeBoard board, int turn)
            : base(board, turn)
        {
        }

        public override int OwnMove()
        {
            int result = -1;
            board.BoardToArray(ref boardArray);
            List<int> rows = new List<int>() {
                boardArray[0] + boardArray[1] + boardArray[2],
                boardArray[3] + boardArray[4] + boardArray[5],
                boardArray[6] + boardArray[7] + boardArray[8],
                boardArray[0] + boardArray[3] + boardArray[6],
                boardArray[1] + boardArray[4] + boardArray[7],
                boardArray[2] + boardArray[5] + boardArray[8],
                boardArray[0] + boardArray[4] + boardArray[8],
                boardArray[2] + boardArray[4] + boardArray[6]
            };

            int i = 0;
            // Order matter: first try to win, than avoid to lose.
            if (aiTurn > 0)
            {
                foreach (int row in rows)
                {
                    if (row == 2)
                    {
                        // Do not loose.
                        result = selectFreeSquare(i);
                    }
                    else if (row == -2)
                    {
                        // Win.
                        result = selectFreeSquare(i);
                    }
                    i++;
                }
            }
            else
            {
                foreach (int row in rows)
                {
                    if (row == -2)
                    {
                        // Do not loose.
                        result = selectFreeSquare(i);
                    }
                    else if (row == 2)
                    {
                        // Win.
                        result = selectFreeSquare(i);
                    }
                    i++;
                }
            }
            if (result < 0)
            {
                Random random = new Random();
                int index = random.Next(0, board.GetTurnLeft());
                result = board.freeSquare.ElementAt(index);
            }
            board.Move(result, aiTurn);
            return result;
        }
    }
}
