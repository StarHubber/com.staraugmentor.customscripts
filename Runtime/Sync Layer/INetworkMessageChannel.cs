using UnityEngine;

namespace StarCooperation
{
    public interface INetworkMessageChannel
    {
        string ChannelID { get; }
        void Dispatch(string messageData);
    }
}