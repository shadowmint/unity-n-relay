using System;
using N.Package.Relay.Events;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Config
{
    public class RelayClientConfig
    {
        /// <summary>
        /// The relay server connection details
        /// </summary>
        public RelayConnectionConfig Connection { get; set; }

        /// <summary>
        /// The name of this client.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The session to join.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Expire transactions which are older than this.
        /// </summary>
        public TimeSpan TransactionTimeout = TimeSpan.FromSeconds(60);

        public Action<RelayException> OnError { get; set; }

        public Action<ClientIncomingEvent> OnEvent { get; set; }
    }
}