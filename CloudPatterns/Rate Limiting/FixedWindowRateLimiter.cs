using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Rate_Limiting
{
    public class FixedWindowRateLimiter
    {
        private readonly int limit;
        private readonly int windowMilliSeconds;
        private readonly Queue<DateTime> requestTimes;
        public readonly Stopwatch stopwatch;

        public FixedWindowRateLimiter(int limit, int windowMilliSeconds)
        {
            this.limit = limit;
            this.windowMilliSeconds = windowMilliSeconds;
            this.requestTimes = new Queue<DateTime>();
            this.stopwatch = new Stopwatch();
        }

        public bool TryAcquire()
        {
            lock (requestTimes)
            {
                var now = DateTime.Now;

                // Remove expired requests from the queue
                while (requestTimes.Count > 0 && (now - requestTimes.Peek()).TotalMilliseconds >= windowMilliSeconds)
                {
                    requestTimes.Dequeue();
                }

                if (requestTimes.Count < limit)
                {
                    requestTimes.Enqueue(now);
                    return true; // Acquired a permit
                }

                return false; // Rate limit exceeded
            }
        }

        public int GetTotalRequestsInWindow()
        {
            lock (requestTimes)
            {
                return requestTimes.Count;
            }
        }
    }
}
