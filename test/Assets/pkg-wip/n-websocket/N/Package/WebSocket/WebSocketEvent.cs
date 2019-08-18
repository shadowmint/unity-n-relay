using System;

namespace N.Package.WebSocket
{
    public struct WebSocketEvent
    {
        public WebSocketEventType EventType { get; set; }
        public Exception Error { get; set; }
        public string Data { get; set; }
    }
}