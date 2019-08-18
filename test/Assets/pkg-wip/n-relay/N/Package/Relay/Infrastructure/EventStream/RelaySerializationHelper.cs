using System;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure.Events;
using N.Package.Relay.Infrastructure.Model;
using UnityEngine;
using MasterDisconnected = N.Package.Relay.Events.Client.In.MasterDisconnected;
using MessageFromClient = N.Package.Relay.Events.Master.In.MessageFromClient;
using MessageToClient = N.Package.Relay.Events.Client.In.MessageToClient;

namespace N.Package.Relay.Infrastructure.EventStream
{
    public class RelaySerializationHelper
    {
        public RelayIncomingEvent Deserialize(string input)
        {
            try
            {
                var typeContainer = JsonUtility.FromJson<RelayIncomingEvent>(input);
                return Deserialize(typeContainer.object_type, input);
            }
            catch (Exception error)
            {
                throw new RelayException(RelayErrorCode.SerializationError, $"Invalid raw: {input}: {error}");
            }
        }

        public RelayIncomingEvent Deserialize(string objectType, string input)
        {
            RelayIncomingEvent rtn;

            // Master events which are incoming from the relay server
            if (Deserialize<ClientDisconnected>(objectType, input, out rtn))
            {
                return rtn;
            }

            if (Deserialize<ClientJoined>(objectType, input, out rtn))
            {
                return rtn;
            }

            if (Deserialize<TransactionResult>(objectType, input, out rtn))
            {
                return rtn;
            }

            if (Deserialize<MessageFromClient>(objectType, input, out rtn))
            {
                return rtn;
            }

            if (Deserialize<MasterDisconnected>(objectType, input, out rtn))
            {
                return rtn;
            }

            if (Deserialize<MessageToClient>(objectType, input, out rtn))
            {
                return rtn;
            }

            throw new RelayException(RelayErrorCode.UnknownObjectCode, objectType);
        }

        private bool Deserialize<T>(string objectType, string input, out RelayIncomingEvent incomingEvent) where T : RelayIncomingEvent
        {
            if (objectType == typeof(T).Name)
            {
                incomingEvent = JsonUtility.FromJson<T>(input);
                return true;
            }

            incomingEvent = null;
            return false;
        }

        public string Serialize<T>(T instance)
        {
            return JsonUtility.ToJson(instance);
        }
    }
}