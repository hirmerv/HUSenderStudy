using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    public interface IPartitionFilter
    {
        void SetPartitions(IEnumerable<string> partitions);
    }
}
