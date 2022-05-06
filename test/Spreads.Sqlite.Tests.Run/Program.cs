using Spreads.SQLite.Tests.Fast;

namespace Spreads.SQLite.Tests.Run
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var test = new FastTest();
            // test.FastBenchShortLiveConnection();
            // test.FastBench();
            test.FastBenchDelegates();
        }
    }
}
