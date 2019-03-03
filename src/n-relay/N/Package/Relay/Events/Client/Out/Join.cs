using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Events.Client.Out
{
    public class Join : ClientOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string session_id;
    }
}