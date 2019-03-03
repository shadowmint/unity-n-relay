using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;
using N.Package.Relay.Infrastructure.Model;

namespace N.Package.Relay.Events.Master.Out
{
    public class InitializeMaster : MasterOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public MasterMetadata metadata;
    }
}