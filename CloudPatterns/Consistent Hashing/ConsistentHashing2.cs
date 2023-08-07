using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.Consistent_Hashing
{
    public class ConsistentHashing2
    {
        private List<uint> ringKeys = new List<uint>();
        private List<string> nodes = new List<string>(); // nodes present in the ring. nodes[i] is present at index keys[i]
        private int totalSlots = 50;
        private int replicas = 3; // number of replicas per server
        private Dictionary<string, Dictionary<string, string>> dataStorage;

         public ConsistentHashing2(int replicas = 3)
        {
            this.replicas = replicas;
            this.dataStorage = new Dictionary<string, Dictionary<string, string>>();
        }

        public void AddNode(string node)
        {
            if (ringKeys.Count == totalSlots)
            {
                throw new Exception("Hash space is full");
            }

            for (int i = 0; i < replicas; i++)
            {
                string nodeReplica = $"{node}_{i}";
                
                uint key = GetHash(nodeReplica, totalSlots);

                int index = FindIndexToInsertKey(key);
                if (index > 0 && ringKeys[index - 1] == key)
                {
                    throw new Exception("Collision occurred");
                }

                nodes.Insert(index, nodeReplica);
                ringKeys.Insert(index, key);
                dataStorage[nodeReplica] = new Dictionary<string, string>();
            }
        }

        public void RemoveNode(string node)
        {
            if (ringKeys.Count == 0)
            {
                throw new Exception("Hash space is empty");
            }

            for (int i = 0; i < replicas; i++)
            {
                string replica = $"{node}_{i}";
                uint key = GetHash(replica, totalSlots);

                int index = ringKeys.BinarySearch(key);
                if (index < 0 || ringKeys[index] != key)
                {
                    throw new Exception("Node does not exist");
                }

                ringKeys.RemoveAt(index);
                nodes.RemoveAt(index);
                // Remove data storage for the replica
                dataStorage.Remove(replica);
            }
        }

        public string Store(string item, string data)
        {
            uint key = GetHash(item, totalSlots);

            int index = ringKeys.BinarySearch(key);
            if (index < 0)
            {
                // Find the next right node
                index = ~index % ringKeys.Count;
            }
            dataStorage[nodes[index]][item] = data;
            return nodes[index];
        }

        public string FetchData(string item)
        {
            uint key = GetHash(item, totalSlots);
            int index = ringKeys.BinarySearch(key);
            if (index < 0)
            {
                // Find the next right node
                index = ~index % ringKeys.Count;
            }
            string server = nodes[index];


            if (dataStorage.ContainsKey(server) && dataStorage[server].ContainsKey(item))
            {
                string data = dataStorage[server][item];
                Console.WriteLine($"Fetching Data for Key: {key} from Server Replica: {server}");
                return data;
            }
            else
            {
                Console.WriteLine($"Data for Key: {item} not found in Server: {server}");
                return null;
            }
        }

        private uint GetHash(string key, int totalSlots)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(key);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return BitConverter.ToUInt32(hashBytes, 0) % (uint)totalSlots;
            }
        }

        private int FindIndexToInsertKey(uint key)
        {
            int index = ringKeys.BinarySearch(key);
            if (index < 0)
            {
                //bitwise NOT operation
                index = ~index;
            }
            return index;
        }
    }
}
