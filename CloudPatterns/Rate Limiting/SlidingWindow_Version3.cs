using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Rate_Limiting
{
    public class SlidingWindow_Version3
    {
        private int capacity;
        private int timeUnit;
        private Action<object> forwardCallback;
        private Action<object> dropCallback;

        private DateTime curTime;
        private int totalCapacity;
        private int curCount;

        public SlidingWindow_Version3(int capacity, int timeUnit, Action<object> forwardCallback, Action<object> dropCallback)
        {
            this.capacity = capacity;
            this.timeUnit = timeUnit;
            this.forwardCallback = forwardCallback;
            this.dropCallback = dropCallback;

            this.curTime = DateTime.Now;
            this.totalCapacity = capacity;
            this.curCount = 0;
        }

        public void Handle(object packet)
        {
            if ((DateTime.Now - curTime).TotalMilliseconds > 1000)
            {
                Console.WriteLine("1 second is complete now we are sliding the window");
                curTime = DateTime.Now;
                totalCapacity = curCount;
                curCount = 0;
            }
            Console.WriteLine($"Current totalCapacity : {totalCapacity}");
            var totalSeconds = (DateTime.Now - curTime).TotalSeconds;

            var currentTimeUnit = (double)timeUnit - (double)totalSeconds;
            var allowedRequest = currentTimeUnit / timeUnit;
            double ec = (totalCapacity * allowedRequest) + curCount;
            if (ec > capacity)
            {
                dropCallback(packet);
                return;
            }

            curCount++;
            forwardCallback(packet);
        }
    }

}
