using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using StrmyCore;

namespace TerrariaBridge.Packet
{
    public sealed class UpdateItemDrop : PacketWrapper
    {
        public short ItemId { get; private set; }
        public ValPair<float> Position { get; private set; }
        public ValPair<float> Velocity { get; private set; }
        public short Stack { get; private set; }
        public byte Prefix { get; private set; }
        public byte NoDelay { get; private set; }
        public short ItemNetId { get; private set; }

        internal UpdateItemDrop() { }

        public UpdateItemDrop(short itemId, ValPair<float> position, short stack, byte prefix, byte noDelay, short itemNetId, ValPair<float> velocity = null)
        {
            ItemId = itemId;
            Position = position;
            Velocity = velocity ?? new ValPair<float>(0, 0);
            Stack = stack;
            Prefix = prefix;
            NoDelay = noDelay;
            ItemNetId = itemNetId;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(ItemId);
            writer.Write(Position);
            writer.Write(Velocity);
            writer.WriteMany(Stack, Prefix, NoDelay, ItemNetId);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            ItemId = reader.ReadByte();
            Position = new ValPair<float>(reader);
            Velocity = new ValPair<float>(reader);
            Stack = reader.ReadInt16();
            Prefix = reader.ReadByte();
            NoDelay = reader.ReadByte();
            ItemNetId = reader.ReadInt16();
        }
    }

    public sealed class HealOtherPlayer : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public short HealAmount { get; private set; }

