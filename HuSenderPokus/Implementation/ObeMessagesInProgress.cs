using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Model
{
    public class ObeMessagesInProgress
    {
        OutgoingMessageModel _messageInFlight;
        List<OutgoingMessageModel> _waitingList;

        public OutgoingMessageModel MessageInFlight => _messageInFlight;

        internal static ObeMessagesInProgress Create(OutgoingMessageModel message)
        {
            return new ObeMessagesInProgress() { _messageInFlight = message, _waitingList = new() };
        }

        internal ObeMessagesInProgress AddMessage(OutgoingMessageModel message)
        {
            lock (this)
            {
                if (_messageInFlight == null)
                {
                    _messageInFlight = message;
                } else
                {
                    _waitingList.Add(message);
                }
            }
            return this;
        }

        internal OutgoingMessageModel Commit()
        {
            lock (this)
            {
                var ret = _messageInFlight;
                _messageInFlight = null;
                return ret;
            }
        }

        internal void Retry()
        {
            lock (this)
            {
                if (_messageInFlight==null)
                {
                    throw new InvalidOperationException("No message in flight, cannot retry");
                }
                _waitingList.Insert(0, _messageInFlight);
                _messageInFlight=null;
            }
        }

        internal bool TryDequeue(out OutgoingMessageModel message)
        {
            lock(this)
            {
                if (_messageInFlight!=null)
                {
                    throw new InvalidOperationException($"Cannot dequeue message when there is still message in flight (Id: {_messageInFlight.Id})");
                }
                if (_waitingList.Count==0)
                {
                    message=null;
                    return false;
                } else
                {
                    message = _waitingList[0];
                    _waitingList.RemoveAt(0);
                    return true;
                }
            }
        }
    }
}
