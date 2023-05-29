using HuSenderPokus.Interface;
using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Worker
{
    public class SendWorker 
    {
        const int DelayNoData = 1000;  // parameter in real implmementation

        IOutgoingMessageProvider _outgoingMessageProvider;
        IKafkaSender _sender;
        IConfirmationQueue _confirmationQueue;
        bool _cancelled = false;


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


        private void OnDelivery(DeliveryReport<string, OutgoingMessageModel> report)
        {
            if (report.Status==PersistenceStatus.Persisted)
            {
                // use instance level variable to potentially block partition removal during this operation
                var messageBeingCommitted = _outgoingMessageProvider.GetMessageInFlight(report.Key);
                messageBeingCommitted.TargetOffset = report.Offset;
                messageBeingCommitted.TargetPartitionKey = report.Partition.ToString();
                messageBeingCommitted.ProcessStatus = OutgoingMessageProcessStatus.Sent;
                // the order of the following two lines is important for preventing potential race condition when releasing partition
                _confirmationQueue.Enqueue(messageBeingCommitted);
                _outgoingMessageProvider.Commit(messageBeingCommitted.CRMOBEID);
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
