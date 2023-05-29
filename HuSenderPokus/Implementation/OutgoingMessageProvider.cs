using HuSenderPokus.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Model
{
    internal class OutgoingMessageProvider : IOutgoingMessageProvider
    {

        const int MaxQueueSize = 10000;  // configurable ?

        // fronta OutgoingMessages načtených z DB
        ConcurrentQueue<OutgoingMessageModel> _outgoingMessageQueue;
        ConcurrentQueue<string> _processedObeQueue;
        IMessagesInProgress _messagesInProgress;

        public bool HasMessagesInFlight => _messagesInProgress.HasMessagesInFlight;

        public bool QueueFull => _outgoingMessageQueue.Count > MaxQueueSize;

        public void Enqueue(OutgoingMessageModel item)
        {
            _outgoingMessageQueue.Enqueue(item);
        }


        public OutgoingMessageModel? Dequeue()
        {
            OutgoingMessageModel message;
            while (true)
            {
                string processedObe;
                if (!_processedObeQueue.TryDequeue(out processedObe))
                    // no response to process, continue with normal processing
                    break;  
                if (_messagesInProgress.TryDequeue(processedObe, out message))
                {
                    return message;
                }
            }
            _ = _outgoingMessageQueue.TryDequeue(out message);
            if (message!=null)
            {
                _messagesInProgress.RegisterMessage(message);
            }
            return message;
        }

        public OutgoingMessageModel Commit(string CRMOBEID)
        {
            var message = _messagesInProgress.Commit(CRMOBEID);
            _processedObeQueue.Enqueue(CRMOBEID);
            return message;
        }


        public void Retry(string CRMOBEID)
        {
            _messagesInProgress.Retry(CRMOBEID);
            _processedObeQueue.Enqueue(CRMOBEID);
        }
    }
}
