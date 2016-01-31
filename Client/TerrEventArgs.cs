using System;
using TerrariaBridge.Model;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client
{
    public sealed class WorldEventBeginEventArgs : EventArgs
    {
        public short EventId { get; private set; }

        public WorldEventBeginEventArgs(short eventId)
        {
            EventId = eventId;
        }
    }

    public sealed class MessageReceivedEventArgs : EventArgs
    {
        public enum SenderType
        {
            Player,
            Server
        }

        public ChatMessage Message { get; }
        public SenderType Sender { get; }
        public Player Player { get; }

        public MessageReceivedEventArgs(ChatMessage msg, SenderType sender, Player player)
        {
            Message = msg;
            Sender = sender;
            Player = player;
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
    public sealed class StatusReceivedEventArgs : EventArgs
    {
        public Status Status { get; }

        public StatusReceivedEventArgs(Status status)
        {
            Status = status;
        }
    }
}
