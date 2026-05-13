using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace StarCooperation
{
    public class SceneManagerNamedMethodChannel: INetworkMessageChannel
    {
        class DispatchRoute
        {
            public int ID { get; }
            public string HandlerMethod { get; }
            public ISerializableMessagePayload PayloadTemplate { get; }
            public Type PayloadType { get; }

            public DispatchRoute(string handlerMethod, [NotNull] ISerializableMessagePayload payloadTemplate)
            {
                ID = handlerMethod.GetHashCode();
                HandlerMethod = handlerMethod;
                PayloadTemplate = payloadTemplate;
                PayloadType = PayloadTemplate.GetType();
            }
        }
        
        static char[] delimiter = {'|'};

        Dictionary<int, DispatchRoute> registeredRoutes = new Dictionary<int,DispatchRoute>();
        
        public string ChannelID { get; }

        public SceneManagerNamedMethodChannel(string channelID)
        {
            ChannelID = channelID;
        }

        //Adds the route and returns its id used to send messages
        public int AddRoute<T>(string handlerMethod) where T : ISerializableMessagePayload, new()
        {
            var route = new DispatchRoute(handlerMethod, new T());
            if (registeredRoutes.ContainsKey(route.ID)) throw new Exception("Handler Method name already reserved");
            registeredRoutes.Add(route.ID, route);
            return route.ID;
        }

        bool FindRoute(int id, out DispatchRoute route)
        {
            return registeredRoutes.TryGetValue(id, out route);
        }

        public void Dispatch(string messageData)
        {
            if (!DeconstructMessage(messageData, out var id, out var payloadData))
            {
#if UNITY_EDITOR
                throw new Exception($"Unable to parse network message content ({messageData})");
#else
                //Debug.LogError($"Unable to parse network message content ({messageData}), skipping");
                //return;
#endif
            }
            if (!FindRoute(id, out var route))
            {
#if UNITY_EDITOR
                throw new Exception($"Network message route not found for id {id}");
#else
                //Debug.LogError($"Network message route not found for id {id}, skipping");
                //return;
#endif
            }
            var payload = route.PayloadTemplate.CreateUninitializedInstance();
            payload.Deserialize(payloadData);
            SceneManager.instance.SendMessage(route.HandlerMethod, payload);
        }

        public string CreateMessage<T>(int routeID, T payload) where T : ISerializableMessagePayload
        {
            var payloadData = payload.Serialize();
            if (!FindRoute(routeID, out var route))
            {
#if UNITY_EDITOR
                throw new Exception($"Network message route ({routeID}) not found for payload {typeof(T).Name}");
#else
                //Debug.LogError($"Network message route ({routeID}) not found for payload {typeof(T).Name}, skipping");
                //return;
#endif
            }
            return ConstructMessage(route.ID, payloadData);
        }

        string ConstructMessage(int routeID, string payloadData)
        {
            var safeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadData));
            return $"{routeID}{delimiter[0]}{safeString}";
        }

        bool DeconstructMessage(string input, out int id, out string payloadData)
        {
            var parts = input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !int.TryParse(parts[0], out id))
            {
                id = -1;
                payloadData = string.Empty;
                return false;
            }
            payloadData = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));
            return true;
        }

    }
}