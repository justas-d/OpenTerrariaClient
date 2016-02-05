using System.IO;

namespace OpenTerrariaClient
{
    public static class Utils
    {
        public static byte[] EncodeTerrString(string value)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(value);
                return stream.ToArray();
            }
        }
    }
}
