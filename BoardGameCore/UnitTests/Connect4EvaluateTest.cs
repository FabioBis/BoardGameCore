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
using NUnit.Framework;

namespace BoardGameCore.UnitTests
{
	[TestFixture]
	public class Connect4EvaluateTest
	{
		[TestCase]
		public void TestMethod()
		{
            int[] moves = null;
            int value;

			moves = new int[7] {0, 1, 1, 2, 2, 3, 3};
			Connect4Board board = new Connect4Board(moves);
            value = board.Evaluate(-1);			
			Assert.AreEqual(-1, value);
            value = board.Evaluate(1);
            Assert.AreNotEqual(-1, value);
		}
	}
}
