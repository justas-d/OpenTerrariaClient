using System;
using TerrariaBridge.Model;

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
        public bool TrackItemData { get; internal set; }
        public bool TrackNpcData { get; internal set; }
        public bool TrackProjectileData { get; internal set; }

        public TerrariaClientConfig(Guid? playerGuid = null, int keepaliveFreqMs = 5000, int timeoutms = 5000,
            Player player = null, string password = null, string terrariaVersion = "Terraria156",
            bool trackItems = true, bool trackProjectiles = true, bool trackNpcs = true)
        {
            TimeoutMs = timeoutms;
            PlayerData = player ?? new Player();
            Password = password;
            TerrariaVersion = terrariaVersion;
            KeepaliveFrequencyMs = keepaliveFreqMs;
            PlayerGuid = playerGuid;
            TrackItemData = trackItems;
            TrackProjectileData = trackProjectiles;
            TrackNpcData = trackNpcs;
        }
    }
}
