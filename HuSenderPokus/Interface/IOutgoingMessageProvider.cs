using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    internal interface IOutgoingMessageProvider : IPartitionRemovalChecker
    {
        bool HasMessagesInFlight { get; }
        bool QueueFull { get; }

        public void Enqueue(OutgoingMessageModel item);
        public OutgoingMessageModel Dequeue();
        public OutgoingMessageModel Commit(string CRMOBEID);  // TODO: mrknout, jak m8 Radek -> parametr
        public void Retry(string CRMOBEID);


    }
}
