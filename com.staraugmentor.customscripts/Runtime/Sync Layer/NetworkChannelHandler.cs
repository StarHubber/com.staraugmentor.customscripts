using System;
using System.Collections.Generic;

namespace StarCooperation
{
    public class NetworkChannelHandler
    {
        public string RoutedMessageKey { get; }
        
        List<INetworkMessageChannel> messageChannels = new List<INetworkMessageChannel>();

        public NetworkChannelHandler(string routingKey, params INetworkMessageChannel[] channels)
        {
            RoutedMessageKey = routingKey;
            var usedKeys = new HashSet<string>();
            foreach (var channel in channels)
            {
                if (!usedKeys.Contains(channel.ChannelID))
                {
                    usedKeys.Add(channel.ChannelID);
                    messageChannels.Add(channel);
                }
                else
                {
#if UNITY_EDITOR
                    throw new Exception($"Duplicate Network Message Channel registration ({channel.ChannelID})");
#else
					//Debug.LogError($"Duplicate Network Message Channel ({channel.ChannelID}), ignoring");
					//return;
#endif	
                }
            }
        }

        public bool FindChannel(string channelID, out INetworkMessageChannel channel)
        {
            foreach (var c in messageChannels)
            {
                if (c.ChannelID == channelID)
                {
                    channel = c;
                    return true;
                }
            }
            channel = null;
            return false;
        }

        public bool FindChannel<T>(out T channel) where T: INetworkMessageChannel
        {
            var channelType = typeof(T);
            foreach (var c in messageChannels)
            {
                if (c.GetType() == channelType)
                {
                    channel = (T)c;
                    return true;
                }
            }
            channel = default;
            return false;
        }

    }
}