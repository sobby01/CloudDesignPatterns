using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Rate_Limiting
{
    public class SlidingWindowRateLimiter
    {
        private readonly int limit;
        private readonly int windowMilliSeconds;
        private readonly LinkedList<DateTime> requestTimes;

        public SlidingWindowRateLimiter(int limit, int windowMilliSeconds)
        {
            this.limit = limit;
            this.windowMilliSeconds = windowMilliSeconds;
            //Maintain a date time of current request
            this.requestTimes = new LinkedList<DateTime>();
        }

        public bool TryAcquire()
        {
            lock (requestTimes)
            {
                var now = DateTime.Now;

                // Remove expired requests from the linked list
                while (requestTimes.Count > 0 && (now - requestTimes.First.Value).TotalMilliseconds >= windowMilliSeconds)
                {
                    requestTimes.RemoveFirst();
                }

                if (requestTimes.Count < limit)
                {
                    requestTimes.AddLast(now);
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
