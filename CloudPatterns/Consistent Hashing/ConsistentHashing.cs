using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Consistent_Hashing
{
    public class ConsistentHashing
    {
        private int replicas;
        private SortedDictionary<long, string> nodes;
        private Dictionary<string, Dictionary<string, string>> dataStorage; // In-memory data storage

        public Dictionary<string, Dictionary<string, string>> Storage
        {
            get
            {
                return this.dataStorage;
            }
        }

        public SortedDictionary<long, string> Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        public ConsistentHashing(IEnumerable<string> nodes, int replicas = 3)
        {
            this.replicas = replicas;
            this.nodes = new SortedDictionary<long, string>();
            this.dataStorage = new Dictionary<string, Dictionary<string, string>>();
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }

        public void AddNode(string node)
        {
            for (int i = 0; i < replicas; i++)
            {
                string replica = GetReplicaName(node, i);
                long hash = GetHash(replica);

                nodes[hash] = node;
                dataStorage[replica] = new Dictionary<string, string>();

            }
        }       

        // Remove virtual nodes (replicas) for a failed node
        public void RemoveNode(string node)
        {
            for (int i = 0; i < replicas; i++)
            {
                long hash = GetHash($"{node}_{i}");
                nodes.Remove(hash);

                // Remove data storage for the replica
                dataStorage.Remove($"{node}_{i}");
            }
        }

        public string GetNode(string key)
        {
            long hash = GetHash(key);
            var node = nodes.ContainsKey(hash)
                ? nodes[hash]
                : nodes.ContainsKey(nodes.Keys.FirstOrDefault(k => k >= hash))
                    ? nodes[nodes.Keys.FirstOrDefault(k => k >= hash)]
                    : nodes.Values.FirstOrDefault();
            return node;
        }

        private string GetReplicaName(string server, int index)
        {
            return $"{server}_{index}";
        }

        public string GetCorrectReplica(string server, string key)
        {
            long hash = GetHash(key);
            var candidateNodes = nodes.Keys;

            long maxHash = long.MinValue;
            string selectedNode = null;

            for (int i = 0; i < replicas; i++)
            {
                // Combine server name and replica index to get a unique hash for each replica
                long nodeHash = GetHash($"{server}_{i}"); 
                if (nodeHash > maxHash)
                {
                    maxHash = nodeHash;
                    selectedNode = $"{server}_{i}";
                }
            }

            // If no node is found, wrap around to the first node in the ring
            return selectedNode ?? $"{server}_0";
        }

        private long GetHash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToInt64(hashBytes, 0);
            }
        }
    }
}
