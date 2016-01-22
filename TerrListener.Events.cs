using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public partial class TerrariaLisener
    {
        public event EventHandler Connected = delegate { };
        public event EventHandler Disconnected = delegate { };
        public event EventHandler LoggedIn = delegate { };
        public event EventHandler<TerrPacketReceivedEventArgs> PacketReceived = delegate { };

        private void OnConnected() => Connected(this, EventArgs.Empty);
        private void OnDisconnected() => Disconnected(this, EventArgs.Empty);
        private void OnLoggedIn() => LoggedIn(this, EventArgs.Empty);
        private void OnPacketReceived(TerrPacket packet)
            => PacketReceived(this, new TerrPacketReceivedEventArgs(packet));
    }
}