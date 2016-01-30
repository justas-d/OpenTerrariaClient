using System;
using System.IO;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
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
