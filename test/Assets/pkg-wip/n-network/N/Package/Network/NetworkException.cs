using System;

namespace N.Package.Network
{
    public class NetworkException : Exception
    {
        public NetworkExceptionType ExceptionType { get; }

        public enum NetworkExceptionType
        {
            /// <summary>
            /// Master must specify a client id to send a message.
            /// </summary>
            MissingClientIdOnMasterRequest,

            /// <summary>
            /// Client cannot specify a client id; can only send to master.
            /// </summary>
            InvalidClientIdOnClientRequest
        }

        public NetworkException(NetworkExceptionType exceptionType) : base($"{exceptionType}")
        {
            ExceptionType = exceptionType;
        }
    }
}