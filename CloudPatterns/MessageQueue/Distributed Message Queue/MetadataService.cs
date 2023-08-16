using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.MessageQueue.Distributed_Message_Queue
{
    public class MetadataService
    {
        private Dictionary<string, Node> queueAssignments = new Dictionary<string, Node>();

        public void CreateQueue(string queueName, Node node)
        {
            if (!queueAssignments.ContainsKey(queueName))
            {
                queueAssignments[queueName] = node;
            }
        }

        public Node GetQueueOwner(string queueName)
        {
            if (queueAssignments.ContainsKey(queueName))
            {
                return queueAssignments[queueName];
            }
            return null;
        }
    }
}
