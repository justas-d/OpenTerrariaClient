using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    public sealed class NpcHomeUpdate : PacketWrapper
    {
        public short UniqueNpcId { get; private set; }
        public short HomeTileX { get; private set; }
        public short HomeTileY { get; private set; }
        public bool IsHomeless { get; private set; }

        internal NpcHomeUpdate()
        {
        }

        public NpcHomeUpdate(short uniqueNpcId, short hometileX, short hometileY, bool isHomeless)
        {
            UniqueNpcId = uniqueNpcId;
            HomeTileX = hometileX;
            HomeTileY = hometileY;
            IsHomeless = isHomeless;
        }

        protected override void WritePayload(BinaryWriter writer)
            => writer.WriteMany(UniqueNpcId, HomeTileX, HomeTileY, IsHomeless);

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.NpcHomeUpdate);

            UniqueNpcId = reader.ReadInt16();
            HomeTileX = reader.ReadInt16();
            HomeTileY = reader.ReadInt16();
            IsHomeless = reader.ReadByte() != 0;
        }
    }

    public sealed class UpdateNpcName : PacketWrapper
    {
        public short UniqueNpcId { get; private set; }
        public string Name { get; private set; }

        internal UpdateNpcName() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.UpdateNpcName);

            UniqueNpcId = reader.ReadInt16();
            Name = reader.ReadString();
        }
    }

    ///<summary>Aka npc update</summary>
    public sealed class Npc : PacketWrapper
    {
        public short UniqueId { get; private set; }
        public ValPair<float> Position { get; private set; }
        public ValPair<float> Velocity { get; private set; }
        public byte TargetPlayerId { get; private set; }
        public BitArray Flags { get; private set; }
      //public float[] Ai { get; private set; }
        public short NpcId { get; private set; }
      //public int Life { get; private set; }
      //public byte ReleaseOwner { get; private set; }

        public IEnumerable<GameItem> Shop { get; internal set; }

        ///<summary>This is only set for home npcs, null for all others.</summary>
        public string Name { get; internal set; }

        public bool IsHomeless { get; internal set; }
        public short HomeTileX { get; internal set; }
        public short HomeTileY { get; internal set; }

        internal Npc() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.NpcUpdate);

            UniqueId = reader.ReadInt16();
            Position = new ValPair<float>(reader);
            Velocity = new ValPair<float>(reader);
            TargetPlayerId = reader.ReadByte();
            Flags = new BitArray(new[] {reader.ReadByte()});
            /* 
               the size of the float[] Ai array is determined by ai flags at indexes 3, 4, 5, 6. 
               Each ai flag sets the ai array size to the previous size + 1. T
               he first one sets the size to 0, we ignore it.
            
               doesn't seem to work for now

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

            NpcId = reader.ReadInt16();
            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Life = reader.ReadInt32();
                ReleaseOwner = reader.ReadByte();
            }
            */
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
