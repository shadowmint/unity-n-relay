using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Events
{
    public class RelayIncomingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string object_type;
    }
}