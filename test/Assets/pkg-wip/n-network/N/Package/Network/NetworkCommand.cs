using System;

namespace N.Package.Network
{
    [System.Serializable]
    public class NetworkCommand
    {
        public string commandInternalType;

        public string commandInternalId;

        public bool commandInternalIsResponse;
        
        /// <summary>
        /// Invoke before sending to force the command type to be correct.
        /// </summary>
        public void Prepare(bool isRequest)
        {
            commandInternalType = CommandTypeFor(GetType());
            commandInternalId = Guid.NewGuid().ToString();
            commandInternalIsResponse = !isRequest;
        }

        public static string CommandTypeFor(Type t)
        {
            var fullName = t.FullName;
            return fullName?.ToLower();
        }
    }
}