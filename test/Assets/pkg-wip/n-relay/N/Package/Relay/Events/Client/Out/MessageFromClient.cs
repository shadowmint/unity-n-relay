using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Events.Client.Out
{
    public class MessageFromClient : ClientOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string data;
    }
}