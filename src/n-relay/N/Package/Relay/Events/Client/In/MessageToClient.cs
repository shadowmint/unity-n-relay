using System;
using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;
using UnityEngine;

namespace N.Package.Relay.Events.Client.In
{
    public class MessageToClient : ClientIncomingEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string data;
    }
}