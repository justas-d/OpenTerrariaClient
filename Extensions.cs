using System;
using System.IO;
using StrmyCore;
using TerrariaBridge.Client;
using TerrariaBridge.Client.Service;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public static class Extensions
    {
        public static PacketEventService Packets(this TerrariaClient client)
            => client.Services.Get<PacketEventService>();

        public static T Add<T>(this ServiceManager seriveManager, params object[] args)
            where T : class, IService => seriveManager.Add((T) Activator.CreateInstance(typeof (T), args));

        public static void Send(this TerrariaClient client, TerrPacketType type, PacketWrapper packet)
            => client.Send(type, packet.CreatePayload());

        public static void Send(this TerrariaClient client, TerrPacketType type, byte[] payload)
            => client.Send(TerrPacket.Create(type, payload));

        public static void Send(this TerrariaClient client, TerrPacketType type) => client.Send(TerrPacket.Create(type));

        public static void Send(this TerrariaClient client, TerrPacketType type, string payload)
            => client.Send(TerrPacket.Create(type, payload));

        public static void ConnectAndLogin(this TerrariaClient client, string host, int port)
        {
            client.Connect(host, port);
            client.Login();
        }

        public static void Write(this BinaryWriter writer, TerrColor color)
            => writer.WriteMany(color.R, color.G, color.B);

        public static void Write<T>(this BinaryWriter writer, ValPair<T> pair) where T : struct
            => writer.WriteMany(pair.Val1, pair.Val2);
    }
}
