using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Events.Master.In
{
    public class ClientJoined : MasterIncomingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string client_id;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string name;
    }
}