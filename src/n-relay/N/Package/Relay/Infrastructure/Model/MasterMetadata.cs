using System.Diagnostics.CodeAnalysis;

namespace N.Package.Relay.Infrastructure.Model
{
    [System.Serializable]
    public class MasterMetadata
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string master_id;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public int max_clients;
    }
}