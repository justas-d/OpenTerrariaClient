using System.IO;

namespace TerrariaBridge.Packet
{
    public struct PlayerItem
    {
        public short Stack { get; }
        public byte Prefix { get; }
        public short Id { get; }

        public PlayerItem(byte id = 0, short stack = 0, byte prefix = 0)
        {
            Id = id;
            Stack = stack;
            Prefix = prefix;
        }

        public byte[] CreatePayload()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(Stack);
                    writer.Write(Prefix);
                    writer.Write(Id);
                }
                return stream.ToArray();
            }
        }
    }
}
