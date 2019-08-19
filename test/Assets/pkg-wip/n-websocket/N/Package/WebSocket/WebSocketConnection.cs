using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using N.Package.WebSocket.Internal;
using UnityEngine;

namespace N.Package.WebSocket
{
    public class WebSocketConnection : IDisposable
    {
        private WebSocketConnectionState _state = WebSocketConnectionState.Closed;

        private WebSocketConnectionOptions _options;

        private ClientWebSocket _client;

        private MemoryStream _recvStream;

        private byte[] _recvBuffer;

        public WebSocketConnection()
        {
            _recvStream = new MemoryStream();
            _recvBuffer = new byte[65535];
        }

        public bool Active => _client != null && _client.State == WebSocketState.Open && _state == WebSocketConnectionState.Open;

        public async Task Connect(string remote, WebSocketConnectionOptions options = null)
        {
            if (_state != WebSocketConnectionState.Closed)
            {
                throw new Exception($"Invalid state transition {_state} -> {WebSocketConnectionState.Connecting}");
            }

            if (options == null)
            {
                options = new WebSocketConnectionOptions();
            }

            _state = WebSocketConnectionState.Connecting;
            _options = options;

            // fix urls with no prefix
            if (!remote.StartsWith("ws://"))
            {
                remote = $"ws://{remote}";
            }

            _client = new ClientWebSocket()
            {
                Options =
                {
                    KeepAliveInterval = _options.KeepAliveInterval
                }
            };

            using (var cts = new CancellationTokenSource(_options.ConnectionTimeout))
            {
                await _client.ConnectAsync(new Uri(remote), cts.Token);
            }

            _state = WebSocketConnectionState.Open;
        }

        public async Task<String> Read()
        {
            _recvStream.Seek(0, SeekOrigin.Begin);
            _recvStream.SetLength(0);

            while (true)
            {
                // Note: If a timeout occurs during the read operation, the socket is aborted by the client.
                // This is a long-running operation; in general this means if your socket is totally idle for
                // > ReadTimeout your websocket connection will ABORT.
                using (var cts = new CancellationTokenSource(_options.ReadTimeout))
                {
                    var recvSegment = new ArraySegment<byte>(_recvBuffer, 0, _recvBuffer.Length);
                    var recvResult = await _client.ReceiveAsync(recvSegment, cts.Token);
                    if (recvSegment.Array == null)
                    {
                        break;
                    }

                    _recvStream.Write(recvSegment.Array, 0, recvResult.Count);
                    if (recvResult.EndOfMessage)
                    {
                        break;
                    }
                }
            }

            var byteValue = _recvStream.ToArray();
            var stringValue = Encoding.UTF8.GetString(byteValue);
            return stringValue;
        }

        public async Task Disconnect()
        {
            if (_state == WebSocketConnectionState.Closed) return;
            if (_client == null) return;
            try
            {
                using (var cts = new CancellationTokenSource(_options.SendTimeout))
                {
                    await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Completed", cts.Token);
                }

                _client.Dispose();
                _client = null;
                _state = WebSocketConnectionState.Closed;
            }
            catch (Exception)
            {
                // If we fail, just ignore it
            }
        }

        public void Dispose()
        {
            Disconnect().Wait();
        }

        public async Task Send(string message)
        {
            var sendStream = new MemoryStream();
            var outputBytes = Encoding.UTF8.GetBytes(message);
            sendStream.Write(outputBytes, 0, outputBytes.Length);

            sendStream.Seek(0, SeekOrigin.Begin);
            var size = sendStream.Length;
            var buffer = sendStream.GetBuffer();

            var segment = new ArraySegment<byte>(buffer, 0, (int) size);
            using (var cts = new CancellationTokenSource(_options.SendTimeout))
            {
                await _client.SendAsync(segment, WebSocketMessageType.Text, true, cts.Token);
            }
        }
    }
}