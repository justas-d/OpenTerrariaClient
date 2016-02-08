using System;
using System.Collections;
using System.IO;
using System.Reflection;
using OpenTerrariaClient.Packet;
using StrmyCore;

namespace OpenTerrariaClient.Model
{
    public class KillMe : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public byte HitDirection { get; private set; }
        public short Damage { get; private set; }
        public bool WasPvP { get; private set; }
        public string DeathText { get; private set; }

        internal KillMe() { }

        public KillMe(byte pid,byte hitDir, short damage, bool pvp, string text)
        {
            PlayerId = pid;
            HitDirection = hitDir;
            Damage = damage;
            WasPvP = pvp;
            DeathText = text;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, HitDirection, Damage, WasPvP, DeathText);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.KillMe);

            PlayerId = reader.ReadByte();
            HitDirection = reader.ReadByte();
            Damage = reader.ReadInt16();
            WasPvP = reader.ReadBoolean();
            DeathText = reader.ReadString();
        }
    }

    public class PlayerAppearance : PacketWrapper
    {
        internal byte? PlayerId { get; set; }
        public byte SkinVarient { get; internal set; }
        public byte Hair { get; internal set; }
        public string Name { get; internal set; }
        public byte HairDye { get; internal set; }
        public byte HideVisuals1 { get; internal set; }
        public byte HideVisuals2 { get; internal set; }
        public byte HideMisc { get; internal set; }
        public TerrColor HairColor { get; internal set; }
        public TerrColor SkinColor { get; internal set; }
        public TerrColor EyeColor { get; internal set; }
        public TerrColor ShirtColor { get; internal set; }
        public TerrColor UnderShirtColor { get; internal set; }
        public TerrColor PantsColor { get; internal set; }
        public TerrColor ShoeColor { get; internal set; }
        public byte Difficulty { get; internal set; }

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
                if (prop.PropertyType == typeof(byte))
                    writer.Write((byte)prop.GetValue(this));

                else if (prop.PropertyType == typeof(TerrColor))
                    writer.Write(((TerrColor)prop.GetValue(this)).GetBytes());

                else if (prop.PropertyType == typeof(string))
                    writer.Write((string)prop.GetValue(this));
            }
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.PlayerAppearance);

            PlayerId = reader.ReadByte();

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(byte))
                    prop.SetValue(this, reader.ReadByte());

                else if (prop.PropertyType == typeof(TerrColor))
                    prop.SetValue(this, reader.ReadTerrColor());

                else if (prop.PropertyType == typeof(string))
                    prop.SetValue(this, reader.ReadString());
            }
        }
    }

    public sealed class PlayerTeam : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public Player.TeamType Team { get; private set; }

        internal PlayerTeam(byte pid, Player.TeamType team)
        {
            PlayerId = pid;
            Team = team;
        }

        internal PlayerTeam() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, (byte)Team);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.PlayerTeam);
            PlayerId = reader.ReadByte();
            Team = (Player.TeamType)reader.ReadByte();
        }
    }

    public sealed class HealOtherPlayer : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public short HealAmount { get; private set; }

        internal HealOtherPlayer() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, HealAmount);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.HealOtherPlayer);

            PlayerId = reader.ReadByte();
            HealAmount = reader.ReadInt16();
        }
    }

    public sealed class AddPlayerBuff : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public byte Buff { get; private set; }
        public short Time { get; private set; }

        internal AddPlayerBuff() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, Buff, Time);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.AddNpcBuff);

            PlayerId = reader.ReadByte();
            Buff = reader.ReadByte();
            Time = reader.ReadInt16();
        }
    }

    public sealed class TogglePvp : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public bool Value { get; private set; }

        internal TogglePvp() { }

        internal TogglePvp(byte pid, bool val)
        {
            PlayerId = pid;
            Value = val;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, Value);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.TogglePvp);

            PlayerId = reader.ReadByte();
            Value = reader.ReadBoolean();
        }
    }

    public sealed class PlayerDamage : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public byte HitDirecion { get; private set; }
        public short Damage { get; private set; }
        public string DeathText { get; private set; }
        public byte Flags { get; private set; }

        internal PlayerDamage() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, HitDirecion, Damage, DeathText, Flags);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.PlayerDamage);

            PlayerId = reader.ReadByte();
            HitDirecion = reader.ReadByte();
            Damage = reader.ReadInt16();
            DeathText = reader.ReadString();
            Flags = reader.ReadByte();
        }
    }

    public sealed class PlayerActive : PacketWrapper
    {
        internal byte PlayerId { get; private set; }
        internal bool Active { get; private set; }

        internal PlayerActive()
        {
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.PlayerActive);

            PlayerId = reader.ReadByte();
            Active = reader.ReadBoolean();
        }
    }

    public sealed class UpdatePlayer : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public byte Control { get; private set; }
        public byte Pulley { get; private set; }
        public byte SelectedItem { get; private set; }
        public ValPair<float> Position { get; private set; }
        public ValPair<float> Velocity { get; private set; }

        internal UpdatePlayer() { }

        internal UpdatePlayer(CurrentPlayer player)
        {
            PlayerId = player.PlayerId.Value;
            Control =
                new BitArray(new[]
                {
                    player.IsGoingUp, player.IsGoingDown,
                    player.IsGoingLeft, player.IsGoingRight, player.IsJumping,
                    player.IsUsingItem, player.Direction
                }).ConvertToByte(false);
            Pulley = player.PulleyFlags;
            SelectedItem = player.SelectedItem;
            Position = player.Position ?? new ValPair<float>(0, 0);
            Velocity = player.Velocity ?? new ValPair<float>(0, 0);
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, Control, Pulley, SelectedItem);
            writer.Write(Position);
            writer.Write(Velocity);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.UpdatePlayer);

            PlayerId = reader.ReadByte();
            Control = reader.ReadByte();
            Pulley = reader.ReadByte();
            SelectedItem = reader.ReadByte();
            Position = new ValPair<float>(reader);

            // the server doesn't send us the velocity if there is none, thus requiring us to skip reading it if position == lenght.
            if (reader.BaseStream.Position == reader.BaseStream.Length) return;
            Velocity = new ValPair<float>(reader);
        }
    }
}
