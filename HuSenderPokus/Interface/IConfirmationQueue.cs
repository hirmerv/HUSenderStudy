using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Interface
{
    public interface IConfirmationQueue : IPartitionRemovalChecker
    {
        void Enqueue(OutgoingMessageModel message);
        OutgoingMessageModel Dequeue();

        // mark end of work
        void MarkComplete();

        bool IsComplete { get; }

    }
}
