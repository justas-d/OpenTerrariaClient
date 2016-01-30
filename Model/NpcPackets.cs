using System;
using System.IO;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
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
            CheckForValidType(type, TerrPacketType.NumberOfAnglerQuestsCompleted);

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
            CheckForValidType(type, TerrPacketType.AnglerQuest);

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
            CheckForValidType(type, TerrPacketType.TravellingMerchantInventory);

            for (byte i = 0; i < Size; i++)
            {
                reader.ReadByte(); // skip the inventory slot byte
                Items[i] = reader.ReadByte(); // and go straight to the item netid.
            }
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
}
