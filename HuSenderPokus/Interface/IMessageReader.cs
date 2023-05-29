using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    public interface IMessageReader : IPartitionFilter
    {
        IEnumerable<OutgoingMessageModel> ReadMessages();

        // TODO: SuspendedOBEs
        IEnumerable<OutgoingMessageModel> ReadMessagesForObe(long idFrom);

    }
}
