using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public class TerrariaLisener : IDisposable
    {
        private Socket Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private string Host { get; }
        private int Port { get; }

        private bool _canReceive = true;

        private byte _playerId;
        private PlayerAppearanceData Appearance => new PlayerAppearanceData
        {
            SkinVarient = 0,
            Hair = 0,
            Name = "Artificial Client",
            HairDye = 0,
            HideVisuals1 = 0,
            HideVisuals2 = 0,
            HideMisc = 0,
            HairColor = new TerrColor(0, 0, 0),
            SkinColor = new TerrColor(0, 0, 0),
            EyeColor = new TerrColor(0, 0, 0),
            ShirtColor = new TerrColor(0, 0, 0),
            UnderShirtColor = new TerrColor(0, 0, 0),
            PantsColor = new TerrColor(0, 0, 0),
            ShoeColor = new TerrColor(0, 0, 0),
            Difficulty = 0
        };

        public TerrariaLisener(string host, int port)
        {
            Host = host;
            Port = port;
        }

        private void SendLoginPackets()
        {
            // player appearance. It's not required but it's good to know which player represents this client.
            Send(TerrPacket.Create(TerrPacketType.PlayerAppearance,
                new[] {_playerId}.Concat(Appearance.CreatePayload()).ToArray()));

            // send uuid
            //Send(TerrPacket.Create(TerrPacketType.ClientUuid,
            //    new byte[] {0x24, 0x30}.Concat(Encoding.ASCII.GetBytes(Guid.NewGuid().ToString())).ToArray()));

            // send player life
            //Send(TerrPacket.Create(TerrPacketType.PlayerLife, new byte[]
            //{
            //    _playerId,
            //    0x64, 0x00, //current life (int16) 0x64
            //    0x64, 0x00 // max life (int16)
            //}));

            // send player mana
            //Send(TerrPacket.Create(TerrPacketType.PlayerMana, new byte[]
            //{
            //    _playerId,
            //    0x14, 0x00, //current mana (int16)
            //    0x14, 0x00 // max mana (int16)
            //}));
            // send player buffs
            //Send(TerrPacket.Create(TerrPacketType.UpdatePlayerBuff, new byte[]
            //{
            //    _playerId,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            //}));

            // send inventory data
            //for (byte i = 0; i <= 0xb3; i++)
            //{
            //    Send(TerrPacket.Create(TerrPacketType.SetInventory, new byte[]
            //    {
            //        i, _playerId,
            //        0x00, 0x00, // stack (int16)
            //        0x00, //prefix (byte)
            //        0x00, 0x00 //item netid (int16)
            //    }));
            //}

            Send(TerrPacket.Create(TerrPacketType.RequestWorldInformation));

            Send(TerrPacket.Create(TerrPacketType.RequestInitialTileData, new byte[]
            {
                0xff, 0xff, 0xff, 0xff, // X (int32)
                0xff, 0xff, 0xff, 0xff // Y (int32)
            }));

            Send(TerrPacket.Create(TerrPacketType.SpawnPlayer, new byte[]
            {
                _playerId,
                0xff, 0xff, //spawn x (int16)
                0xff, 0xff //spawn y (int16)
                //0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff // starts in player id
            }));
        }

        private async Task OnReceivePacket(TerrPacket packet)
        {
            if (packet == null) return;

            //Console.WriteLine($"Received {packet.Type} of length {packet.Length}");

            switch (packet.Type)
            {
                case TerrPacketType.ContinueConnecting:
                    _playerId = packet.Payload[0];
                    Console.WriteLine($"Set pid to {_playerId}");
                    SendLoginPackets();
                    break;
                case TerrPacketType.ChatMessage:
                    Console.WriteLine($"Chat message: {Encoding.ASCII.GetString(packet.Payload)}");
                    break;
            }
            // todo: send keepalives
        }

        public async Task Start()
        {
            Socket.BeginConnect(Host, Port, (ar) => Socket.EndConnect(ar), null);

            while (!Socket.Connected) await Task.Delay(100);
            Console.WriteLine($"Socket connected to {Socket.RemoteEndPoint}");

            Send(TerrPacket.ConnectPacket);

            while (true)
            {
                if (!_canReceive)
                    await Task.Delay(100);
                else
                    Receive();
            }
        }

        private void Receive()
        {
            byte[] buffer = new byte[Constants.BufferSize];

            _canReceive = false;
            Socket.BeginReceive(buffer, 0, Constants.BufferSize, 0, ReceiveCallback, buffer);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int bytesRead = Socket.EndReceive(ar);

            if (bytesRead <= 0)
            {
                _canReceive = true;
                return;
            }

            byte[] data = new byte[bytesRead];
            Array.Copy((byte[]) ar.AsyncState, data, bytesRead);

            Task.Run(async () => await OnReceivePacket(TerrPacket.Parse(data)));
            _canReceive = true;
        }

        private void Send(byte[] data)
        {
            Socket.BeginSend(data, 0, data.Length, 0, (ar) => Socket.EndSend(ar), null);
        }

        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}