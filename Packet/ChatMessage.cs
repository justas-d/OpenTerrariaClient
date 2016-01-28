using System;
using System.IO;

namespace TerrariaBridge.Packet
{
    public class ChatMessage : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public TerrColor Color { get; private set; }
        public string Text { get; private set; }

        internal ChatMessage() { }

        internal ChatMessage(byte pid, TerrColor color, string message)
        {
            PlayerId = pid;
            Color = color;
            Text = message;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(Color.CreatePayload());
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
}
