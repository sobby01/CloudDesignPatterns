using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue.Distributed_Message_Queue
{
    public class Consumer
    {
        public async Task ConsumeAsync(BlockingCollection<string> queue)
        {
            await Task.Run(() =>
            {
                foreach (var message in queue.GetConsumingEnumerable())
                {
                    Console.WriteLine($"Consumer received message: {message}");
                }
            });
        }

        public async void Test()
        {
            DistributedMessageQueue frontend = new DistributedMessageQueue();

            Producer producer = new Producer();
            await producer.ProduceAsync(frontend, "Send Message", "Hello from Producer", 5);

            frontend.CreateQueue("Send Message");

            var consumerTask = ConsumeAsync(frontend.GetNode("Send Message").MessageQueue);

            // Simulate sending messages
            frontend.SendToQueue("Send Message", "Hello from Send Message queue!");
            frontend.SendToQueue("Send Message", "Another message from Send Message queue!");

            await Task.WhenAll(consumerTask);
        }
    }
}
