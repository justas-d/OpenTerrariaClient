using System;

namespace TerrariaBridge.Packet
{
    public class TerrPacket
    {
        private const byte Index_Length = 0;
        private const byte Index_PacketId = 2;
        private const byte Index_Payload = 3;

        private const byte Size_Length = sizeof(ushort);
        private const byte Size_PacketId = sizeof(byte);

        public byte Length { get; private set; }
        public TerrPacketType Type { get; private set; }
        public byte[] Payload { get; private set; }

        private TerrPacket()
        {

        }

        public static TerrPacket Parse(byte[] data)
        {
            byte size = data[Index_Length];

            if (data.Length != size)
                return null;
            //if the lenght of the data array isin't the same as the one we got from the array data then it's probably a malformed packet.

            TerrPacket retval = new TerrPacket
            {
                Length = size,
                Type = (TerrPacketType)data[Index_PacketId]
            };

            //extract payload
            byte[] payloadBuffer = new byte[size - Index_Payload];
            Buffer.BlockCopy(data, Index_Payload, payloadBuffer, 0, payloadBuffer.Length);
            retval.Payload = payloadBuffer;

            return retval;
        }

        /// <summary>
        /// Creates bytes that represent a terraria client -> server (with payload) packet.
        /// </summary>
        /// <param name="type">The type of the packet.</param>
        /// <param name="payload">The payload, which we will be sending to the server.</param>
        /// <returns>A new byte array containing the bytes of the packet.</returns>
        public static byte[] Create(TerrPacketType type, byte[] payload)
        {
            //Create payloadless packet
            byte[] payloadLessPacket = Create(type);

            ushort packetLength = Convert.ToUInt16(payloadLessPacket.Length + payload.Length);
            byte[] packet = new byte[packetLength];

            //Copy over the stuff over from the payload less packet into our new one.
            Buffer.BlockCopy(payloadLessPacket, 0, packet, 0, payloadLessPacket.Length);

            //Set the new length
            Buffer.BlockCopy(BitConverter.GetBytes(packetLength), 0, packet, 0, Size_Length);

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
            Console.WriteLine($"Creating packet {type}");

            ushort packetLength = Size_Length + Size_PacketId;
            byte[] packet = new byte[packetLength];

            //set the length of the packet.
            Buffer.BlockCopy(BitConverter.GetBytes(packetLength), 0, packet, 0, Size_Length);

            //set the type
            packet[Index_PacketId] = (byte)type;

            return packet;
        }
    }
}
