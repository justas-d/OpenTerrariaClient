using OpenTerrariaClient.Client;

namespace OpenTerrariaClient.Model
{
    public class Player
    {
        public enum TeamType : byte
        {
            None = 0,
            Red = 1,
            Green = 2,
            Blue = 3,
            Yellow = 4,
            Pink,
        }

        private const int DefaultHp = 100;
        private const int DefaultMana = 10;

        private byte? _pid;

        public byte? PlayerId
        {
            get { return _pid; }
            internal set
            {
                _pid = value;
                if (Appearance != null)
                    Appearance.PlayerId = value;

                if (Health != null)
                    Health.PlayerId = value;

                if (Mana != null)
                    Mana.PlayerId = value;

                if (Buffs != null)
                    Buffs.PlayerId = value;

                if (Inventory != null)
                    for (byte i = 0; i < PlayerInventory.InventorySize; i++)
                        Inventory.InternalItems[i].PlayerId = value;
            }
        }

        ///<summary>Gets whether this player is the server represented as a player.</summary>
        public bool IsServer => PlayerId == byte.MaxValue;

        public PlayerAppearance Appearance { get; internal set; }
        public ValPidPair<short> Health { get; internal set; }
        public ValPidPair<short> Mana { get; internal set; }
        public BuffList Buffs { get; internal set; }
        public PlayerInventory Inventory { get; internal set; }

        public ValPair<float> Position { get; internal set; }
        public ValPair<float> Velocity { get; internal set; }

        public bool IsPvp { get; internal set; }

        public byte SelectedItem { get; internal set; }
        public byte Pulley { get; internal set; }
        public byte Control { get; internal set; }
        public TeamType Team { get; internal set; }

        internal TerrariaClient Client { get; set; }

        internal Player(byte pid, TerrariaClient client)
        {
            PlayerId = pid;
            Client = client;
        }

        internal Player(Player player)
        {
            Appearance = new PlayerAppearance(player.Appearance);
            Health = new ValPidPair<short>(player.Health);
            Mana = new ValPidPair<short>(player.Mana);
            Buffs = new BuffList(player.Buffs);
            Inventory = new PlayerInventory(player.Inventory);
            PlayerId = player.PlayerId;
        }

        public Player(PlayerAppearance appearance = null,
            ValPidPair<short> health = null, ValPidPair<short> mana = null,
            BuffList buffs = null, PlayerInventory inventory = null)
        {
            Appearance = appearance ?? new PlayerAppearance();
            Buffs = buffs ?? new BuffList();
            Inventory = inventory ?? new PlayerInventory();
            Health = health ?? new ValPidPair<short>(DefaultHp, DefaultHp);
            Mana = mana ?? new ValPidPair<short>(DefaultMana, DefaultMana);
        }
    }
}
