using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using OpenTerrariaClient.Packet;

namespace OpenTerrariaClient.Model
{
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
            private Action<BinaryWriter, object> WriteFunc { get; }
            private Func<PayloadReader, object> ReadFunc { get; }

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
        public static T Read<T>(PayloadReader reader) => SupportedTypes[typeof(T)].ReadValue<T>(reader);
        public static void Write<T>(BinaryWriter writer, T value) => SupportedTypes[typeof(T)].WriteValue(writer, value);
    }

    public class ValPair<T> : PacketWrapper where T : struct
    {
        private static readonly int TSize = Marshal.SizeOf(typeof(T));

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
            if (!GenericByteConverter.IsValidType(typeof(T)))
                throw new NotSupportedException($"{typeof(T)} is not a supported ValPair type.");
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
}
