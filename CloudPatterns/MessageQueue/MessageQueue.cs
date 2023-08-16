using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue
{
    public class MessageQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly object queueLock = new object();

        public void Enqueue(T message)
        {
            lock (queueLock)
            {
                queue.Enqueue(message);
                Monitor.Pulse(queueLock); // Notify waiting consumers
            }
        }

        public T Dequeue()
        {
            lock (queueLock)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queueLock); // Wait until there's a message to consume
                }

                return queue.Dequeue();
            }
        }
    }
}
