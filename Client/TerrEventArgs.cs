using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        public ChatMessage Message { get; }

        public MessageReceivedEventArgs(ChatMessage msg)
        {
            Message = msg;
        }
    }

    public sealed class PacketReceivedEventArgs : EventArgs
    {
        public TerrPacket Packet { get; }

        public PacketReceivedEventArgs(TerrPacket packet)
        {
            Packet = packet;
        }
    }

    public sealed class DisconnectEventArgs : EventArgs
    {
        public string Reason { get;}

        public DisconnectEventArgs(string reason)
        {
            Reason = reason;
        }
    }

    public sealed class LoggedInEventArgs : EventArgs
    {
        public byte PlayerId { get; }

        public LoggedInEventArgs(byte pid)
        {
            PlayerId = pid;
        }
    }
}
