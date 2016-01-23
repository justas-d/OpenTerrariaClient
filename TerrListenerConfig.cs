using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public class TerrListenerConfig
    {
        public int TimeoutMs { get; }
        public PlayerData PlayerData { get; }

        public TerrListenerConfig(int timeoutms = 5000, PlayerData playerData = null)
        {
            TimeoutMs = timeoutms;
            PlayerData = playerData ?? new PlayerData();
        }
    }
}