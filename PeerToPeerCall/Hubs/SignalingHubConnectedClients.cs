using PeerToPeerCall.Models;
using System.Collections.Generic;

namespace PeerToPeerCall.Hubs
{
    public class SignalingHubConnectedClients
    {
        public List<TargetPeerDto> ConnectedClients { get; } = new();
    }
}
