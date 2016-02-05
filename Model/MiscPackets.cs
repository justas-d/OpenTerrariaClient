using System;
using System.IO;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    public class ChatMessage : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public TerrColor Color { get; private set; }
        public string Text { get; internal set; }

        internal ChatMessage() { }

        internal ChatMessage(byte pid, string message)
        {
            PlayerId = pid;
            Text = message;
        }

        internal ChatMessage(byte pid, TerrColor color, string message) : this(pid, message)
        {
            Color = color;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Color.GetBytes());
            writer.Write(Text);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.ChatMessage) throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.ChatMessage}");

            PlayerId = reader.ReadByte();
            Color = reader.ReadTerrColor();
            Text = reader.ReadString();
        }
    }

    public sealed class Status : PacketWrapper
    {
        public int StatusMax { get; private set; }
        public string Text { get; private set; }

        internal Status()
        {
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.Statusbar);

            StatusMax = reader.ReadInt32();
            Text = reader.ReadString();
        }
    }
}
