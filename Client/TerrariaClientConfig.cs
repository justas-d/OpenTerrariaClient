using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client
{
    public sealed class TerrariaClientConfig
    {
        public Guid? PlayerGuid { get; internal set; }
        public int TimeoutMs { get; internal set; }
        public int KeepaliveFrequencyMs { get; internal set; }
        public Player PlayerData { get; internal set; }
        public string Password { get; internal set; }
        public string TerrariaVersion { get; internal set; }
        public string ItemTableDir { get; internal set; }
        public string BuffTable { get; internal set; }
        public string NpcTable { get; internal set; }
        public string InvasionTable { get; internal set; }
        public string ProjectileTable { get; internal set; }

        private TerrariaClientConfig() { }

        public TerrariaClientConfig(Guid? playerGuid = null, int keepaliveFreqMs = 5000, int timeoutms = 5000,
            Player playerData = null, string password = null, string terrariaVersion = "Terraria156",
            string itemTableDir = "itemid.json", string buffTable = "buffid.json", string npcTable = "npcid.json", string invasionTable = "invasionid.json", string projTable = "projectileid.json")
        {
            TimeoutMs = timeoutms;
            PlayerData = playerData ?? new Player();
            Password = password;
            TerrariaVersion = terrariaVersion;
            KeepaliveFrequencyMs = keepaliveFreqMs;
            PlayerGuid = playerGuid;
            ItemTableDir = itemTableDir;
            BuffTable = buffTable;
            NpcTable = npcTable;
            InvasionTable = invasionTable;
            ProjectileTable = projTable;
        }
    }
}
