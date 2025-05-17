using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.PubSub
{
    using System;
    using System.Collections.Generic;

    public class ClusterManager
    {
        private Dictionary<int, Broker> brokers = new Dictionary<int, Broker>();
        private Dictionary<string, Topic> topics = new Dictionary<string, Topic>();

        public void AddBroker(int brokerId, string host, int port)
        {
            brokers[brokerId] = new Broker { BrokerId = brokerId, Host = host, Port = port };
        }

        public void AddTopic(string topicName, int partitions, int replicationFactor)
        {
            var topic = new Topic { TopicName = topicName, Partitions = partitions, ReplicationFactor = replicationFactor };
            topics[topicName] = topic;

            for (int i = 0; i < partitions; i++)
            {
                var partition = new Partition { PartitionId = i, TopicName = topicName };
                topic.PartitionsInfo[i] = partition;
            }
        }

        public void AssignPartitionsToBrokers()
        {
            int brokerIndex = 0;
            foreach (var topic in topics.Values)
            {
                foreach (var partition in topic.PartitionsInfo.Values)
                {
                    // Assign leader and replicas
                    partition.LeaderBrokerId = brokerIndex;
                    for (int i = 1; i <= topic.ReplicationFactor - 1; i++)
                    {
                        partition.ReplicaBrokerIds.Add((brokerIndex + i) % brokers.Count);
                    }

                    brokerIndex = (brokerIndex + 1) % brokers.Count; // Move to the next broker
                }
            }
        }

        public void HandleBrokerFailure(int failedBrokerId)
        {
            // Handle broker failure by reassigning leadership and replicas.
        }

        public void Start()
        {
            // Start the cluster manager and its services.
        }

        public void PrintClusterState()
        {
            Console.WriteLine("Brokers:");
            foreach (var broker in brokers.Values)
            {
                Console.WriteLine($"Broker {broker.BrokerId} - Host: {broker.Host}, Port: {broker.Port}, Leader: {broker.IsLeader}");
            }

            Console.WriteLine("\nTopics:");
            foreach (var topic in topics.Values)
            {
                Console.WriteLine($"Topic {topic.TopicName} - Partitions: {topic.Partitions}, Replication: {topic.ReplicationFactor}");
                foreach (var partition in topic.PartitionsInfo.Values)
                {
                    Console.WriteLine($"Partition {partition.PartitionId} - Leader: {partition.LeaderBrokerId}, Replicas: {string.Join(", ", partition.ReplicaBrokerIds)}");
                }
            }
        }
    }
}
