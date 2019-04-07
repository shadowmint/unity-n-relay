using System;
using System.Threading;
using N.Package.Promises;
using N.Package.Relay;
using N.Package.Relay.Infrastructure.Model;
using UnityEngine;

namespace WsTest
{
    class ServiceMasterComponent : MonoBehaviour
    {
        public string remote = "localhost:9977";

        public bool isActive;

        private ServiceMaster _service;

        public void Update()
        {
            if (isActive && _service == null)
            {
                _service = new ServiceMaster();
                _service.Start(remote, new RelayMasterOptions()
                    {
                        ConnectionTimeout = TimeSpan.FromSeconds(10),
                        metadata = new MasterMetadata()
                        {
                            master_id = "HelloWorld",
                            max_clients = 2
                        },
                        auth = new RelayAuthOptions()
                        {
                            authKey = "key1234567890",
                            authSecret = "secret1234567890",
                            sessionLength = 1800
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
        }

        private void OnDestroy()
        {
            _service?.Halt();
        }
    }
}