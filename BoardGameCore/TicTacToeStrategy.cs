using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameCore
{
    abstract public class TicTacToeStrategy
    {
        abstract public int OwnMove();
        abstract public void OpponentMove(int square);
        abstract public void Reset();
    }
}
