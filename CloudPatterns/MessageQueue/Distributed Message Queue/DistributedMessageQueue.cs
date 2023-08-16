using CloudPatterns.MessageQueue.Distributed_Message_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue
{
    public class DistributedMessageQueue
    {
        private MetadataService metadataService = new MetadataService();
        private Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        public void CreateQueue(string queueName)
        {
            Node node = new Node();
            nodes[queueName] = node;
            metadataService.CreateQueue(queueName, node);
        }
        public Node GetNode(string queueName)
        {
            return nodes.ContainsKey(queueName) ? nodes[queueName] : null;
        }

        public void SendToQueue(string queueName, string message)
        {
            if (nodes.ContainsKey(queueName))
            {
                nodes[queueName].MessageQueue.Add(message);
            }
        }
    }
}
