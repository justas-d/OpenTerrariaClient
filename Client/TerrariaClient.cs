using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using OpenTerrariaClient.Client.Service;
using OpenTerrariaClient.Model;
using OpenTerrariaClient.Packet;

namespace OpenTerrariaClient.Client
{
    public partial class TerrariaClient : IDisposable
    {
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly ManualResetEvent _disconnectEvent = new ManualResetEvent(false);
        private readonly ConcurrentDictionary<byte, Player> _players = new ConcurrentDictionary<byte, Player>();
        private readonly ConcurrentDictionary<short, WorldItem> _items = new ConcurrentDictionary<short, WorldItem>();
        internal readonly ConcurrentDictionary<short, Npc> _npcs = new ConcurrentDictionary<short, Npc>();
        private readonly ConcurrentDictionary<short, WorldProjectile> _projectiles = new ConcurrentDictionary<short, WorldProjectile>();

        private const int BufferSize = 0x1FFFE;
        private MemoryStream _packetStream = new MemoryStream();
        private BinaryReader _packetReader;

        public const byte ServerPlayerId = byte.MaxValue;
        private Player ServerDummyPlayer => new Player(new PlayerAppearance("Server")) {PlayerId = ServerPlayerId};

        public LogManager Log { get; } = new LogManager();
        ///<summary>Returns the configuration data used for this client.</summary>
        public TerrariaClientConfig Config { get; internal set; }
        ///<summary>Returns the service manager for this client.</summary>
        public ServiceManager Services { get; private set; }

        ///<summary>Returns whether this client is logged into a server</summary>
        public bool IsLoggedIn { get; internal set; }
        ///<summary>Returns whether this client is connected to a server.</summary>
        public bool IsConnected => _socket.Connected;

        ///<summary>Returns a list of projectiles currently active in the world.</summary>
        public IEnumerable<WorldProjectile> Projectiles => _projectiles.Values;
        ///<summary>Returns a list of currently online players on the server.</summary>
        public IEnumerable<Player> Players => _players.Values;
        ///<summary>Returns a list of items present the world.</summary>
        public IEnumerable<WorldItem> WorldItems => _items.Values;
        ///<summary>Returns a list of npcs that are present in the world.</summary>
        public IEnumerable<Npc> Npcs => _npcs.Values;
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
            _packetReader = new BinaryReader(_packetStream);

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
                try
                {
                    _socket.EndConnect(ar);
                    connectDone.Set();
                }
                catch (Exception ex)
                {
                    Log.Critical($"Couldn't connect to {host}:{port}. {ex}");
                }
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

            Log.Info($"Disconnected: {pid}");
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
            this.Send(TerrPacketType.UpdateItemOwner, new UpdateItemOwner(id, owner));

            if (!Config.TrackItemData) return;

            WorldItem item = GetExistingItem(id);

            if (item != null)
                item.Owner = owner;
        }

        internal void ItemAddOrUpdate(WorldItem item)
            => _items.AddOrUpdate(item.UniqueId, item, (oldkey, oldval) => item);

        internal void RemoveItem(short id)
        {
            WorldItem ignored;
            _items.TryRemove(id, out ignored);
        }

        public WorldItem GetExistingItem(short id)
        {
            if(!Config.TrackItemData) throw new InvalidOperationException("Cannot get item data when item data tracking is disabled.");

            WorldItem retval;
            _items.TryGetValue(id, out retval);
            return retval;
        }
        #endregion

        #region Npc

        internal void NpcAddOrUpdate(Npc npc)
           => _npcs.AddOrUpdate(npc.UniqueId, npc, (oldkey, oldval) => npc);

        internal void RemoveNpc(short id)
        {
            Npc ignored;
            _npcs.TryRemove(id, out ignored);
        }

        public Npc GetExistingNpc(short id)
        {
            if (!Config.TrackProjectileData) throw new InvalidOperationException("Cannot get npc data when npc data tracking is disabled.");

            if (!_npcs.ContainsKey(id)) return null;

            Npc retval;
            _npcs.TryGetValue(id, out retval);
            return retval;
        }

        #endregion

        #region Projectile

        internal void ProjectileAddOrUpdate(WorldProjectile proj)
            => _projectiles.AddOrUpdate(proj.UniqueId, proj, (oldkey, oldval) => proj);

        internal void RemoveProjectile(short uniqueId)
        {
            WorldProjectile ignored;
            _projectiles.TryGetValue(uniqueId, out ignored);
        }

        public WorldProjectile GetExistingProjectile(short uniqueId)
        {
            if(!Config.TrackProjectileData) throw new InvalidOperationException("Cannot get projectile data when projectile data tracking is disabled.");

            WorldProjectile retval;
            _projectiles.TryGetValue(uniqueId, out retval);
            return retval;
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

        private void BeginReceive()
        {
            try
            {
                byte[] buffer = new byte[BufferSize];
                _socket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, (ar) =>
                {
                    int bytesRead = _socket.EndReceive(ar);

                    _packetStream.Write(buffer, 0, bytesRead);
                    _packetStream.Position = _packetStream.Position - bytesRead;
                    TryReadPacket();

                    BeginReceive();
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

        private void TryReadPacket()
        {
            while (true)
            {
                if (_packetStream.Position >= _packetStream.Length - TerrPacket.MinPacketSize)
                    break;

                ushort packetLength = _packetReader.ReadUInt16();
                _packetReader.BaseStream.Position -= sizeof(ushort);

                if (packetLength < TerrPacket.MinPacketSize)
                {
                    Log.Critical($"Corrupted packet buffer, read packet length of {packetLength}");
                    break;
                }

                if (_packetStream.Position + packetLength > _packetStream.Length)
                    break;

                OnPacketReceived(TerrPacket.Parse(_packetReader.ReadBytes(packetLength), this));
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

        ///<summary>Disposes the client without calling any disconnect events. Use this you are getting StackOverflowExeceptions by calling Dispose().</summary>
        public void SocketDispose()
        {
            _socket?.Dispose();
            _packetStream?.Dispose();
            _packetReader?.Dispose();
        }

        public void Dispose()
        {
            Disconnect("TerrariaClient Dispose()");
            SocketDispose();
        }
    }
}
