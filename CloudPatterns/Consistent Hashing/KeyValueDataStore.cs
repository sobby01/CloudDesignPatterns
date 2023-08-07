using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Consistent_Hashing
{
    public class KeyValueDataStore
    {
        ConsistentHashing CHashing;

        public void Initialize(IEnumerable<string> nodes, int replicas = 3)
        {
            CHashing = new ConsistentHashing(nodes, replicas);
            //SimiluteNodeFialure();
        }

        private void SimiluteNodeFialure()
        {
            Timer nodeOperationTimer = new Timer(state =>
            {
                // Simulate node failure (adjust the logic for failure simulation as needed)
                string failedNode = SimulateNodeFailure();
                Console.WriteLine($"Simulated Node Failure: {failedNode}");

                // Simulate adding a new node (adjust the logic for node addition as needed)
                string newNode = SimulateNewNodeAddition();
                Console.WriteLine($"Simulated New Node Addition: {newNode}");
            }, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(100)); // Interval: 10 seconds (initial delay), then every 60 seconds
        }

        public void StoreData(string key, string data)
        {
            string server = CHashing.GetNode(key);
            string replica = CHashing.GetCorrectReplica(server, key);

            CHashing.Storage[replica][key] = data;
            Console.WriteLine($"Storing Key: {key} => Data: {data} on Server Replica: {server}");
        }

        public string FetchData(string key)
        {
            string server = CHashing.GetNode(key);
            string replica = CHashing.GetCorrectReplica(server, key);
            //string server = CHashing.GetNode(key);
            //string replica = CHashing.GetCorrectReplica(server, key);

            if (CHashing.Storage.ContainsKey(server) && CHashing.Storage[server].ContainsKey(key))
            {
                string data = CHashing.Storage[server][key];
                Console.WriteLine($"Fetching Data for Key: {key} from Server Replica: {server}");
                return data;
            }
            else
            {
                Console.WriteLine($"Data for Key: {key} not found in Server: {server}");
                return null;
            }
        }

        private string SimulateNodeFailure()
        {
            Random random = new Random();
            List<string> nodesList = new List<string>(CHashing.Nodes.Values);
            string failedNode = nodesList[random.Next(nodesList.Count)];

            CHashing.RemoveNode(failedNode);

            return failedNode;
        }

        private string SimulateNewNodeAddition()
        {
            //We can randomize algorithm to get server details
            CHashing.AddNode("Server-D");
            return "Server-D";
        }
    }
}
