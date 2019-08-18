using System;

namespace N.Package.Network
{
    [System.Serializable]
    public class NetworkCommand
    {
        public string commandType;

        /// <summary>
        /// Invoke before sending to force the command type to be correct.
        /// </summary>
        public void Prepare()
        {
            commandType = CommandTypeFor(GetType());
        }

        public static string CommandTypeFor(Type t)
        {
            var fullName = t.FullName;
            return fullName?.ToLower();
        }
    }
}