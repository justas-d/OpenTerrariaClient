using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public partial class TerrariaLisener : IDisposable
    {
        private Socket Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly ManualResetEvent _disconnectEvent = new ManualResetEvent(false);

        public TerrListenerConfig Config { get; }
        public bool IsLoggedIn { get; private set; }
        private bool _isLoggingIn;

        public byte PlayerId { get; private set; }

        public TerrariaLisener(TerrListenerConfig config = null)
        {
            Config = config ?? new TerrListenerConfig();
            Config.Lock();
        }

        public void ConnectAndLogin(string host, int port)
        {
            Connect(host, port);
            Login();
        }

        public void Connect(string host, int port)
        {
            if(Socket.Connected) throw new ArgumentException("You are already connected to a server.");

            _disconnectEvent.Reset();

            ManualResetEvent connectDone = new ManualResetEvent(false);

            Socket.BeginConnect(host, port, (ar) =>
            {
                Socket.EndConnect(ar);
                connectDone.Set();
            }, null);

            connectDone.WaitOne(Config.TimeoutMs);

            if (!Socket.Connected) throw new InvalidOperationException($"Failed connecting to {host}:{port}");
            OnConnected();

            BeginReceive();
        }

        public void Login()
        {
            if (!Socket.Connected)
                throw new InvalidOperationException("You first need to connect to the server if you want to login.");
            if (IsLoggedIn) throw new InvalidOperationException("You cannot log into a server two times.");
            if (_isLoggingIn) throw new InvalidOperationException("You cannot try to log in when already trying to log in.");

            _isLoggingIn = true;
            OnLoggedIn();

            PacketReceived += (s, e) =>
            {
                // login data
                if (_isLoggingIn && e.Packet.Type == TerrPacketType.ContinueConnecting)
                {
                    PlayerId = e.Packet.Payload[0];
                    Task.Run(() => SendLoginPackets());
                }
                if (e.Packet.Type == TerrPacketType.RemoveItemOwner)
                {
                    byte[] payload = new byte[sizeof(short) + 1];

                    // copy over the item id from the remove item owner packet.
                    Buffer.BlockCopy(e.Packet.Payload, 0, payload, 0, sizeof (short));

                    // set pid
                    payload[sizeof (short)] = 0xff;

                    // send an update item owner sync packet with the item id from the remove owner packet and a player id of 0xff.
                    Send(TerrPacket.Create(TerrPacketType.UpdateItemOwner, payload));
                }
            };

            Send(TerrPacket.ConnectPacket);
        }

        private void SendLoginPackets()
        {
            // player appearance. It's not required but it's good to know which player represents this client.
            Send(TerrPacket.Create(TerrPacketType.PlayerAppearance,
                new[] {PlayerId}.Concat(Config.Appearance.CreatePayload()).ToArray()));

            Send(TerrPacket.Create(TerrPacketType.RequestWorldInformation));

            Send(TerrPacket.Create(TerrPacketType.RequestInitialTileData, new byte[]
            {
                0xff, 0xff, 0xff, 0xff, // X (int32)
                0xff, 0xff, 0xff, 0xff // Y (int32)
            }));

            Send(TerrPacket.Create(TerrPacketType.SpawnPlayer, new byte[]
            {
                PlayerId,
                0xff, 0xff, //spawn x (int16)
                0xff, 0xff //spawn y (int16)
            }));
            _isLoggingIn = false;
        }

        private void BeginReceive()
        {
            try
            {
                if (!Socket.Connected) return;

                byte[] buffer = new byte[Constants.BufferSize];
                Socket.BeginReceive(buffer, 0, Constants.BufferSize, 0, ReceivePackets, buffer);
            }
            catch
            {
                Disconnect();
            }
        }

        private void ReceivePackets(IAsyncResult ar)
        {
            try
            {
                int bytesRead = Socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    byte[] data = new byte[bytesRead];
                    Array.Copy((byte[]) ar.AsyncState, data, bytesRead);

                    TerrPacket packet = TerrPacket.Parse(data);

                    if (packet != null)
                        Task.Run(() => OnPacketReceived(packet));
                }
                else
                    Disconnect();

                BeginReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when receiving packet: {ex}");
            }
        }

        ///<summary> Disconnects from the currently connected terraria server, allowing you to reuse this listener.</summary>
        public void Disconnect()
        {
            if (!Socket.Connected)
                throw new InvalidOperationException("You must be connected to a server to disconnect from it.");

            _disconnectEvent.Set();
            OnDisconnected();
            Socket.Disconnect(true);
            IsLoggedIn = false;
        }

        ///<summary> Blocking call and wait until the client has disconnected.</summary>
        public void Wait() => _disconnectEvent.WaitOne();

        ///<summary> Sends data to the connected server.</summary>
        public void Send(byte[] data)
        {
            if (!Socket.Connected)
                throw new InvalidOperationException("You must be connected to a server to send data to it.");
            Socket.BeginSend(data, 0, data.Length, 0, (ar) => Socket.EndSend(ar), null);
        }

        public void Dispose() => Socket.Dispose();
    }
}