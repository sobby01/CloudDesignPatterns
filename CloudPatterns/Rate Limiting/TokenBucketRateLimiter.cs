using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Rate_Limiting
{
    public class TokenBucketRateLimiter
    {
        private readonly int capacity;
        private readonly int refillTokens;
        private int tokens;
        private readonly object lockObject = new object();

        public TokenBucketRateLimiter(int capacity, int refillTokens, int refillIntervalSeconds)
        {
            this.capacity = capacity;
            this.refillTokens = refillTokens;
            this.tokens = capacity;
            Task.Run(() => RefillTokens(refillIntervalSeconds));
        }

        private void RefillTokens(int refillIntervalSeconds)
        {
            while (true)
            {
                Thread.Sleep(refillIntervalSeconds * 1000);
                lock (lockObject)
                {
                    tokens = Math.Min(capacity, tokens + refillTokens);
                }
            }
        }

        public bool TryAcquire()
        {
            lock (lockObject)
            {
                if (tokens > 0)
                {
                    tokens--;
                    return true; // Acquired a permit
                }

                return false; // Rate limit exceeded
            }
        }
    }
}
