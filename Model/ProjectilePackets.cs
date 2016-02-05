using System.Collections;
using System.Collections.Generic;
using System.IO;
using StrmyCore;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Model
{
    public sealed class DestroyProjectile : PacketWrapper
    {
        public short ProjectileId { get; private set; }
        public byte PlayerId { get; private set; }

        internal DestroyProjectile()
        {
        }

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

    ///<summary>Aka ProjectileUpdate</summary>
    public sealed class WorldProjectile : PacketWrapper
    {
        public const byte MaxAi = 2;

        public short UniqueId { get; private set; }
        public ValPair<float> Position { get; private set; }
        public ValPair<float> Velocity { get; private set; }
        public float Knockback { get; private set; }
        public short Damage { get; private set; }
        public byte OwnerPlayerId { get; private set; }
        public short Type { get; private set; }
        public BitArray AiFlags { get; private set; }
        public float[] Ai { get; private set; }

        internal WorldProjectile()
        {

        }

        public WorldProjectile(short uniqueId, ValPair<float> position, ValPair<float> velocity, float knockback, short damage, byte ownerPlayerId, short type, BitArray aiFlags, float[] ai = null)
        {
            UniqueId = uniqueId;
            Position = position;
            Velocity = velocity;
            Knockback = knockback;
            Damage = damage;
            OwnerPlayerId = ownerPlayerId;
            Type = type;
            Ai = ai;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            writer.Write(UniqueId);
            writer.Write(Position);
            writer.Write(Velocity);
            writer.WriteMany(Knockback, Damage, OwnerPlayerId, Type, AiFlags.ConvertToByte());

            if(Ai != null)
            foreach (float ai in Ai)
                writer.Write(ai);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.ProjectileUpdate);

            UniqueId = reader.ReadInt16();
            Position = new ValPair<float>(reader);
            Velocity = new ValPair<float>(reader);
            Knockback = reader.ReadSingle();
            Damage = reader.ReadInt16();
            OwnerPlayerId = reader.ReadByte();
            Type = reader.ReadInt16();
            AiFlags = new BitArray(new[] { reader.ReadByte() });

            List<float> aiBuilder = new List<float>();

            for (byte i = 0; i < MaxAi; i++)
                if (AiFlags[i])
                    aiBuilder.Add(reader.ReadSingle());

            Ai = aiBuilder.ToArray();
        }
    }
}
