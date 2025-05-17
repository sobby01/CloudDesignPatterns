using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.PubSub
{
    public class Broker
    {
        public int BrokerId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsLeader { get; set; }
    }
}
