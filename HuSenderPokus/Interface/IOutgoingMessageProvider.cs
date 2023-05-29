using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    internal interface IOutgoingMessageProvider : IPartitionFilter, IPartitionRemovalChecker
    {
        bool HasMessagesInFlight { get; }
        bool QueueFull { get; }

        public void Enqueue(OutgoingMessageModel item);
        public OutgoingMessageModel Dequeue();
        public OutgoingMessageModel GetMessageInFlight(string CRMOBEID);
        public void Commit(string CRMOBEID);  
        public void Retry(string CRMOBEID);


    }
}
