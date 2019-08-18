using System;

namespace N.Package.Relay.Rpc
{
    public class RelayRpcCommandHandler
    {
        public static RelayRpcEventHandler For<TRequest, TResponse>()
        /// <summary>
        /// A unique command id for this type of command.
        /// </summary>
        string RelayRpcCommandId { get; }

        /// <summary>
        /// Serialize this objects accepted by this handler as a string for sending. 
        /// </summary>
        string Serialize<T>(T request)
        {
        }

        /// <summary>
        /// For the sender, deserialize the response object.
        /// </summary>
        TResponse DeserializeResponse(string raw);

        TReqeust
    }
}