namespace TerrariaBridge.Packet
{
    public struct TerrColor : IPayload
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public TerrColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte[] CreatePayload() => new[] { R, G, B };
    }
}
