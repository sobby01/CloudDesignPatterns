using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.PubSub
{
    public class ClusterManagerHandler
    {
        public void Test()
        {
            ClusterManager clusterManager = new ClusterManager();

            // Adding brokers
            clusterManager.AddBroker(1, "broker1.example.com", 9092);
            clusterManager.AddBroker(2, "broker2.example.com", 9092);

            // Adding topics
            clusterManager.AddTopic("TopicA", partitions: 3, replicationFactor: 2);
            clusterManager.AddTopic("TopicB", partitions: 5, replicationFactor: 3);

            // Printing cluster state
            clusterManager.PrintClusterState();
        }        
    }
}
