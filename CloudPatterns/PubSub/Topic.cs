using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.PubSub
{
    public class Topic
    {
        public string TopicName { get; set; }
        public int Partitions { get; set; }
        public int ReplicationFactor { get; set; }
        public Dictionary<int, Partition> PartitionsInfo { get; } = new Dictionary<int, Partition>();
    }
}
