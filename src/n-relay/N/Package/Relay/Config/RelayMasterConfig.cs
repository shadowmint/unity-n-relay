using System;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Config
{
    public class RelayMasterConfig
    {
        /// <summary>
        /// The relay server connection details
        /// </summary>
        public RelayConnectionConfig Connection { get; set; }

        /// <summary>
        /// The maximum number of clients to accept
        /// </summary>
        public int MaxClients { get; set; }

        /// <summary>
        /// The name of the session.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Expire transactions which are older than this.
        /// </summary>
        public TimeSpan TransactionTimeout = TimeSpan.FromSeconds(60);

        public Action<RelayException> OnError { get; set; }

        public Action<MasterIncomingEvent> OnEvent { get; set; }
    }
}