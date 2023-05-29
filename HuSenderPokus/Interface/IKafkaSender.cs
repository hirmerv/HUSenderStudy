using HuSenderPokus.Model;

namespace HuSenderPokus.Interface
{
    // just simulate behavior
    public interface IKafkaSender
    {
        public void Produce(OutgoingMessageModel message, Action<DeliveryReport<string, OutgoingMessageModel>> onDeliveryAction);
    }

    public class DeliveryReport<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public long Offset { get; set; }
        public int Partition { get; set; }
        public PersistenceStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum PersistenceStatus
    {
        NotPersisted,
        Persisted,
        PossiblyPersisted
    }
}
