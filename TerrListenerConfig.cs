using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge
{
    public class TerrListenerConfig
    {
        private bool _isLocked;

        private PlayerAppearanceData _appearance = new PlayerAppearanceData();
        private int _timeoutMs = 5000;

        public PlayerAppearanceData Appearance
        {
            get { return _appearance; }
            set { SetValue(ref _appearance, value); }
        }

        public int TimeoutMs
        {
            get { return _timeoutMs; }
            set { SetValue(ref _timeoutMs, value); }
        }

        public TerrListenerConfig()
        {
        }

        // https://github.com/RogueException/Discord.Net/blob/master/src/Discord.Net/Config.cs
        private void SetValue<U>(ref U storage, U value)
        {
            if (_isLocked)
                throw new InvalidOperationException(
                    "Unable to modify a client's configuration after it has been created.");
            storage = value;
        }

        public void Lock() => _isLocked = true;
    }
}