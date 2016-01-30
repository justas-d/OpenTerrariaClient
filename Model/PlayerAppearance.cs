using System;
using System.IO;
using System.Reflection;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    /// <summary>
    /// Defines a player appearance packet payload, not including the player id which should be provided before this data in the payload.
    /// </summary>
    public class PlayerAppearance : PacketWrapper
    {
        internal byte? PlayerId { get; set; }
        public byte SkinVarient { get; internal set; }
        public byte Hair  { get; internal set; }
        public string Name  { get; internal set; }
        public byte HairDye  { get; internal set; }
        public byte HideVisuals1  { get; internal set; }
        public byte HideVisuals2  { get; internal set; }
        public byte HideMisc  { get; internal set; }
        public TerrColor HairColor  { get; internal set; }
        public TerrColor SkinColor  { get; internal set; }
        public TerrColor EyeColor  { get; internal set; }
        public TerrColor ShirtColor  { get; internal set; }
        public TerrColor UnderShirtColor  { get; internal set; }
        public TerrColor PantsColor  { get; internal set; }
        public TerrColor ShoeColor  { get; internal set; }
        public byte Difficulty  { get; internal set; }

        internal PlayerAppearance(PlayerAppearance value)
        {
            PlayerId = value.PlayerId;
            SkinVarient = value.SkinVarient;
            Hair = value.Hair;
            Name = value.Name;
            HairDye = value.HairDye;
            HideVisuals1 = value.HideVisuals1;
            HideVisuals2 = value.HideVisuals2;
            HideMisc = value.HideMisc;
            HairColor = value.HairColor;
            SkinColor = value.SkinColor;
            EyeColor = value.EyeColor;
            ShirtColor = value.ShirtColor;
            UnderShirtColor = value.UnderShirtColor;
            PantsColor = value.PantsColor;
            ShoeColor = value.ShoeColor;
            Difficulty = value.Difficulty;
        }

        internal PlayerAppearance() { }

        public PlayerAppearance(string name = "Artificial Client")
        {
            Name = name;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            if (PlayerId == null) throw new NullReferenceException($"{nameof(PlayerId)} doesn't have a value set.");

            writer.Write(PlayerId.Value);

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (byte))
                    writer.Write((byte) prop.GetValue(this));

                else if (prop.PropertyType == typeof (TerrColor))
                    writer.Write(((TerrColor) prop.GetValue(this)).GetBytes());

                else if (prop.PropertyType == typeof (string))
                    writer.Write((string) prop.GetValue(this));
            }
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.PlayerAppearance)
                throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.PlayerAppearance}");

            PlayerId = reader.ReadByte();

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof (byte))
                    prop.SetValue(this, reader.ReadByte());

                else if (prop.PropertyType == typeof (TerrColor))
                    prop.SetValue(this, reader.ReadTerrColor());

                else if (prop.PropertyType == typeof (string))
                    prop.SetValue(this, reader.ReadString());
            }
        }
    }
}