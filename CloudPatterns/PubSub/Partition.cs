using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPatterns.PubSub
{
    public class Partition
    {
        public int PartitionId { get; set; }
        public string TopicName { get; set; }
        public int LeaderBrokerId { get; set; }
        public List<int> ReplicaBrokerIds { get; } = new List<int>();
    }

}
