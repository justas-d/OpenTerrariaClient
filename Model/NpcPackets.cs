using System;
using System.Collections;
using System.IO;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    public sealed class NpcUpdate : PacketWrapper
    {
        public short NpcId { get; private set; }
        public ValPair<float> Position { get; private set; }
        public ValPair<float> Velocity { get; private set; }
        public byte TargetPlayerId { get; private set; }
        public BitArray Flags { get; private set; }
        public float[] Ai { get; private set; }
        public short NpcNetId { get; private set; }
        public int Life { get; private set; }
        public byte ReleaseOwner { get; private set; }

        internal NpcUpdate()
        {
            
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.NpcUpdate);

            NpcId = reader.ReadByte();
            Position = new ValPair<float>(reader);
            Velocity = new ValPair<float>(reader);
            TargetPlayerId = reader.ReadByte();
            Flags = new BitArray(new[] { reader.ReadByte() });

            /* 
               the size of the float[] Ai array is determined by ai flags at indexes 3, 4, 5, 6. 
               Each ai flag sets the ai array size to the previous size + 1. T
               he first one sets the size to 0, we ignore it.
            */

            byte aiSize = 0;
            if (Flags[3])
                aiSize = 1;
            if (Flags[4])
                aiSize = 2;
            if (Flags[5])
                aiSize = 3;

            if (aiSize > 0)
            {
                Ai = new float[aiSize];
                for (byte i = 0; i < aiSize; i++)
                    Ai[i] = reader.ReadSingle();
            }

            NpcNetId = reader.ReadInt16();
            Life = reader.ReadInt32();
            ReleaseOwner = reader.ReadByte();
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

        public GameItem[] Items { get; private set; }

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
                byte slot = reader.ReadByte();
                Items[i] = new GameItem(reader.ReadByte()) {SlotId = slot};
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
