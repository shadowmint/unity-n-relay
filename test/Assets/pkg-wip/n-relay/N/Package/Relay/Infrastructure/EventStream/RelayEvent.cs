using System;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Infrastructure.EventStream
{
    public class RelayEvent
    {
        public RelayEventType EventType { get; set; }
        public RelayIncomingEvent IncomingEvent { get; set; }
        public Exception Error { get; set; }
    }
}