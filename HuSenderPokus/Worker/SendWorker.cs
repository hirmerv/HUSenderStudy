using HuSenderPokus.Interface;
using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Worker
{
    public class SendWorker : IPartitionFilter, IPartitionRemovalChecker
    {
        const int DelayNoData = 1000;  // parameter in real implmementation

        IOutgoingMessageProvider _outgoingMessageProvider;
        IKafkaSender _sender;
        IConfirmationQueue _confirmationQueue;
        bool _cancelled = false;
        OutgoingMessageModel _messageBeingCommitted;   // just to eliminate rare race condition during partition removal

        public bool AllowPartitionRemoval(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public async void Execute(CancellationToken token)
        {
            OutgoingMessageModel messageToSend;
            // TODO: cancellation token
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    _cancelled = true;
                    break;
                }
                messageToSend = _outgoingMessageProvider.Dequeue();
                if (messageToSend == null)
                {
                    await Task.Delay(DelayNoData, token);
                } else
                {
                    messageToSend.SentAt = DateTime.UtcNow;
                    _sender.Produce(messageToSend, report => OnDelivery(report));
                }
            }
        }

        public void SetPartitions(IEnumerable<string> partitions)
        {
            throw new NotImplementedException();
        }

        private void OnDelivery(DeliveryReport<string, OutgoingMessageModel> report)
        {
            if (report.Status==PersistenceStatus.Persisted)
            {
                // use instance level variable to potentially block partition removal during this operation
                _messageBeingCommitted = _outgoingMessageProvider.Commit(report.Key);
                _messageBeingCommitted.TargetOffset = report.Offset;
                _messageBeingCommitted.TargetPartitionKey = report.Partition.ToString();
                _messageBeingCommitted.ProcessStatus = OutgoingMessageProcessStatus.Sent;
                _confirmationQueue.Enqueue(_messageBeingCommitted);
                _messageBeingCommitted = null;
                if (_cancelled && !_outgoingMessageProvider.HasMessagesInFlight)
                {
                    _confirmationQueue.MarkComplete();
                } 
            } else
            {
                // retry: naive implementation - not counting unsuccesful retries
                _outgoingMessageProvider.Retry(report.Key);
            }
        }
    }
}
