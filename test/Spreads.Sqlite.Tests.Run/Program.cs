using System;
using Spreads.SQLite.Tests.Fast;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new FastTest(null);
            test.FastSimpleTest();
        }
    }
}
