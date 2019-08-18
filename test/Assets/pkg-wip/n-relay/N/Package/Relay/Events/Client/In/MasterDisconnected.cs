using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Events.Client.In
{
    public class MasterDisconnected : ClientIncomingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string reason;
    }
}