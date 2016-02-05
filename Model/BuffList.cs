using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTerrariaClient.Packet;

namespace OpenTerrariaClient.Model
{
    public class BuffList : PacketWrapper
    {
        public const byte MaxBuffs = 22;

        internal byte? PlayerId { get; set; }

        internal byte[] InternalBuffs;

        ///<summary>Returns the buffs this player has. Use CurrentPlayer.SetBuffs to set buffs.</summary>
        public IEnumerable<byte> Buffs => InternalBuffs.Select(b => b);

        private BuffList() { }

        public BuffList(BuffList value)
        {
            PlayerId = value.PlayerId;

            InternalBuffs = new byte[MaxBuffs];
            for (byte i = 0; i < MaxBuffs; i++)
                InternalBuffs[i] = value.InternalBuffs[i];
        }

        public BuffList(byte[] buffs = null)
        {
            InternalBuffs = buffs?.Length == MaxBuffs ? buffs : new byte[MaxBuffs];
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            if (PlayerId == null) throw new NullReferenceException($"{nameof(PlayerId)} doesn't have a value set.");

            writer.Write(PlayerId.Value);
            for (byte i = 0; i < MaxBuffs; i++)
                writer.Write(InternalBuffs[i]);
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.UpdatePlayerBuff)
                throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.UpdatePlayerBuff}");

            if (InternalBuffs == null)
                InternalBuffs = new byte[MaxBuffs];

            PlayerId = reader.ReadByte();
            for (byte i = 0; i < MaxBuffs; i++)
                InternalBuffs[i] = reader.ReadByte();
        }
    }
}
