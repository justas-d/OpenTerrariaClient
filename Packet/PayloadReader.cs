using System.IO;

namespace TerrariaBridge.Packet
{
    public class PayloadReader : BinaryReader
    {
        public PayloadReader(byte[] payload) : base( new MemoryStream(payload)) { }

        public TerrColor ReadTerrColor() => new TerrColor(ReadByte(), ReadByte(), ReadByte());
    }
}
