using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CloudPatterns.RetryPattern
{
    public class TransientException : Exception
    {
        //A Transient Exception, also known as a transient fault, 
        //is a type of error that occurs in distributed systems and cloud-based applications.
        //It is usually temporary and intermittent in nature, meaning it appears and disappears over time.
        public TransientException(string message) : base(message)
        {
        }
    }
}
