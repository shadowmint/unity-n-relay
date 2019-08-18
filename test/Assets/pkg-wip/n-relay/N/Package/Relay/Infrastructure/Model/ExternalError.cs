using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Model
{
    [System.Serializable]
    public class ExternalError
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public int error_code;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string error_reason;
    }
}