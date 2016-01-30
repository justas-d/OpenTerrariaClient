using System;
using System.IO;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    public class GameItem : PacketWrapper
    {
        internal byte? PlayerId { get; set; }
        internal byte? SlotId { get; set; }

        public short Stack { get; internal set; }
        public byte Prefix { get; internal set; }
        public short Id { get; internal set; }

        internal GameItem() { }

        internal GameItem(GameItem value)
        {
            PlayerId = value.PlayerId;
            SlotId = value.SlotId;
            Stack = value.Stack;
            Prefix = value.Prefix;
            Id = value.Id;
        }

        public GameItem(short id = 0, short stack = 0, byte prefix = 0)
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
