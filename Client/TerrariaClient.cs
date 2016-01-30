using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using TerrariaBridge.Client.Service;
using TerrariaBridge.Model;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client
{
    public partial class TerrariaClient : IDisposable
    {
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly ManualResetEvent _disconnectEvent = new ManualResetEvent(false);
        private readonly ConcurrentDictionary<byte, Player> _players = new ConcurrentDictionary<byte, Player>();
        private readonly ConcurrentDictionary<short, WorldItem> _items = new ConcurrentDictionary<short, WorldItem>();

        private const int BufferSize = 0x1FFFE;
        public const byte ServerPlayerId = byte.MaxValue;

        private Player ServerDummyPlayer => new Player(new PlayerAppearance("Server")) {PlayerId = ServerPlayerId};

        public Logger Log { get; } = new Logger();

        ///<summary>Returns the configuration data used for this client.</summary>
        public TerrariaClientConfig Config { get; internal set; }
        ///<summary>Returns the service manager for this client.</summary>
        public ServiceManager Services { get; private set; }

        ///<summary>Returns whether this client is logged into a server</summary>
        public bool IsLoggedIn { get; internal set; }
        ///<summary>Returns whether this client is connected to a server.</summary>
        public bool IsConnected => _socket.Connected;

        ///<summary>Returns a list of currently online players on the server.</summary>
        public IEnumerable<Player> Players => _players.Values;
        ///<summary>Returns the player that the bot appears as.</summary>
        public CurrentPlayer CurrentPlayer { get; private set; }
        ///<summary>Returns the latest information about the terraria world the server provided to the bot.</summary>
        public WorldInfo World { get; internal set; }

        internal bool IsLoggingIn;

        public TerrariaClient(TerrariaClientConfig config = null)
        {
            Config = config ?? new TerrariaClientConfig();
            FinishConstruction();
        }

        public TerrariaClient(Action<TerrariaClientConfigBuilder> builder)
        {
            builder(new TerrariaClientConfigBuilder(Config = new TerrariaClientConfig()));
            FinishConstruction();
        }

        private void FinishConstruction()
        {
            Services = new ServiceManager(this);

            Services.Add<PacketEventService>();
            Services.Add<InternalPacketManagerService>();
        }

        public void Connect(string host, int port)
        {
            if (_socket.Connected) throw new ArgumentException("You are already connected to a server.");

            _disconnectEvent.Reset();

            ManualResetEvent connectDone = new ManualResetEvent(false);

            _socket.BeginConnect(host, port, (ar) =>
            {
                _socket.EndConnect(ar);
                connectDone.Set();
            }, null);

            connectDone.WaitOne(Config.TimeoutMs);

            if (!_socket.Connected) throw new ArgumentException($"Failed connecting to {host}:{port}");
            OnConnected();

            BeginReceive();
        }

        public void Login()
        {
            if (!_socket.Connected)
                throw new InvalidOperationException("You first need to connect to the server if you want to login.");
            if (IsLoggedIn) throw new InvalidOperationException("You cannot log into a server two times.");
            if (IsLoggingIn)
                throw new InvalidOperationException("You cannot try to log in when already trying to log in.");

            IsLoggingIn = true;

            LoggedIn += (s, e) =>
            {
                CurrentPlayer = new CurrentPlayer(Config.PlayerData)
                {
                    PlayerId = e.PlayerId,
                    Client = this,
                    Guid = Config.PlayerGuid
                };
            };

            this.Send(TerrPacketType.ConnectRequest, Config.TerrariaVersion);
        }

        #region Player

        internal bool RemovePlayer(byte pid)
        {
            if (!_players.ContainsKey(pid)) return false;

            Player ignored;
            _players.TryRemove(pid, out ignored);

            Log.Warning($"Disconnected: {pid}");
            return true;
        }

        internal Player RegisterPlayer(byte pid)
        {
            if (pid == ServerPlayerId) return null;
            if (_players.ContainsKey(pid)) return null; // dont register a player if we contain it
            if (CurrentPlayer.PlayerId == pid) return null; // dont register ourselves

            Player player = new Player(pid, this);
            _players.TryAdd(pid, player);

            Log.Info($"Connected: {pid}");
            return player;
        }

        public Player GetExistingPlayer(byte playerId)
        {
            if (!IsLoggedIn) throw new InvalidOperationException("You need to be logged in.");

            if (CurrentPlayer.PlayerId == playerId)
                return CurrentPlayer;

            if (playerId == ServerPlayerId) return ServerDummyPlayer;

            if (!_players.ContainsKey(playerId)) return null;

            Player retval;
            _players.TryGetValue(playerId, out retval);
            return retval;
        }

        internal Player GetPlayer(byte pid) => GetExistingPlayer(pid) ?? RegisterPlayer(pid);

        #endregion

        #region Item

        internal void UpdateItemOwner(short id, byte owner)
        {
            WorldItem item = GetExistingItem(id);

            if (item != null)
            {
                item.Owner = owner;
                this.Send(TerrPacketType.UpdateItemOwner, new UpdateItemOwner(id, owner));
            }
        }

        internal void OverwriteItem(WorldItem item)
        {
            RemoveItem(item.UniqueId, false);
            RegisterItem(item.UniqueId, item, false);
           // this.Send(TerrPacketType.UpdateItemDrop, item);
        }

        internal bool RemoveItem(short id, bool log = true)
        {
            if (_items.ContainsKey(id)) return false;

            WorldItem ignored;
            _items.TryRemove(id, out ignored);

            if(log)
                Log.Info($"Removed item id {id}");
            return false;
        }

        internal bool RegisterItem(WorldItem item) => RegisterItem(item.UniqueId, item);

        private bool RegisterItem(short id, WorldItem item, bool log = true)
        {
            if (_items.ContainsKey(id)) return false;

            _items.TryAdd(id, item);

            if(log)
                Log.Info($"Registered {item.Item.Id} as id {id}");
            return true;
        }

        public WorldItem GetExistingItem(short id)
        {
            if (!_items.ContainsKey(id)) return null;

            WorldItem retval;
            _items.TryGetValue(id, out retval);
            return retval;
        }

        internal WorldItem GetItem(WorldItem item)
            => GetItem(item.UniqueId, item);

        private WorldItem GetItem(short id, WorldItem item)
        {
            WorldItem existing = GetExistingItem(id);
            if (existing != null)
                return existing;

            RegisterItem(item);
            return item;
        }

        #endregion

        #region Socket

        //<summary> Sends data to the connected server.</summary>
        public void Send(byte[] data)
        {
            try
            {
                if (!_socket.Connected)
                    throw new InvalidOperationException("You must be connected to a server to send data to it.");
                _socket.BeginSend(data, 0, data.Length, 0, (ar) =>
                {
                    try
                    {
                        _socket.EndSend(ar);
                    }
                    catch (SocketException ex)
                    {
                        SetDisconnectState($"SocketException: {ex}");
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    // the catch below doesn't apply for this lambda for some reason
                }, null);
            }
            catch (SocketException ex)
            {
                SetDisconnectState($"SocketException: {ex}");
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void BeginReceive(object state = null)
        {
            try
            {
                if (!_socket.Connected) throw new InvalidOperationException();

                byte[] incompletePacketBuffer = null;
                int incompletePacketsLength = 0;

                byte[] buffer = new byte[BufferSize];
                int packetBufferOffset = 0;

                Tuple<byte[], int> tuple = state as Tuple<byte[], int>;

                if (tuple != null)
                {
                    incompletePacketBuffer = tuple.Item1;
                    incompletePacketsLength = tuple.Item2;
                }

                if (incompletePacketBuffer != null)
                {
                    Buffer.BlockCopy(incompletePacketBuffer, 0, buffer, 0, incompletePacketBuffer.Length);
                    packetBufferOffset = incompletePacketsLength;
                    incompletePacketBuffer = null;
                }

                _socket.BeginReceive(buffer, packetBufferOffset, BufferSize, SocketFlags.None, (ar) =>
                {
                    int bytesReceived = _socket.EndReceive(ar);

                    if (bytesReceived <= 0)
                    {
                        Disconnect("No bytes received");
                        return;
                    }

                    ushort packetProvidedLength = TerrPacket.GetSize(buffer);

                    if (bytesReceived >= packetProvidedLength)
                    {
                        // one or more packets in buffer
                        using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer, 0, bytesReceived)))
                        {
                            while (reader.BaseStream.Position <= reader.BaseStream.Length - sizeof (ushort))
                            {
                                ushort packetLength = reader.ReadUInt16();
                                if (packetLength <= 0)
                                {
                                    Log.Warning($"Corrupted packetbuffer, read packet length of {packetLength}");
                                    break;
                                }
                                reader.BaseStream.Position -= sizeof (ushort);

                                byte[] packetBuffer = reader.ReadBytes(packetLength);

                                if (packetBuffer.Length != packetLength)
                                {
                                    TerrPacketType type = TerrPacket.GetType(packetBuffer);
                                    incompletePacketsLength += packetBuffer.Length;
                                    Log.Info(
                                        $"Incomplete packet in packetBuffer. Type {type} Sizes: expected {packetLength} actual: {packetBuffer.Length} Adding to incomplete packet buffer");

                                    if (type == TerrPacketType.SendSection)
                                    {
                                        Log.Info("Dropped send secion packet.");
                                        continue;
                                    }

                                    if (incompletePacketBuffer == null)
                                        incompletePacketBuffer = new byte[BufferSize];

                                    Buffer.BlockCopy(packetBuffer, 0, incompletePacketBuffer,
                                        incompletePacketsLength, packetBuffer.Length);
                                    /*
                                    todo : start a thread safe write queue loop in ctor for non thread safe sets
                                    since every packet should possible be a seperate thread we want as much thread safety as possible and the least amount of locks
                                    for this i'd say we have a queue of (ref object setobj, object value)
                                    we iterate through this value on the main loop and set setobj = value.
                                    if any thread that isint the main thread wants to change a value they'll have to go through this queue. 

                                    this currently is not needed but if we decide to receive immediately after end receiving (which we can't do right now seeing as this method isint thread safe)
                                    */
                                }
                                else
                                {
                                    if (packetLength >= TerrPacket.MinPacketSize)
                                        OnPacketReceived(TerrPacket.Parse(packetBuffer, this));
                                    else // this is rare but when it happens it breaks receiving.
                                        Log.Critical($"Received packet under min size: {packetLength}");
                                }
                            }
                        }
                    }
                    else
                    {
                        // not a full packet in buffer, fuck it
                        // havent hit this with a critical packet (SendSections have hit it but fuck them)
                        Log.Warning(
                            $"Malformed packet. Parsed type {TerrPacket.GetType(buffer)}. Sizes: expected {packetProvidedLength,-5} received {bytesReceived}");
                    }
                    BeginReceive(incompletePacketBuffer);

                }, null);
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
        public void Disconnect(string reason = "Disconnect() called.")
        {
            if (!_socket.Connected)
                throw new InvalidOperationException("You must be connected to a server to disconnect from it.");

            SetDisconnectState(reason);
            _socket.Close();
        }

        internal void SetDisconnectState(string reason)
        {
            _disconnectEvent.Set();
            OnDisconnected(reason);
            IsLoggedIn = false;
        }

        ///<summary> Blocking call and wait until the client has disconnected.</summary>
        public void Wait() => _disconnectEvent.WaitOne();

        #endregion

        public void Dispose()
        {
            Disconnect("TerrariaClient Dispose()");
            _socket?.Dispose();
        }
    }
}
