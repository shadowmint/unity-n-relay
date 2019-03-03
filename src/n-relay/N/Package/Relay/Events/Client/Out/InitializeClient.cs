using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;
using N.Package.Relay.Infrastructure.Model;

namespace N.Package.Relay.Events.Client.Out
{
    public class InitializeClient : ClientOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public ClientMetadata metadata;
    }
}