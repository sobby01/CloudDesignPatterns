using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.RetryPattern
{
    public class MaxRetryAttemptsReachedException : Exception
    {
        public MaxRetryAttemptsReachedException() : base("Maximum retry attempts reached.") { }
    }
}
