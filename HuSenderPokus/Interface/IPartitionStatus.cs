using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    public interface IPartitionStatus
    {
        // returns whether there is change in partition list since the last call
        bool CheckUpdates();

        string[] ActivePartitions { get; }
        string[] PartitionWaitingForRemoval { get; }

        void ConfirmPartitionRemoval(string partitionKey);
    }
}
