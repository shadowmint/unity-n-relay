using System;
using System.Collections;
using System.Threading.Tasks;
using N.Package.Promises;
using UnityEngine;

namespace N.Package.WebSocket
{
    public class WebSocketEventStream : IDisposable
    {
        private WebSocketConnection _connection;

        private readonly Func<WebSocketEvent, WebSocketEventStream, Task> _onEvent;

        public WebSocketEventStream(Func<WebSocketEvent, WebSocketEventStream, Task> onEvent)
        {
            _onEvent = onEvent;
        }

        public async Task Connect(string remote, WebSocketConnectionOptions options = null)
        {
            _connection = new WebSocketConnection();
            await _connection.Connect(remote, options);

            AsyncWorker.Run(EventLoop);
            await TriggerConnected();
        }

        private IEnumerator EventLoop()
        {
            var busy = false;
            while (_connection != null)
            {
                if (!busy)
                {
                    busy = true;
                    EventLoopStep(() => { busy = false; }).Dispatch();
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private async Task EventLoopStep(Action onComplete)
        {
            try
            {
                var message = await _connection.Read();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    await TriggerData(message);    
                }                
            }
            catch (Exception error)
            {
                if (_connection == null || _connection.Active) return;
                await TriggerError(error);
                await Disconnect();
            }

            onComplete();
        }

        public async Task Disconnect()
        {
            if (_connection == null)
            {
                return;
            }

            var connection = _connection;
            _connection = null;
            await connection.Disconnect();
            await TriggerDisconnected();
        }

        public void Dispose()
        {
            Disconnect().Wait();
        }

        public async Task Send(string message)
        {
            if (_connection == null)
            {
                throw new Exception("Not connected");
            }

            await _connection.Send(message);
        }

        private Task TriggerDisconnected()
        {
            return Trigger(new WebSocketEvent()
            {
                EventType = WebSocketEventType.Disconnected
            });
        }

        private Task TriggerConnected()
        {
            return Trigger(new WebSocketEvent()
            {
                EventType = WebSocketEventType.Connected
            });
        }

        private Task TriggerData(string message)
        {
            return Trigger(new WebSocketEvent()
            {
                EventType = WebSocketEventType.Data,
                Data = message
            });
        }

        private Task TriggerError(Exception error)
        {
            return Trigger(new WebSocketEvent()
            {
                EventType = WebSocketEventType.Error,
                Error = error
            });
        }

        private async Task Trigger(WebSocketEvent e)
        {
            try
            {
                var task = _onEvent?.Invoke(e, this);
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