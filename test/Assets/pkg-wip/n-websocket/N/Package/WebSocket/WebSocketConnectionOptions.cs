using System;

namespace N.Package.WebSocket
{
    public class WebSocketConnectionOptions
    {
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// This is a sensible default; don't override it unless you understanding
        /// how the client aborts when timeouts occur.
        /// </summary>
        public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// This is a sensible default; don't override it unless you understanding
        /// how the client aborts when timeouts occur. ...and yes, it should be in
        /// minutes.
        /// </summary>
        public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromMinutes(60);
    }
}