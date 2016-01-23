using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public partial class TerrariaLisener : IDisposable
    {
        public const string TerrariaVersion = "Terraria156";

        private Socket Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly ManualResetEvent _disconnectEvent = new ManualResetEvent(false);

        public TerrListenerConfig Config { get; }
        public bool IsLoggedIn { get; private set; }
        private bool _isLoggingIn;

        public byte PlayerId { get; private set; }

        public TerrariaLisener(TerrListenerConfig config = null)
        {
            Config = config ?? new TerrListenerConfig();
        }

        public void ConnectAndLogin(string host, int port, string password = null)
        {
            Connect(host, port);
            Login(password);
        }

        public void Connect(string host, int port)
        {
            if (Socket.Connected) throw new ArgumentException("You are already connected to a server.");

            _disconnectEvent.Reset();

            ManualResetEvent connectDone = new ManualResetEvent(false);

            Socket.BeginConnect(host, port, (ar) =>
            {
                Socket.EndConnect(ar);
                connectDone.Set();
            }, null);

            connectDone.WaitOne(Config.TimeoutMs);

            if (!Socket.Connected) throw new ArgumentException($"Failed connecting to {host}:{port}");
            OnConnected();

            BeginReceive();
        }

        public void Login(string password = null)
        {
            if (!Socket.Connected)
                throw new InvalidOperationException("You first need to connect to the server if you want to login.");
            if (IsLoggedIn) throw new InvalidOperationException("You cannot log into a server two times.");
            if (_isLoggingIn)
                throw new InvalidOperationException("You cannot try to log in when already trying to log in.");

            _isLoggingIn = true;

            LoggedIn += (s, e) => PlayerId = e.PlayerId;

            PacketReceived += (s, e) =>
            {
                switch (e.Packet.Type)
                {
                    case TerrPacketType.ContinueConnecting:
                        if (_isLoggingIn)
                        {
                            OnLoggedIn(e.Packet.Payload[0]);
                            Task.Run(() => SendLoginPackets());
                        }
                        break;

                    case TerrPacketType.RemoveItemOwner:
                        byte[] payload = new byte[sizeof (short) + 1];

                        // copy over the item id from the remove item owner packet.
                        Buffer.BlockCopy(e.Packet.Payload, 0, payload, 0, sizeof (short));

                        // set pid
                        payload[sizeof (short)] = 0xff;

                        // send an update item owner sync packet with the item id from the remove owner packet and a player id of 0xff.
                        Send(TerrPacket.Create(TerrPacketType.UpdateItemOwner, payload));
                        break;

                    case TerrPacketType.Disconnect:
                        // terraria transfers its strings prefixed with a length.
                        // we don't need that length so lets get rid of it here.
                        byte[] stringData = new byte[e.Packet.Payload.Length - 1];
                        Buffer.BlockCopy(e.Packet.Payload, 1, stringData, 0, stringData.Length);
                        SetDisconnectState(Encoding.ASCII.GetString(stringData));
                        break;

                    case TerrPacketType.RequestPassword:
                        if (string.IsNullOrEmpty(password))
                            throw new ArgumentNullException(password);

                        Send(TerrPacket.Create(TerrPacketType.SendPassword, Utils.EncodeTerrString(password)));

                        break;

                    case TerrPacketType.ChatMessage:
                        OnMessageReceived(ChatMessage.Parse(e.Packet));
                        break;
                }
            };
            Send(TerrPacket.Create(TerrPacketType.ConnectRequest, Utils.EncodeTerrString(TerrariaVersion)));
        }

        private void SendLoginPackets()
        {
            // player appearance. It's not required but it's good to know which player represents this client.
            Send(TerrPacket.Create(TerrPacketType.PlayerAppearance, Config.PlayerData.Appearance.CreatePayload(PlayerId)));

            #region Optional login packets

            if (Config.PlayerData.PlayerGuid != null)
                Send(TerrPacket.Create(TerrPacketType.ClientUuid,
                    Utils.EncodeTerrString(Config.PlayerData.PlayerGuid.Value.ToString())));

            if (Config.PlayerData.CurrentLife != null &&
                Config.PlayerData.MaxLife != null)
            {
                byte[] payload = new byte[sizeof (byte) + (sizeof (short)*2)];

                // set pid
                payload[0] = PlayerId;

                // set current life
                Buffer.BlockCopy(BitConverter.GetBytes(Config.PlayerData.CurrentLife.Value), 0, payload, sizeof (byte),
                    sizeof (short));

                // copy max life
                Buffer.BlockCopy(BitConverter.GetBytes(Config.PlayerData.MaxLife.Value), 0, payload,
                    sizeof (byte) + sizeof (short), sizeof (short));

                Send(TerrPacket.Create(TerrPacketType.PlayerLife, payload));
            }

            if (Config.PlayerData.CurrentMana != null &&
                Config.PlayerData.MaxMana != null)
            {
                byte[] payload = new byte[sizeof (byte) + (sizeof (short)*2)];

                // set pid
                payload[0] = PlayerId;

                // set current mana
                Buffer.BlockCopy(BitConverter.GetBytes(Config.PlayerData.CurrentMana.Value), 0, payload, sizeof (byte),
                    sizeof (short));

                // copy max mana
                Buffer.BlockCopy(BitConverter.GetBytes(Config.PlayerData.MaxMana.Value), 0, payload,
                    sizeof (byte) + sizeof (short), sizeof (short));

                Send(TerrPacket.Create(TerrPacketType.PlayerMana, payload));
            }

            if (Config.PlayerData.Buffs != null)
                Send(TerrPacket.Create(TerrPacketType.UpdatePlayerBuff,
                    new[] {PlayerId}.Concat(Config.PlayerData.Buffs).ToArray()));

            if (Config.PlayerData.Inventory != null)
            {
                for (byte i = 0; i < PlayerInventory.InventorySize; i++)
                    Send(TerrPacket.Create(TerrPacketType.SetInventory,
                        Config.PlayerData.Inventory.CreatePayload(PlayerId, i)));
            }

            #endregion

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
            catch (SocketException ex)
            {
                SetDisconnectState($"SocketException: {ex}");
            }
            catch (ObjectDisposedException)
            {
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
            catch (SocketException ex)
            {
                SetDisconnectState($"SocketException: {ex}");
            }
            catch (ObjectDisposedException)
            {
            }
        }

        ///<summary> Disconnects from the currently connected terraria server, allowing you to reuse this listener.</summary>
        public void Disconnect()
        {
            if (!Socket.Connected)
                throw new InvalidOperationException("You must be connected to a server to disconnect from it.");

            Socket.Disconnect(true);
            SetDisconnectState("Disconnect() called.");
        }

        private void SetDisconnectState(string reason)
        {
            _disconnectEvent.Set();
            OnDisconnected(reason);
            IsLoggedIn = false;
        }

        ///<summary> Blocking call and wait until the client has disconnected.</summary>
        public void Wait() => _disconnectEvent.WaitOne();

        ///<summary> Sends data to the connected server.</summary>
        public void Send(byte[] data)
        {
            try
            {
                if (!Socket.Connected)
                    throw new InvalidOperationException("You must be connected to a server to send data to it.");
                Socket.BeginSend(data, 0, data.Length, 0, (ar) => Socket.EndSend(ar), null);
            }
            catch (SocketException ex)
            {
                SetDisconnectState($"SocketException: {ex}");
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Dispose() => Socket.Dispose();
    }
}