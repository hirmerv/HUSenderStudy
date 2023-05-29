using HuSenderPokus.Interface;
using HuSenderPokus.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Implementation
{
    public class ConfirmationQueue : IConfirmationQueue
    {
        ConcurrentQueue<OutgoingMessageModel> _queue = new();
        public void Enqueue(OutgoingMessageModel message)
        {
            _queue.Enqueue(message);
        }

        public OutgoingMessageModel? Dequeue()
        {
            OutgoingMessageModel message;
            var ret = _queue.TryDequeue(out message);
            return ret ? message : null;
        }

     }
}
