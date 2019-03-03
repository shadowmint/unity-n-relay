using System;
using System.Threading.Tasks;
using N.Package.Relay.Infrastructure.Events;
using N.Package.WebSocket;

namespace N.Package.Relay.Infrastructure.EventStream
{
    public class RelayEventStream
    {
        private readonly Func<RelayEvent, RelayEventStream, Task> _onEvent;

        private WebSocketEventStream _connection;

        private readonly RelaySerializationHelper _serializer;

        public RelayEventStream(Func<RelayEvent, RelayEventStream, Task> onEvent)
        {
            _onEvent = onEvent;
            _serializer = new RelaySerializationHelper();
        }

        public async Task Connect(string remote, WebSocketConnectionOptions options = null)
        {
            _connection = new WebSocketEventStream(OnWebSocketEvent);
            await _connection.Connect(remote, options);
        }

        public async Task Disconnect()
        {
            if (_connection == null) return;
            var events = _connection;
            _connection = null;
            await events.Disconnect();
        }

        public void Dispose()
        {
            Disconnect().Wait();
        }

        public async Task Send(RelayOutgoingEvent message)
        {
            if (_connection == null)
            {
                throw new Exception("Not connected");
            }

            message.object_type = message.GetType().Name;
            var raw = _serializer.Serialize(message);
            await _connection.Send(raw);
        }

        private async Task OnWebSocketEvent(WebSocketEvent webSocketEvent, WebSocketEventStream events)
        {
            switch (webSocketEvent.EventType)
            {
                case WebSocketEventType.Connected:
                    await TriggerConnected();
                    break;
                case WebSocketEventType.Disconnected:
                    await TriggerDisconnected();
                    break;
                case WebSocketEventType.Error:
                    await TriggerError(webSocketEvent.Error);
                    break;
                case WebSocketEventType.Data:
                    await TriggerRelayEvent(webSocketEvent.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task TriggerConnected()
        {
            await Trigger(new RelayEvent()
            {
                EventType = RelayEventType.Connected
            });
        }

        private async Task TriggerDisconnected()
        {
            await Trigger(new RelayEvent()
            {
                EventType = RelayEventType.Disconnected
            });
        }

        private async Task TriggerError(Exception error)
        {
            await Trigger(new RelayEvent()
            {
                EventType = RelayEventType.Error,
                Error = error
            });
        }

        private async Task TriggerRelayEvent(string raw)
        {
            var typedObject = _serializer.Deserialize(raw);
            await Trigger(new RelayEvent()
            {
                EventType = RelayEventType.IncomingEvent,
                IncomingEvent = typedObject
            });
        }

        private async Task Trigger(RelayEvent relayEvent)
        {
            try
            {
                var task = _onEvent?.Invoke(relayEvent, this);
                if (task != null)
                {
                    await task;
                }
            }
            catch (Exception error)
            {
                try
                {
                    await TriggerError(error);
                }
                catch (Exception)
                {
                    // Just ignore it, we tried to raise it already
                }
            }
        }
    }
}