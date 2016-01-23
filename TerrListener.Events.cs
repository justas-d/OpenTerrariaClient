using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public partial class TerrariaLisener
    {
        public event EventHandler Connected = delegate { };
        public event EventHandler<DisconnectEventArgs> Disconnected = delegate { };
        public event EventHandler<LoggedInEventArgs> LoggedIn = delegate { };
        public event EventHandler<PacketReceivedEventArgs> PacketReceived = delegate { };
        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };

        private void OnConnected() => Connected(this, EventArgs.Empty);
        private void OnDisconnected(string reason) => Disconnected(this, new DisconnectEventArgs(reason));
        private void OnLoggedIn(byte pid) => LoggedIn(this, new LoggedInEventArgs(pid));
        private void OnPacketReceived(TerrPacket packet) => PacketReceived(this, new PacketReceivedEventArgs(packet));
        private void OnMessageReceived(ChatMessage msg) => MessageReceived(this, new MessageReceivedEventArgs(msg));
    }
}