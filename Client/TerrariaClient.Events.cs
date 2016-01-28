using System;
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

        internal void OnConnected() => Connected(this, EventArgs.Empty);
        internal void OnDisconnected(string reason) => Disconnected(this, new DisconnectEventArgs(reason));
        internal void OnLoggedIn(byte pid) => LoggedIn(this, new LoggedInEventArgs(pid));
        internal void OnPacketReceived(TerrPacket packet) => PacketReceived(this, new PacketReceivedEventArgs(packet));
        internal void OnMessageReceived(ChatMessage msg) => MessageReceived(this, new MessageReceivedEventArgs(msg));
    }
}