using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public class TerrListenerConfig
    {
        public int TimeoutMs { get; }
        public Player PlayerData { get; }

        public TerrListenerConfig(int timeoutms = 5000, Player playerData = null)
        {
            TimeoutMs = timeoutms;
            PlayerData = playerData ?? new Player();
        }
    }
}