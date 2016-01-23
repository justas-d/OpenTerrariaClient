using System;
using System.IO;
using System.Reflection;

namespace TerrariaBridge.Packet
{
    public class ModifyTileData
    {
        public byte Action { get; set; }
        public short TileX { get; set; }
        public short TileY { get; set; }
        public short Var1 { get; set; }
        public byte Var2 { get; set; }

        public ModifyTileData()
        {
            
        }

        public byte[] CreatePayload()
        {
            using (MemoryStream stream = new MemoryStream(new byte[Constants.BufferSize]))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    foreach (PropertyInfo prop in GetType().GetProperties())
                    {
                        if (prop.PropertyType == typeof(byte))
                            writer.Write((byte)prop.GetValue(this));
                        else if (prop.PropertyType == typeof(short))
                            writer.Write((short)prop.GetValue(this));

                        else throw new InvalidOperationException($"$Invalid property type {prop.PropertyType}.");
                    }
                    byte[] payload = new byte[stream.Position];
                    Buffer.BlockCopy(stream.ToArray(), 0, payload, 0, (int)stream.Position);

                    return payload;
                }
            }
        }
    }
}
