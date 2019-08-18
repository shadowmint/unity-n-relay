using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;
using N.Package.Relay.Infrastructure.Model;

namespace N.Package.Relay.Events.Master.Out
{
    /// <summary>
    /// Serialized form: {"object_type":"Auth","transaction_id":"1234","request":{"object_type":"AuthRequest","expires":12312312312,"key":"public_key_1adfasdfasdf","hash":"12312321312312321"}}
    /// </summary>
    public class Auth : MasterOutgoingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public AuthRequest request;
    }
}