using System;
using N.Package.Promises;
using N.Package.Relay;
using N.Package.Relay.Infrastructure.Model;
using UnityEngine;

namespace WsTest
{
    class ServiceClientComponent : MonoBehaviour
    {
        public string remote = "localhost:9977";

        public string id = "Client 1";
        
        public bool isActive;

        public float elapsed;

        public bool sendMessages;

        private ServiceClient _service;

        public void Update()
        {
            if (isActive && _service == null)
            {
                _service = new ServiceClient();
                _service.Start(remote, new RelayClientOptions()
                    {
                        SessionId = "HelloWorld",
                        Metadata = new ClientMetadata()
                        {
                            name = id
                        }
                    })
                    .Promise()
                    .Then(() => { Debug.Log("Connection passed!"); }, Debug.LogException)
                    .Dispatch();
            }
            else if (!isActive && _service != null)
            {
                _service.Halt();
                _service = null;
            }

            elapsed += Time.deltaTime;
            if (elapsed > 10f)
            {
                elapsed = 0;
                SendHello();
            }
        }

        private void SendHello()
        {
            if (!sendMessages || _service == null || !_service.connected) return;
            Debug.Log("Sending a hello");
            _service.Send(new DemoMessage {Message = "HelloWorld"}).Dispatch();
        }

        private void OnDestroy()
        {
            _service?.Halt();
        }
    }
}