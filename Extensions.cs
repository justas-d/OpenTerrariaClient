using System;
using System.IO;
using System.Threading.Tasks;
using OpenTerrariaClient.Client;
using OpenTerrariaClient.Client.Service;
using OpenTerrariaClient.Model;
using OpenTerrariaClient.Model.ID;
using OpenTerrariaClient.Packet;
using StrmyCore;

namespace OpenTerrariaClient
{
    public static class Extensions
    {
        public static string Name(this GameItem item)
            => IdLookup.GetItem(item.Id);

        public static PacketEventService Packets(this TerrariaClient client)
            => client.Services.Get<PacketEventService>();

        public static T Add<T>(this ServiceManager serviceManager, params object[] args)
            where T : class, IService => serviceManager.Add((T) Activator.CreateInstance(typeof (T), args));

        public static void Send(this TerrariaClient client, TerrPacketType type, PacketWrapper packet)
            => client.Send(type, packet.CreatePayload());

        public static void Send(this TerrariaClient client, TerrPacketType type, byte[] payload)
            => client.Send(TerrPacket.Create(type, payload));

        public static void Send(this TerrariaClient client, TerrPacketType type) => client.Send(TerrPacket.Create(type));

        public static void Send(this TerrariaClient client, TerrPacketType type, string payload)
            => client.Send(TerrPacket.Create(type, payload));

        public static async Task ConnectAndLogin(this TerrariaClient client, string host, int port)
        {
            await client.Connect(host, port);
            client.Login();
        }

        public static void Write(this BinaryWriter writer, TerrColor color)
            => writer.WriteMany(color.R, color.G, color.B);

        public static void Write<T>(this BinaryWriter writer, ValPair<T> pair) where T : struct
            => writer.WriteMany(pair.Val1, pair.Val2);
    }
}
