using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Events
{
    
    public class MasterOutgoingEvent : RelayOutgoingEvent, IRelayTransactionEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string transaction_id;
        
        public string TransactionId
        {
            get { return transaction_id; }
            set { transaction_id = value; }
        }
    }
}