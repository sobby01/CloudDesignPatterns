using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue.Distributed_Message_Queue
{
    public class Node
    {
        public BlockingCollection<string> MessageQueue { get; } = new BlockingCollection<string>(boundedCapacity: 10);
    }
}
