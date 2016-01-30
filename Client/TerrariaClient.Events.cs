using System;
using TerrariaBridge.Model;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client
{
    public partial class TerrariaClient
    {
        public event EventHandler Connected = delegate { };
        public event EventHandler<DisconnectEventArgs> Disconnected = delegate { };
        public event EventHandler<LoggedInEventArgs> LoggedIn = delegate { };
        public event EventHandler<PacketReceivedEventArgs> PacketReceived = delegate { };
        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };
        public event EventHandler<StatusReceivedEventArgs> StatusReceived = delegate { };

        internal void OnConnected() => Connected(this, EventArgs.Empty);
        internal void OnDisconnected(string reason) => Disconnected(this, new DisconnectEventArgs(reason));
        internal void OnLoggedIn(byte pid) => LoggedIn(this, new LoggedInEventArgs(pid));

        internal void OnPacketReceived(TerrPacket packet)
        {
            if (TerrPacket.IsValidType(packet.Type))
                PacketReceived(this, new PacketReceivedEventArgs(packet));
        }

        internal void OnMessageReceived(ChatMessage msg, MessageReceivedEventArgs.SenderType sender)
            => MessageReceived(this, new MessageReceivedEventArgs(msg, sender, GetPlayer(msg.PlayerId)));

        internal void OnStatusReceived(Status status) => StatusReceived(this, new StatusReceivedEventArgs(status));
    }
}
