using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    public interface IMessagesInProgress : IPartitionRemovalChecker
    {
        bool TryDequeue(string obeId, out OutgoingMessageModel ret);
        OutgoingMessageModel Commit(string obeId);

        void Retry(string obeId);
        void RegisterMessage(OutgoingMessageModel message);

        bool HasMessagesInFlight { get; }

        OutgoingMessageModel GetMessageInFlight(string cRMOBEID);
    }
}
