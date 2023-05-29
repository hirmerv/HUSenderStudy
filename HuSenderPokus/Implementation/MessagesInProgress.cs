using HuSenderPokus.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Model
{
    public class MessagesInProgress : IMessagesInProgress
    {
        ConcurrentDictionary<string, ObeMessagesInProgress> _obeDict;

 
        public void RegisterMessage(OutgoingMessageModel message)
        {
            _obeDict.AddOrUpdate(message.CRMOBEID, obeId => ObeMessagesInProgress.Create(message), (obeId, oldValue) => oldValue.AddMessage(message));
        }


        public OutgoingMessageModel Commit(string obeId)
        {
            var ret = _obeDict[obeId].Commit();
            if (ret==null)
            {
                throw new InvalidOperationException($"There is no message in flight for OBE {obeId}, cannot commit");
            }
            return ret;
        }


        public void Retry(string obeId)
        {
            _obeDict[obeId].Retry();
        }

        public bool TryDequeue(string obeId, out OutgoingMessageModel message)
        {
            return _obeDict[obeId].TryDequeue(out message);
        }

        public bool HasMessagesInFlight => _obeDict.Any(x => x.Value.MessageInFlight != null);   

    }
}
