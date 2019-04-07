using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Model
{
    [System.Serializable]
    public class AuthRequest
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string object_type = "AuthRequest";

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string key;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public long expires;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string hash;
    }
}