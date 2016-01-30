namespace TerrariaBridge.Model
{
    public struct TerrColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get;  }

        public TerrColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte[] GetBytes() => new[] {R, G, B};
    }
}
