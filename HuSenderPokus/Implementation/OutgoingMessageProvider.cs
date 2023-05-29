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
        IEnumerable<string> _partitions;

        public bool HasMessagesInFlight => _messagesInProgress.HasMessagesInFlight;

        public bool QueueFull => _outgoingMessageQueue.Count > MaxQueueSize;

        public void Enqueue(OutgoingMessageModel item)
        {
            _outgoingMessageQueue.Enqueue(item);
        }


        public OutgoingMessageModel? Dequeue()
        {
            lock (this)     // possible collision with AllowPartitionRemoval
            {
                OutgoingMessageModel message = null;
                while (true)
                {
                    string processedObe;
                    if (!_processedObeQueue.TryDequeue(out processedObe))
                        // no response to process, continue with normal processing
                        break;
                    if (_messagesInProgress.TryDequeue(processedObe, out message) && _partitions.Contains(message.PartitinoKey))
                    {
                        break;
                    }
                }
                if (message == null)
                {
                    while (true)
                    {
                        if (!_outgoingMessageQueue.TryDequeue(out message))
                            return null;

                        if (_partitions.Contains(message.PartitinoKey))
                            break;
                    }
                }

                if (message != null)
                {
                    _messagesInProgress.RegisterMessage(message);
                }
                return message;
            }
        }

        public OutgoingMessageModel GetMessageInFlight(string CRMOBEID)
        {
            return _messagesInProgress.GetMessageInFlight(CRMOBEID);
        }


        public void Commit(string CRMOBEID)
        {
            _messagesInProgress.Commit(CRMOBEID);
            _processedObeQueue.Enqueue(CRMOBEID);
        }


        public void Retry(string CRMOBEID)
        {
            _messagesInProgress.Retry(CRMOBEID);
            _processedObeQueue.Enqueue(CRMOBEID);
        }

        public void SetPartitions(IEnumerable<string> partitions)
        {
            _partitions = partitions;
        }

        public bool AllowPartitionRemoval(string partitionKey)
        {
            lock (this)
            {
                return _messagesInProgress.AllowPartitionRemoval(partitionKey);
            }
        }
    }
}
