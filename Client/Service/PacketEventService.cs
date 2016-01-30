using System;
using System.Collections.Generic;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client.Service
{
    public sealed class PacketEventService : IService
    {
        public delegate void OnPacketReceivedEvent(TerrPacket packet);

        private readonly Dictionary<TerrPacketType, OnPacketReceivedEvent> _packetEvents =
            new Dictionary<TerrPacketType, OnPacketReceivedEvent>();

        public void Install(TerrariaClient client)
        {
            client.PacketReceived += (s, e) =>
            {
                if (_packetEvents.ContainsKey(e.Packet.Type))
                    _packetEvents[e.Packet.Type](e.Packet);
            };
        }

        public bool ContainsEvent(TerrPacketType type) => _packetEvents.ContainsKey(type);

        /// <summary>
        /// Subscribes to a given PacketType event, which is called every time we receive a packet of that type.
        /// </summary>
        public void Subscribe(TerrPacketType type, OnPacketReceivedEvent evnt)
        {
            if (TerrPacket.IsValidType(type))
            {
                if (_packetEvents.ContainsKey(type))
                    _packetEvents[type] += evnt;
                else
                    _packetEvents.Add(type, evnt);
            }
            else
                throw new ArgumentException($"{type} is not a valid TerrPacketType type.");
        }

        public void SubscribeMany(OnPacketReceivedEvent evnt, params TerrPacketType[] types)
        {
            foreach(TerrPacketType type in types)
                Subscribe(type, evnt);
        }
    }
}
