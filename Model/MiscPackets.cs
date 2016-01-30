using System;
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

    public sealed class Status : PacketWrapper
    {
        public int StatusMax { get; private set; }
        public string Text { get; private set; }

        internal Status()
        {
        }

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
}
