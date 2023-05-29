using HuSenderPokus.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Worker
{
    public class Dispatcher // TODO : BackgroundService
    {
        const int DelayDispatcher = 1000;  // configurable ?

        SendWorker _sendWorker;
        UpdateStatusWorker _updateStatusWorker;
        InputWorker _inputWorker;
        IPartitionStatus _partitionStatus;
        IOutgoingMessageProvider _outgoingMessageProvider;
        IConfirmationQueue _confirmationQueue;

        /*
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: register worker + initial partition set
        }
        */

        public async void Execute(CancellationToken token)
        {


            var inputTask = new Task(()=>_inputWorker.Execute(token));
            var sendTask = new Task(() => _sendWorker.Execute(token));
            var updateStatusTask = new Task(() => _updateStatusWorker.Execute());

            while (true)
            {
                if (token.IsCancellationRequested) { break; }
                // else: check partitions, do healthchecks, heartbeats etc.
                CheckPartitionUpdates();
                await Task.Delay(DelayDispatcher);
            }
            // finalize
            Task.WaitAll(inputTask, sendTask, updateStatusTask);
            // explicitly exit application here ?
        }

        private void CheckPartitionUpdates()
        {
            bool changed = _partitionStatus.CheckUpdates();
            if (changed)
            {
                _inputWorker.SetPartitions(_partitionStatus.ActivePartitions);
                _outgoingMessageProvider.SetPartitions(_partitionStatus.ActivePartitions);
            }
            foreach (string partitionKey in  _partitionStatus.PartitionWaitingForRemoval)
            {
                if (_outgoingMessageProvider.AllowPartitionRemoval(partitionKey) 
                   && _confirmationQueue.AllowPartitionRemoval(partitionKey))
                {
                    _partitionStatus.ConfirmPartitionRemoval(partitionKey);
                }
                
            }
        }
    }
}
