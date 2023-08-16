using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue
{
    public class MessageQueueHandler
    {
        MessageQueue<string> messageQueue = new MessageQueue<string>();

        public void Producer()
        {
            for (int i = 0; i < 10; i++)
            {
                string message = $"Message {i}";
                Console.WriteLine($"Producing: {message}");
                messageQueue.Enqueue(message);
                Thread.Sleep(100); // Simulate some processing time
            }
        }

        public void Consumer()
        {
            for (int i = 0; i < 10; i++)
            {
                string message = messageQueue.Dequeue();
                Console.WriteLine($"Consuming: {message}");
                Thread.Sleep(200); // Simulate some processing time
            }
        }

        public void Test()
        {
            Thread producerThread = new Thread(Producer);
            Thread consumerThread = new Thread(Consumer);

            producerThread.Start();
            consumerThread.Start();

            producerThread.Join();
            consumerThread.Join();

            Console.WriteLine("Finished");
        }
    }
}
