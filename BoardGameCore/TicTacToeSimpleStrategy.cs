using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameCore
{
    public class TicTacToeSimpleStrategy : TicTacToeStrategy
    {
        protected TicTacToeBoard board;
        protected int aiTurn;
        protected int[] boardArray = null;


        public TicTacToeSimpleStrategy(TicTacToeBoard board, int turn)
        {
            this.board = new TicTacToeBoard(board);
            aiTurn = turn;
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
            // Order matter: first avoid to lose, than try to win.
            if (aiTurn > 0)
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
            else
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
            if (result < 0)
            {
                Random random = new Random();
                int index = random.Next(0, board.GetTurnLeft());
                result = board.freeSquare.ElementAt(index);
            }
            board.Move(result, aiTurn);
            return result;
        }


        protected int selectFreeSquare(int p1, int p2, int p3)
        {
            if (boardArray[p1] == 0)
            {
                return p1;
            }
            else if (boardArray[p2] == 0)
            {
                return p2;
            }
            else
            {
                return p3;
            }
        }


        protected int selectFreeSquare(int i)
        {
            int result = -1;
            switch (i)
            {
                case 0:
                    result = selectFreeSquare(0, 1, 2);
                    break;
                case 1:
                    result = selectFreeSquare(3, 4, 5);
                    break;
                case 2:
                    result = selectFreeSquare(6, 7, 8);
                    break;
                case 3:
                    result = selectFreeSquare(0, 3, 6);
                    break;
                case 4:
                    result = selectFreeSquare(1, 4, 7);
                    break;
                case 5:
                    result = selectFreeSquare(2, 5, 8);
                    break;
                case 6:
                    result = selectFreeSquare(0, 4, 8);
                    break;
                case 7:
                    result = selectFreeSquare(2, 4, 6);
                    break;
                default:
                    break;
            }
            return result;
        }


        public override void OpponentMove(int square)
        {
            board.Move(square, -1*aiTurn);
        }


        public override void Reset()
        {
            this.board = new TicTacToeBoard(new TicTacToeBoard()); 
        }

    }
}
