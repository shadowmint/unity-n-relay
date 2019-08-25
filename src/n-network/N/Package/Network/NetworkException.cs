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
            InvalidClientIdOnClientRequest,

            /// <summary>
            /// A request was received that didn't match any known binding.
            /// </summary>
            UnsupportedCommandType,
            CommandFailed
        }

        public NetworkException(NetworkExceptionType exceptionType) : base($"{exceptionType}")
        {
            ExceptionType = exceptionType;
        }

        public NetworkException(NetworkExceptionType exceptionType, string message) : base($"{exceptionType}: {message}")
        {
            ExceptionType = exceptionType;
        }
        
        public NetworkException(NetworkExceptionType exceptionType, Exception innerException) : base($"{exceptionType}", innerException)
        {
            ExceptionType = exceptionType;
        }
    }
}