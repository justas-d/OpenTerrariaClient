using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public partial class TerrariaLisener
    {
        public event EventHandler Connected = delegate { };
        public event EventHandler<TerrDisconnectEventArgs> Disconnected = delegate { };
        public event EventHandler<TerrLoggedInEventArgs> LoggedIn = delegate { };
        public event EventHandler<TerrPacketReceivedEventArgs> PacketReceived = delegate { };

        private void OnConnected() => Connected(this, EventArgs.Empty);
        private void OnDisconnected(string reason) => Disconnected(this, new TerrDisconnectEventArgs(reason));
        private void OnLoggedIn(byte pid) => LoggedIn(this, new TerrLoggedInEventArgs(pid));
        private void OnPacketReceived(TerrPacket packet) => PacketReceived(this, new TerrPacketReceivedEventArgs(packet));
    }
}