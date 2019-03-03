using System;
using N.Package.Relay.Infrastructure.Model;

namespace N.Package.Relay.Infrastructure
{
    public class RelayException : Exception
    {
        public RelayException(Exception error) : base(RelayErrorCode.InnerError.ToString(), error)
        {
        }

        public RelayException(RelayErrorCode code) : base(code.ToString())
        {
            Code = code;
        }

        public RelayException(RelayErrorCode code, Exception error) : base(code.ToString(), error)
        {
            Code = code;
        }

        public RelayException(RelayErrorCode code, string message) : base($"{code}: {message}")
        {
            Code = code;
        }

        public RelayException(ExternalError externalError) : base(
            $"{RelayErrorCode.ExternalError}: {externalError.error_reason} ({externalError.error_code})")
        {
            Code = RelayErrorCode.ExternalError;
        }

        public RelayErrorCode Code { get; set; }
    }
}