using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Events
{
    public interface IRelayTransactionEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        string TransactionId { get; set; }
    }
}