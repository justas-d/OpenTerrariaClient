using System;
using System.IO;
using System.Reflection;

namespace TerrariaBridge.Packet
{
    /// <summary>
    /// Defines a player appearance packet payload, not including the player id which should be provided before this data in the payload.
    /// </summary>
    public class PlayerAppearanceData : IPayload
    {
        public byte SkinVarient { get; set; }
        public byte Hair { get; set; }
        public string Name { get; set; }
        public byte HairDye { get; set; }
        public byte HideVisuals1 { get; set; }
        public byte HideVisuals2 { get; set; }
        public byte HideMisc { get; set; }
        public TerrColor HairColor { get; set; }
        public TerrColor SkinColor { get; set; }
        public TerrColor EyeColor { get; set; }
        public TerrColor ShirtColor { get; set; }
        public TerrColor UnderShirtColor { get; set; }
        public TerrColor PantsColor { get; set; }
        public TerrColor ShoeColor { get; set; }
        public byte Difficulty { get; set; }

        public PlayerAppearanceData(string name = "Artificial Client")
        {
            Name = name;
        }

        public byte[] CreatePayload()
        {
            using (MemoryStream stream = new MemoryStream(new byte[Constants.BufferSize]))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    foreach (PropertyInfo prop in GetType().GetProperties())
                    {
                        if (prop.PropertyType == typeof (byte))
                            writer.Write((byte) prop.GetValue(this));

                        else if (prop.PropertyType == typeof (TerrColor))
                            writer.Write(((TerrColor) prop.GetValue(this)).CreatePayload());

                        else if (prop.PropertyType == typeof (string))
                            writer.Write((string) prop.GetValue(this));

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