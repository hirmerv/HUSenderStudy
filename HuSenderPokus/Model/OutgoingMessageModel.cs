using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuSenderPokus.Model
{
    public class OutgoingMessageModel
    {
        public long Id;
        public string PartitinoKey;
        public string CRMOBEID;
        public string Content;
        public string RequestId;
        public bool ResetFlag;
        public long TargetOffset;
        public string TargetPartitionKey;
        public OutgoingMessageProcessStatus ProcessStatus;
        public DateTime SentAt;
    }

    public enum OutgoingMessageProcessStatus
    {
        New,
        Sent,
        Confirmed,
        Rejected
    }
}
