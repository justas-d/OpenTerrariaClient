using System;
using System.IO;
using System.Text;

namespace TerrariaBridge.Packet
{
    public class ChatMessage
    {
        //public const int Index_MessageData = 5;

        public byte PlayerId { get; }
        public TerrColor Color { get;  }
        public string Message { get; }

        public ChatMessage(byte pid, TerrColor color, string message)
        {
            PlayerId = pid;
            Color = color;
            Message = message;
        }

        public byte[] CreatePayload()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(PlayerId);
                    writer.Write(Color.CreatePayload());
                    writer.Write(Message);
                }
                return stream.ToArray();
            }
        }

        public static ChatMessage Parse(TerrPacket packet)
        {
            if(packet.Type != TerrPacketType.ChatMessage) throw new ArgumentException($"{nameof(packet.Type)} is not {TerrPacketType.ChatMessage}");

            using (PayloadReader reader = new PayloadReader(packet.Payload))
            {
                return new ChatMessage(
                    reader.ReadByte(),
                    new TerrColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte()),
                    reader.ReadString());
            }
        }
    }
}
