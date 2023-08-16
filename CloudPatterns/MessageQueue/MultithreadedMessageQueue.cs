using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue
{
    public class MultiThreadedMessageQueue<T>
    {
        private BlockingCollection<T> queue =  new BlockingCollection<T> ();

        public void Enqueue(T message)
        {
            queue.Add(message);
        }

        public T Dequeue()
        {
            return queue.Take();
        }
    }
}
