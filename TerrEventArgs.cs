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

    public sealed class TerrDisconnectEventArgs : EventArgs
    {
        public string Reason { get;}

        public TerrDisconnectEventArgs(string reason)
        {
            Reason = reason;
        }
    }

    public sealed class TerrLoggedInEventArgs : EventArgs
    {
        public byte PlayerId { get; }

        public TerrLoggedInEventArgs(byte pid)
        {
            PlayerId = pid;
        }
    }
}
