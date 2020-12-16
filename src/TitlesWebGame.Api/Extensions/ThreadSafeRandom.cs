using System;
using System.Threading;

namespace TitlesWebGame.Api.Extensions
{
    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;
        public static Random ThisThreadsRandom
        {
            get { return Local ??= new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)); }
        }
    }
}