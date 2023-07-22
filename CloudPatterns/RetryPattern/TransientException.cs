using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.RetryPattern
{
    public class TransientException : Exception
    {
        public TransientException(string message) : base(message)
        {
        }
    }
}
