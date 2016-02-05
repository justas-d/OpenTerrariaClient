using System;
using System.IO;
using System.Linq;

namespace OpenTerrariaClient.Packet
{
    public abstract class PacketWrapper
    {
        public byte[] CreatePayload()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    WritePayload(writer);
                }
                return stream.ToArray();
            }
        }

        public static T Parse<T>(TerrPacket packet) where T : PacketWrapper
        {
            using (PayloadReader reader = new PayloadReader(packet.Payload))
            {
                T retval = (T) Activator.CreateInstance(typeof (T), true);
                retval.ReadPayload(reader, packet.Type);
                return retval;
            }
        }

        protected abstract void WritePayload(BinaryWriter writer);
        protected abstract void ReadPayload(PayloadReader reader, TerrPacketType type);

        protected void CheckForValidType(TerrPacketType type, params TerrPacketType[] validTypes)
        {
            if (validTypes.Contains(type)) return;

            string exMsg = $"{nameof(type)} is not a ";
            throw new ArgumentException(validTypes.Aggregate(exMsg, (current, validType) => current + $"{validType};"));
        }
    }
}
