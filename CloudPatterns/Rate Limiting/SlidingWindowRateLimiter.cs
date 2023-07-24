using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Rate_Limiting
{
    public class SlidingWindowRateLimiter
    {
        private readonly int capacity;
        private readonly TimeSpan windowDuration;
        private readonly Queue<DateTime> requestTimes;

        public SlidingWindowRateLimiter(int capacity, TimeSpan windowDuration)
        {
            this.capacity = capacity;
            this.windowDuration = windowDuration;
            this.requestTimes = new Queue<DateTime>();
        }

        public bool TryAcquire()
        {
            var now = DateTime.Now;

            // Remove old request times that are outside the current window
            while (requestTimes.Count > 0 && now - requestTimes.Peek() >= windowDuration)
            {
                requestTimes.Dequeue();
            }

            // Check if the current request can be allowed
            if (requestTimes.Count < capacity)
            {
                requestTimes.Enqueue(now);
                Console.WriteLine($"Acquired : {now:yyyy-MM-dd HH:mm:ss.fff}");
                return true;
            }
            else
            {
                Console.WriteLine($"Rejected : {now:yyyy-MM-dd HH:mm:ss.fff}");
                return false;
            }

            //return false; // Rate limit exceeded
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