        internal HealOtherPlayer() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId,HealAmount);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type,TerrPacketType.HealOtherPlayer);

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

    public sealed class SetActiveNpc : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public short NpcTalkTarget { get; private set; }

        internal SetActiveNpc() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, NpcTalkTarget);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.SetActiveNpc);

            PlayerId = reader.ReadByte();
            NpcTalkTarget = reader.ReadInt16();
        }
    }

    public sealed class TogglePvp : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public bool Value { get; private set; }

        internal TogglePvp() { }

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

    public sealed class DestroyProjectile : PacketWrapper
    {
        public short ProjectileId { get; private set; }
        public byte PlayerId { get; private set; }

        internal DestroyProjectile() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(ProjectileId, PlayerId);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.DestroyProjectile);

            ProjectileId = reader.ReadInt16();
            PlayerId = reader.ReadByte();
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

    public sealed class StrikeNpcWithHeldItem : PacketWrapper
    {
        public short NpcId { get; private set; }
        public byte PlayerId { get; private set; }

        internal StrikeNpcWithHeldItem() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(NpcId, PlayerId);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.StrikeNpcWithHeldItem);

            NpcId = reader.ReadInt16();
            PlayerId = reader.ReadByte();
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

    public sealed class Status : PacketWrapper
    {
        public int StatusMax { get; private set; }
        public string Text { get; private set; }

        internal Status() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.Statusbar);

            StatusMax = reader.ReadInt32();
            Text = reader.ReadString();
        }
    }

    public sealed class UpdateItemOwner : PacketWrapper
    {
        public short ItemId { get; private set; }
        public byte Owner { get; private set; }

        public UpdateItemOwner(short itemId, byte owner)
        {
            ItemId = itemId;
            Owner = owner;
        }

        internal UpdateItemOwner() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(ItemId, Owner);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if(type != TerrPacketType.UpdateItemOwner) throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.UpdateItemOwner}");

            ItemId = reader.ReadInt16();
            Owner = reader.ReadByte();
        }
    }

    public sealed class RemoveItemOwner : PacketWrapper
    {
        public short ItemIndex { get; private set; }

        internal RemoveItemOwner() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if(type != TerrPacketType.RemoveItemOwner) throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.RemoveItemOwner}");
            ItemIndex = reader.ReadInt16();
        }
    }

    public sealed class AnglerQuestsCompleted : PacketWrapper
    {
        public byte PlayerId { get; private set; }
        public int QuestsCompleted { get; private set; }

        internal AnglerQuestsCompleted() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.WriteMany(PlayerId, QuestsCompleted);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.NumberOfAnglerQuestsCompleted) throw new ArgumentException($"{nameof(type)} is not equal to {TerrPacketType.NumberOfAnglerQuestsCompleted}");

            PlayerId = reader.ReadByte();
            QuestsCompleted = reader.ReadInt32();
        }
    }

    public sealed class AnglerQuest : PacketWrapper
    {
        public byte Quest { get; private set; }
        public bool Completed { get; private set; }

        internal AnglerQuest() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.AnglerQuest) throw new ArgumentException($"{nameof(type)} is not equal to {TerrPacketType.AnglerQuest}");

            Quest = reader.ReadByte();
            Completed = reader.ReadBoolean();
        }
    }

    public sealed class TravellingMerchantInventory : PacketWrapper
    {
        public const byte Size = 40;

        public byte[] Items { get; private set; }

        internal TravellingMerchantInventory() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.TravellingMerchantInventory) throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.TravellingMerchantInventory}");

            for (byte i = 0; i < Size; i++)
            {
                reader.ReadByte(); // skip the inventory slot byte
                Items[i] = reader.ReadByte(); // and go straight to the item netid.
            }
        }
    }

    public sealed class ValPidPair<T> : ValPair<T> where T : struct
    {
        public byte? PlayerId { get; internal set; }

        internal ValPidPair() { }

        internal ValPidPair(ValPidPair<T> copy) : base(copy)
        {
            PlayerId = copy.PlayerId;
        }

        internal ValPidPair(T val1, T val2, byte pid) : base(val1, val2)
        {
            PlayerId = pid;
        }

        public ValPidPair(T val1, T val2) : base(val1, val2)
        {
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            if (PlayerId == null) throw new NullReferenceException($"{nameof(PlayerId)} doesn't have a value set.");

            writer.Write(PlayerId.Value);
            base.WritePayload(writer);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            PlayerId = reader.ReadByte();
            base.ReadPayload(reader, type);
        }
    }

    // can't have this inside of ValPair<T> because we don't want to instantiate SupportedTypes for each T used in ValPair<T>
    internal static class GenericByteConverter
    {
        private class ValueConverter
        {
            private  Action<BinaryWriter, object> WriteFunc { get; }
            private  Func<PayloadReader, object> ReadFunc { get; }

            public ValueConverter(Action<BinaryWriter, object> write, Func<PayloadReader, object> read)
            {
                WriteFunc = write;
                ReadFunc = read;
            }

            public TRead ReadValue<TRead>(PayloadReader reader) => (TRead)ReadFunc(reader);
            public void WriteValue<TWrite>(BinaryWriter writer, TWrite value) => WriteFunc(writer, value);
        }

        private static readonly Dictionary<Type, ValueConverter> SupportedTypes = new Dictionary
            <Type, ValueConverter>
        {
            {typeof (bool),    new ValueConverter((w, v) => w.Write((bool) v),    (r) => r.ReadBoolean())},
            {typeof (byte),    new ValueConverter((w, v) => w.Write((byte) v),    (r) => r.ReadByte())},
            {typeof (char),    new ValueConverter((w, v) => w.Write((char) v),    (r) => r.ReadChar())},
            {typeof (decimal), new ValueConverter((w, v) => w.Write((decimal) v), (r) => r.ReadDecimal())},
            {typeof (double),  new ValueConverter((w, v) => w.Write((double) v),  (r) => r.ReadDouble())},
            {typeof (float),   new ValueConverter((w, v) => w.Write((float) v),   (r) => r.ReadSingle())},
            {typeof (int),     new ValueConverter((w, v) => w.Write((int) v),     (r) => r.ReadInt32())},
            {typeof (long),    new ValueConverter((w, v) => w.Write((long) v),    (r) => r.ReadInt64())},
            {typeof (sbyte),   new ValueConverter((w, v) => w.Write((sbyte) v),   (r) => r.ReadSByte())},
            {typeof (short),   new ValueConverter((w, v) => w.Write((short) v),   (r) => r.ReadInt16())},
            {typeof (uint),    new ValueConverter((w, v) => w.Write((uint) v),    (r) => r.ReadUInt32())},
            {typeof (ulong),   new ValueConverter((w, v) => w.Write((ulong) v),   (r) => r.ReadUInt64())},
            {typeof (ushort),  new ValueConverter((w, v) => w.Write((ushort) v),  (r) => r.ReadUInt16())},
        };

        public static bool IsValidType(Type type) => SupportedTypes.ContainsKey(type);
        public static T Read<T>(PayloadReader reader) => SupportedTypes[typeof (T)].ReadValue<T>(reader);
        public static void Write<T>(BinaryWriter writer, T value) => SupportedTypes[typeof(T)].WriteValue(writer, value);
    }

    public class ValPair<T> : PacketWrapper where T : struct
    {
        private static readonly int TSize = Marshal.SizeOf(typeof (T));

        private T _val1;
        private T _val2;

        public T Val1 => _val1;
        public T Val2 => _val2;

        internal ValPair() { }

        internal ValPair(PayloadReader reader)
        {
            _val1 = GenericByteConverter.Read<T>(reader);
            _val2 = GenericByteConverter.Read<T>(reader);
        }

        internal ValPair(ValPair<T> copy)
        {
            _val1 = copy.Val1;
            _val2 = copy.Val2;
        }

        public ValPair(T val1, T val2)
        {
            _val1 = val1;
            _val2 = val2;
        }

        static ValPair()
        {
            if (!GenericByteConverter.IsValidType(typeof (T)))
                throw new NotSupportedException($"{typeof (T)} is not a supported ValPair type.");
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            GenericByteConverter.Write(writer, Val1);
            GenericByteConverter.Write(writer, Val2);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            _val1 = GenericByteConverter.Read<T>(reader);
            _val2 = GenericByteConverter.Read<T>(reader);
        }
    }

    public sealed class PlayerActive : PacketWrapper
    {
        internal byte PlayerId { get; private set; }
        internal bool Active { get; private set; }

        internal PlayerActive() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if(type != TerrPacketType.PlayerActive) throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.PlayerActive}");

            PlayerId = reader.ReadByte();
            Active = reader.ReadBoolean();
        }
    }
}
