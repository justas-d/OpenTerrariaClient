using System;
using System.IO;

namespace TerrariaBridge.Packet
{
    public class PlayerItem : PacketWrapper
    {
        internal byte? PlayerId { get; set; }
        internal byte? SlotId { get; set; }

        public short Stack { get; private set; }
        public byte Prefix { get; private set; }
        public short Id { get; private set; }

        internal PlayerItem() { }

        internal PlayerItem(PlayerItem value)
        {
            PlayerId = value.PlayerId;
            SlotId = value.SlotId;
            Stack = value.Stack;
            Prefix = value.Prefix;
            Id = value.Id;
        }

        public PlayerItem(short id = 0, short stack = 0, byte prefix = 0)
        {
            Id = id;
            Stack = stack;
            Prefix = prefix;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            if (PlayerId == null) throw new NullReferenceException($"{nameof(PlayerId)} doesn't have a value set.");
            if (SlotId == null) throw new NullReferenceException($"{nameof(SlotId)} doesn't have a value set.");

            writer.Write(PlayerId.Value);
            writer.Write(SlotId.Value);
            writer.Write(Stack);
            writer.Write(Prefix);
            writer.Write(Id);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.SetInventory) throw new ArgumentException(nameof(type));

            PlayerId = reader.ReadByte();
            SlotId = reader.ReadByte();
            Stack = reader.ReadInt16();
            Prefix = reader.ReadByte();
            Id = reader.ReadInt16();
        }
    }
}
