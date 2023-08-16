using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue.Distributed_Message_Queue
{
    public class Producer
    {
        public async Task ProduceAsync(DistributedMessageQueue frontend, string queueName, string message, int messageCount)
        {
            // Create the queue if it doesn't exist
            if (!frontend.GetNode(queueName).MessageQueue.IsAddingCompleted)
            {
                frontend.CreateQueue(queueName);
            }

            // Send messages to the queue
            for (int i = 0; i < messageCount; i++)
            {
                frontend.SendToQueue(queueName, $"{message} {i}");
                await Task.Delay(100); // Simulate message production delay
            }
        }

        public async void Test()
        {
            DistributedMessageQueue frontend = new DistributedMessageQueue();

            Producer producer = new Producer();
            await producer.ProduceAsync(frontend, "Send Message", "Hello from Producer", 5);
        }
    }
}
