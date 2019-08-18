using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Events.Master.Out
{
    public class MessageToClient : MasterOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string client_id;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string data;
    }
}