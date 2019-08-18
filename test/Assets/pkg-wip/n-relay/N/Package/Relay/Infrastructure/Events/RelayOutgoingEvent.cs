using System;
using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Events
{
    [Serializable]
    public class RelayOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string object_type;
    }
}