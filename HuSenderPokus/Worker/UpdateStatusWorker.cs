﻿using HuSenderPokus.Interface;
using HuSenderPokus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Worker
{
    public class UpdateStatusWorker
    {
        const int DelayNoData = 1000;  // parameter in real implmementation


        IConfirmationQueue _confirmationQueue;

        public async void Execute()
        {
            OutgoingMessageModel messageToConfirm;

            while (true)
            {
                messageToConfirm = _confirmationQueue.Dequeue();
                if (messageToConfirm == null)
                {
                    if (_confirmationQueue.IsComplete)
                    {
                        break;
                    }

                    await Task.Delay(DelayNoData);
                }
                else
                {
                   // TODO: update message in table
                }

            }

        }
    }
}
