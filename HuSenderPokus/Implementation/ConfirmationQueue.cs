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
        bool _isComplete = false; 


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

        public bool IsComplete => _isComplete;

        public void MarkComplete()
        {
           _isComplete = true;
        }

        public bool AllowPartitionRemoval(string partitionKey)
        {
            // pokud se dodrží pořadí volání jednotlivých AllowPartitionRemoval v Dispatcheru, neměl by být potřeba lock
            // pokud ne, tak asi ani lock nepomůže  
            return !_queue.Any(x=>x.PartitinoKey == partitionKey);
        }
    }
}
