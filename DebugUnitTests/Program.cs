using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameCore.UnitTests;

namespace DebugUnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Connect4EvaluateTest test = new Connect4EvaluateTest();
            test.TestMethod();
        }
    }
}
