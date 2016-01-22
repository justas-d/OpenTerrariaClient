using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public sealed class TerrPacketReceivedEventArgs : EventArgs
    {
        public TerrPacket Packet { get; }

        public TerrPacketReceivedEventArgs(TerrPacket packet)
        {
            Packet = packet;
        }
    }
}
