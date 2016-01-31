using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    [DebuggerDisplay("Id = {Id}")]
    public sealed class GameItem : PacketWrapper
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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("[i");

            if (Prefix != 0)
                builder.Append($"/p{Prefix}");

            if (Stack > 1)
                builder.AppendLine($"/s{Stack}");

            return $"{builder}:{Id}]";
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
            CheckForValidType(type, TerrPacketType.UpdateItemOwner);

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
            CheckForValidType(type, TerrPacketType.RemoveItemOwner);

            ItemIndex = reader.ReadInt16();
        }
    }

    ///<summary>Aka UpdateItemDrop</summary>
    public sealed class WorldItem : PacketWrapper
    {
        ///<summary>The unique id for this world item. It is not equal to Item.Id</summary>
        public short UniqueId { get; private set; }
        public ValPair<float> Position { get; private set; }
        public ValPair<float> Velocity { get; private set; }
        public GameItem Item { get; private set; }
        public byte NoDelay { get; private set; }
        public byte Owner { get; internal set; }

        internal WorldItem() { }

        public WorldItem(GameItem item, short itemId, ValPair<float> position, byte noDelay,  ValPair<float> velocity = null)
        {
            UniqueId = itemId;
            Position = position;
            Velocity = velocity ?? new ValPair<float>(0, 0);
            NoDelay = noDelay;
            Item = item;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(UniqueId);
            writer.Write(Position);
            writer.Write(Velocity);
            writer.WriteMany(Item.Stack, Item.Prefix, NoDelay, Item.Id);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.UpdateItemDrop, TerrPacketType.UpdateItemDrop2);

            Item = new GameItem();
            UniqueId = reader.ReadInt16();
            Position = new ValPair<float>(reader);
            Velocity = new ValPair<float>(reader);
            Item.Stack = reader.ReadInt16();
            Item.Prefix = reader.ReadByte();
            NoDelay = reader.ReadByte();
            Item.Id = reader.ReadInt16(); // todo : we might be reading something wrong here
        }
    }
}
