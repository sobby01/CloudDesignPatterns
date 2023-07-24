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

        private double GetPreviousWindowWeight(DateTime currentTime)
        {
            TimeSpan timePassedInSecond = currentTime.TimeOfDay;
            double weight = (1000000 - timePassedInSecond.TotalMilliseconds * 1000) / 1000000;
            weight = Math.Max(weight, 0); // Ensure weight is not negative
            weight = Math.Min(weight, 1); // Ensure weight is not greater than 1
            return weight;
        }

        public bool TryAcquire()
        {
            lock (windowCounts)
            {
                // Calculate the current window key and weight
                var now = DateTime.Now;
                string currentWindowKey = GetWindowKey(now);//222822

                var currentDt = now - now.Date;
                var currentMS = currentDt.TotalMilliseconds;
                var previousWindowStart = now.AddMilliseconds(-windowMilliSeconds);
                previousWindowKey = GetWindowKey(previousWindowStart);
                
                TimeSpan timePassedInSecond = now.TimeOfDay;
                var previousWindowWeight = (1000000 - timePassedInSecond.TotalMilliseconds * 1000) / 1000000;
                previousWindowWeight = Math.Max(previousWindowWeight, 0); // Ensure weight is not negative
                previousWindowWeight = Math.Min(previousWindowWeight, 1); // Ensure weight is not greater than 1


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
