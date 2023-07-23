using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Rate_Limiting
{
    public class SlidingWindowRateLimiter_Version2
    {
        private readonly int limit;
        private readonly int windowMilliSeconds;
        private readonly LinkedList<DateTime> requestTimes;
        private readonly Dictionary<string, int> windowCounts;
        private readonly Stopwatch stopwatch;
        private string previousWindowKey;
        private double previousWindowWeight;

        public SlidingWindowRateLimiter_Version2(int limit, int windowMilliSeconds)
        {
            this.limit = limit;
            this.windowMilliSeconds = windowMilliSeconds;
            this.windowCounts = new Dictionary<string, int>();
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
            this.previousWindowKey = GetWindowKey(DateTime.Now);
            this.previousWindowWeight = 1.0;
            //Maintain a date time of current request
            this.requestTimes = new LinkedList<DateTime>();
        }

        private string GetWindowKey(DateTime time)
        {
            return $"{time:HHmmss}";
        }

        public bool TryAcquire()
        {
            lock (windowCounts)
            {
                // Calculate the current window key and weight
                var now = DateTime.Now;
                string currentWindowKey = GetWindowKey(now);
                double currentWindowWeight = (now - now.Date).TotalMilliseconds / windowMilliSeconds;

                // Check if the current window has expired and remove expired windows
                var windowStart = now.AddMilliseconds(-windowMilliSeconds);
                var expiredWindowKeys = new List<string>();
                foreach (var key in windowCounts.Keys)
                {
                    Console.WriteLine(key == previousWindowKey);
                    if (key != currentWindowKey && key != previousWindowKey)
                    {
                        if (DateTime.TryParseExact(key, "HHmmss", null, System.Globalization.DateTimeStyles.None, out var windowTime))
                        {
                            if (windowTime < windowStart)
                                expiredWindowKeys.Add(key);
                        }
                    }
                }
                foreach (var key in expiredWindowKeys)
                {
                    windowCounts.Remove(key);
                }

                // Check if a new window has started and update the previous window key and weight
                if (currentWindowKey != previousWindowKey)
                {
                    previousWindowKey = currentWindowKey;
                    previousWindowWeight = currentWindowWeight;
                }

                // Adjust the window counts based on the sliding window
                int currentWindowCount = windowCounts.TryGetValue(currentWindowKey, out int count) ? count : 0;
                int previousWindowCount = windowCounts.TryGetValue(previousWindowKey, out count) ? count : 0;
                int totalCount = (int)Math.Floor(previousWindowCount * previousWindowWeight + currentWindowCount);

                if (totalCount < limit)
                {
                    // Increment the count for the current window
                    if (windowCounts.ContainsKey(currentWindowKey))
                        windowCounts[currentWindowKey]++;
                    else
                        windowCounts[currentWindowKey] = 1;

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
