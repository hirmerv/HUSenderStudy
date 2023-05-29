using HuSenderPokus.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Worker
{
    public class InputWorker : IPartitionFilter
    {

        const int DelayNoData = 1000;  // parameter in real implmementation
        const int DelayQueueFull = 1000;  // parameter in real implmementation

        IMessageReader _messageReader;
        IOutgoingMessageProvider _outgoingMessageProvider;

        public async void Execute(CancellationToken token)
        {

 
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                if (_outgoingMessageProvider.QueueFull)
                {
                    await Task.Delay(DelayQueueFull, token);
                    continue;
                }
                var messagesRead = _messageReader.ReadMessages();
                if (messagesRead.Count() == 0)
                {
                    await Task.Delay(DelayNoData, token);
                } else
                {
                    foreach (var message in messagesRead)
                    {
                        _outgoingMessageProvider.Enqueue(message);
                    }
                }
            }
        }

        public void SetPartitions(IEnumerable<string> partitions)
        {
            throw new NotImplementedException();
        }
    }
}
