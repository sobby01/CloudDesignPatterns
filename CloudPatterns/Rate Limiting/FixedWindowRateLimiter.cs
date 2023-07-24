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
        private readonly int capacity;
        private readonly TimeSpan windowDuration;
        private DateTime windowStartTime;
        private int requestCount;

        public FixedWindowRateLimiter(int capacity, TimeSpan windowDuration)
        {
            this.capacity = capacity;
            this.windowDuration = windowDuration;
            this.requestCount = 0;
            this.windowStartTime = DateTime.Now;
        }

        public bool TryAcquire()
        {
            var now = DateTime.Now;

            // Check if the current window has expired
            if (now - windowStartTime >= windowDuration)
            {
                // Reset the request count and update the window start time
                requestCount = 0;
                windowStartTime = now;
            }

            // Check if the current request can be allowed
            if (requestCount < capacity)
            {
                requestCount++;
                return true; // Acquired a permit
            }

            return false; // Rate limit exceeded
        }
    }
}
