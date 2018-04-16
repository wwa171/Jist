using System;

// https://blogs.msdn.microsoft.com/pfxteam/2009/02/19/getting-random-numbers-in-a-thread-safe-way/

namespace Jist.Next.Plugin
{
    public class ThreadSafeRandom
    {
        private static readonly Random _global = new Random();

        [ThreadStatic]
        private static Random _local;

        public ThreadSafeRandom()
        {
            threadInit();
        }

        private void threadInit()
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }
        }

        public int Next()
        {
            threadInit();
            return _local.Next();
        }

        public int Next(int from, int to)
        {
            threadInit();
            return _local.Next(from, to);
        }

        public int NextInclusive(int from, int to)
        {
            threadInit();
            return _local.Next(from, to + 1);
        }
    }
}