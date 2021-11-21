using Microsoft.AspNetCore.SignalR;
using PeerToPeerCall.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerToPeerCall.Hubs
{
    public class SignalingHub : Hub
    {
        private readonly SignalingHubConnectedClients _signalingHubConnectedClients;

        public SignalingHub(SignalingHubConnectedClients signalingHubConnectedClients)
        {
            _signalingHubConnectedClients = signalingHubConnectedClients;
        }

        public override Task OnConnectedAsync()
        {
            ImAvailable();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ImNotAvailable();

            return base.OnDisconnectedAsync(exception);
        }

        public TargetPeerDto GetNextPeerToCall()
        {
            var availablePeers = _signalingHubConnectedClients.ConnectedClients.Where(p => p.ConnectionId != Context.ConnectionId).ToList();

            if (availablePeers.Count == 0) return null;

            return availablePeers.ElementAt(new Random().Next(0, availablePeers.Count));
        }

        public async Task SendOfferAsync(TargetPeerDto targetPeerDto, string offer)
        {
            await Clients.Client(targetPeerDto.ConnectionId).SendAsync("ReceivedConnectionRequest", new TargetPeerDto
            {
                ConnectionId = Context.ConnectionId,
                UserName = Context.User.Identity.Name
            }, offer);
        }

        public async Task SendAnswerAsync(TargetPeerDto targetPeerDto, string answer)
        {
            await Clients.Client(targetPeerDto.ConnectionId).SendAsync("ReceivedConnectionRequestAnswer", answer);
        }

        public async Task SendNoMediaDevicesAnswerAsync(TargetPeerDto targetPeerDto)
        {
            await Clients.Client(targetPeerDto.ConnectionId).SendAsync("ReceivedTargetPeerNoMediaAnswer");
        }

        public async Task TerminateConnectionAsync(TargetPeerDto targetPeerDto)
        {
            await Clients.Client(targetPeerDto.ConnectionId).SendAsync("ReceivedTerminateConnectionRequest");
        }

        public void ImNotAvailable()
        {
            var peerToRemove = _signalingHubConnectedClients.ConnectedClients.SingleOrDefault(p => p.ConnectionId == Context.ConnectionId);
            _signalingHubConnectedClients.ConnectedClients.Remove(peerToRemove);
        }

        public void ImAvailable()
        {
            _signalingHubConnectedClients.ConnectedClients.Add(new TargetPeerDto { ConnectionId = Context.ConnectionId, UserName = Context.User.Identity.Name });
        }

    }
}
