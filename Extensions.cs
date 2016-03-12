using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenTerrariaClient.Client;
using OpenTerrariaClient.Client.Service;
using OpenTerrariaClient.Model;
using OpenTerrariaClient.Model.ID;
using OpenTerrariaClient.Packet;

namespace OpenTerrariaClient
{
    public static class Extensions
    {
        private static readonly Dictionary<Type, MethodInfo> WriteManyValidMethods = new Dictionary<Type, MethodInfo>();

        // http://stackoverflow.com/questions/560123/convert-from-bitarray-to-byte
        public static byte ConvertToByte(this BitArray bits, bool mustBeEightBits = true)
        {
            if (mustBeEightBits)
                if (bits.Count != 8)
                    throw new ArgumentException("bits");

            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);

            return bytes[0];
        }

        /// <summary>
        /// Writes a list of objects to the BinaryWriter if the method which supports the objects type is found.
        /// </summary>
        public static void WriteMany(this BinaryWriter writer, params object[] objects)
        {
            Action<MethodInfo, object> invokeMethod = (m, o) => m.Invoke(writer, new[] { o });
            const string writePrefix = "Write";
            IEnumerable<MethodInfo> methods = writer.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (object obj in objects)
            {
                if (WriteManyValidMethods.ContainsKey(obj.GetType()))
                    invokeMethod(WriteManyValidMethods[obj.GetType()], obj);
                else
                {
                    // perform lookup of method which takes the "obj" type as the only paramater
                    MethodInfo writeMethod = methods.FirstOrDefault(m =>
                    {
                        if (!m.Name.StartsWith(writePrefix)) return false;

                        ParameterInfo[] parameters = m.GetParameters();
                        if (parameters.Length != 1) return false;
                        if (parameters.FirstOrDefault(param => param.ParameterType == obj.GetType()) == null)
                            return false;
                        return true;
                    });
                    if (writeMethod != null)
                    {
                        if (!WriteManyValidMethods.ContainsKey(obj.GetType()))
                            WriteManyValidMethods.Add(obj.GetType(), writeMethod);

                        invokeMethod(writeMethod, obj);
                    }
                    else
                        throw new InvalidOperationException(
                            $"Could not find method in BinaryWriter which could write obj type of {obj.GetType()}");

                }
            }
        }

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
