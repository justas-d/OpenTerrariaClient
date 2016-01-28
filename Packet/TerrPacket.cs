using System;

namespace TerrariaBridge.Packet
{
    public class TerrPacket
    {
        private const byte Index_PacketId = 2;
        private const byte Index_Payload = 3;

        private const byte Size_Length = sizeof(ushort);
        private const byte Size_PacketId = sizeof(byte);

        public ushort Length { get; private set; }
        public TerrPacketType Type { get; private set; }
        public byte[] Payload { get; private set; }

        private TerrPacket()
        {

        }

        public static ushort GetSize(byte[] data) => BitConverter.ToUInt16(data, 0);
        public static TerrPacketType GetType(byte[] data) => (TerrPacketType) data[Index_PacketId];

        public static TerrPacket Parse(byte[] data, int length) => Parse(data, length, GetSize(data));

        public static TerrPacket Parse(byte[] data)
        {
            ushort packetSize = GetSize(data);
            return Parse(data, packetSize, packetSize);
        }

        private static TerrPacket Parse(byte[] data, int expectedLength, ushort packetProvidedLength)
        {
            if (expectedLength != packetProvidedLength)
            {
                if (GetType(data) == TerrPacketType.SendSection)
                {
                    Console.WriteLine("Dropped send section");
                    return null;
                }

                /*
                if we hit expectedLength > packetProvidedLength
                  that probably means there are more packets then one in the buffer,
                  go into a while loop like the one we see in netbuffer
                if its the opposite
                  probably means incomplete packet, not sure if this even happens
                */

                Console.WriteLine($"Expected != provided: {expectedLength} != {packetProvidedLength}");
                return null;
            }

            //if the lenght of the data array isin't the same as the one we got from the array data then it's probably a malformed packet.

            TerrPacket retval = new TerrPacket
            {
                Length = packetProvidedLength,
                Type = GetType(data)
            };

            //extract payload
            byte[] payloadBuffer = new byte[packetProvidedLength - Index_Payload];

            Buffer.BlockCopy(data, Index_Payload, payloadBuffer, 0, payloadBuffer.Length);
            retval.Payload = payloadBuffer;

            return retval;
        }

        /// <summary>
        /// Converts a payloadless packet to a packet with an empty payload of given size.
        /// </summary>
        /// <param name="payloadlessPacket">The packet without a payload which will be converted to a packet with an empty payload of given size.</param>
        /// <param name="payloadSize">The size of the payload which will be added to the given payloadless packet.</param>
        /// <returns>The new packet with an empty payload of given size.</returns>
        public static byte[] PayloadlessToPayloadPacket(byte[] payloadlessPacket, short payloadSize)
        {
            ushort packetLength = Convert.ToUInt16(payloadlessPacket.Length + payloadSize);
            byte[] packet = new byte[packetLength];

            //Copy over the stuff over from the payload less packet into our new one.
            Buffer.BlockCopy(payloadlessPacket, 0, packet, 0, payloadlessPacket.Length);

            //Set the new length
            Buffer.BlockCopy(BitConverter.GetBytes(packetLength), 0, packet, 0, Size_Length);

            return packet;
        }

        /// <summary>
        /// Creates bytes that represent a terraria client -> server (with payload) packet.
        /// </summary>
        /// <param name="type">The type of the packet.</param>
        /// <param name="payload">The payload, which we will be sending to the server.</param>
        /// <returns>A new byte array containing the bytes of the packet.</returns>
        public static byte[] Create(TerrPacketType type, byte[] payload)
        {
            byte[] packet = PayloadlessToPayloadPacket(Create(type), (short)payload.Length);

            //Set payload
            Buffer.BlockCopy(payload, 0, packet, Index_Payload, payload.Length);

            return packet;
        }

        /// <summary>
        /// Creates bytes that represent a payloadless terraria client -> server packet.
        /// </summary>
        /// <param name="type">The type of the packet.</param>
        /// <returns>A new byte array containing the bytes of the packet.</returns>
        public static byte[] Create(TerrPacketType type)
        {
            ushort packetLength = Size_Length + Size_PacketId;
            byte[] packet = new byte[packetLength];

            //set the length of the packet.
            Buffer.BlockCopy(BitConverter.GetBytes(packetLength), 0, packet, 0, Size_Length);

            //set the type
            packet[Index_PacketId] = (byte)type;

            return packet;
        }

        public static byte[] Create(TerrPacketType type, string payload)
            => Create(type, Utils.EncodeTerrString(payload));
    }
}
