using System;
using OpenTerrariaClient.Model;

namespace OpenTerrariaClient.Client
{
    public class ClassBuilder<T> where T : class
    {
        protected readonly T Outval;

        protected ClassBuilder(T outval)
        {
            Outval = outval;
        }
    }

    public sealed class BuffBuilder : ClassBuilder<BuffList>
    {
        private byte _currentIndex = 0;

        public void Add(byte id) => Outval.InternalBuffs[_currentIndex++] = id;

        public BuffBuilder(BuffList outval) : base(outval)
        {
        }
    }

    public sealed class PlayerInventoryBuilder : ClassBuilder<PlayerInventory>
    {
        private class IndexHolder
        {
            private byte _current;

            public byte Current
            {
                get { return _current; }
                set
                {
                    if (value > Max)
                        _current = Min;
                    if (value < Min)
                        _current = Max;
                }
            }

            public byte  Min { get; set; }
            public byte Max { get; set; }

            public IndexHolder(byte min, byte max)
            {
                Current = min;
                Min = min;
                Max = max;
            }
        }

        // [0  -  9] hotbar
        // [10 - 49] inventory
        // [50 - 53] coins
        // [54 - 57] ammo
        // [58] unknown
        // [59 - 61] armor
        // [62 - 66] accessories
        // [67 - 68] unknown
        // [69 - 71] armor social
        // [72 - 76] accessory social
        // [77 - 78] unknown
        // [79 - 81] armor dye
        // [82 - 86] accessory dye
        // [87 - 88] unnown
        // [89 - 93] misc equips
        // [94 - 98] misc dye
        // [99 - 138] piggy bank
        // [139 - 178] safe
        // [179] trash

        private IndexHolder _hotbarIndex = new IndexHolder(0, 9);
        private IndexHolder _inventoryIndex = new IndexHolder(10, 49);
        private IndexHolder _coinIndex = new IndexHolder(50, 53);
        private IndexHolder _ammoIndex = new IndexHolder(54, 57);
        private IndexHolder _accessoryIndex = new IndexHolder(62, 66);
        private IndexHolder _socialAccessoryIndex = new IndexHolder(72, 76);
        private IndexHolder _accessoryDyeIndex = new IndexHolder(82, 86);
        private IndexHolder _piggybankIndex = new IndexHolder(99,138);
        private IndexHolder _safeIndex = new IndexHolder(139, 178);

        public void AddHotbar(GameItem item) => Outval.SetItem(_hotbarIndex.Current++, item);
        public void AddInventory(GameItem item) => Outval.SetItem(_inventoryIndex.Current++, item);
        public void AddCoin(GameItem item) => Outval.SetItem(_coinIndex.Current++, item);
        public void AddAmmo(GameItem item) => Outval.SetItem(_ammoIndex.Current++, item);
        public void SetHelmet(GameItem item) => Outval.SetItem(59, item);
        public void SetChestplate(GameItem item) => Outval.SetItem(60, item);
        public void SetLeggings(GameItem item) => Outval.SetItem(61, item);
        public void AddAccessory(GameItem item) => Outval.SetItem(_accessoryIndex.Current++, item);
        public void SetSocialHelmet(GameItem item) => Outval.SetItem(69, item);
        public void SetSocialChestplate(GameItem item) => Outval.SetItem(70, item);
        public void SetSocialLeggings(GameItem item) => Outval.SetItem(71, item);
        public void AddSocialAccessory(GameItem item) => Outval.SetItem(_socialAccessoryIndex.Current++, item);
        public void SetHelmetDye(GameItem item) => Outval.SetItem(79, item);
        public void SetChestplateDye(GameItem item) => Outval.SetItem(80, item);
        public void SetLeggingDye(GameItem item) => Outval.SetItem(81, item);
        public void AddAccessoryDye(GameItem item) => Outval.SetItem(_accessoryDyeIndex.Current++, item);
        public void SetPet(GameItem item) => Outval.SetItem(89, item);
        public void SetLightPet(GameItem item) => Outval.SetItem(90, item);
        public void SetMinecart(GameItem item) => Outval.SetItem(91, item);
        public void SetMount(GameItem item) => Outval.SetItem(92, item);
        public void SetGrapplingHook(GameItem item) => Outval.SetItem(93, item);
        public void SetPetDye(GameItem item) => Outval.SetItem(94, item);
        public void SetLightPetDye(GameItem item) => Outval.SetItem(95, item);
        public void SetMinecartDye(GameItem item) => Outval.SetItem(96, item);
        public void SetMountDye(GameItem item) => Outval.SetItem(97, item);
        public void SetGrapplingHookDye(GameItem item) => Outval.SetItem(98, item);
        public void AddPiggybank(GameItem item) => Outval.SetItem(_piggybankIndex.Current++, item);
        public void AddSafe(GameItem item) => Outval.SetItem(_safeIndex.Current++, item);
        public void SetTrash(GameItem item) => Outval.SetItem(179, item);

        public PlayerInventoryBuilder(PlayerInventory outval) : base(outval)
        {
        }
    }


    public sealed class PlayerAppearanceBuilder : ClassBuilder<PlayerAppearance>
    {
        public void Skin(byte value) => Outval.SkinVarient = value;
        public void Hair(byte value) => Outval.Hair = value;
        public void Name(string name) => Outval.Name = name;
        public void HairDye(byte value) => Outval.HairDye = value;
        public void HideVisuals1(byte value) => Outval.HideVisuals1 = value;
        public void HideVisuals2(byte value) => Outval.HideVisuals2 = value;
        public void HideMisc(byte value) => Outval.HideMisc = value;
        public void HairColor(TerrColor color) => Outval.HairColor = color;
        public void SkinColor(TerrColor color) => Outval.SkinColor = color;
        public void EyeColor(TerrColor color) => Outval.EyeColor = color;
        public void ShirtColor(TerrColor color) => Outval.ShirtColor = color;
        public void UnderShirtColor(TerrColor color) => Outval.UnderShirtColor = color;
        public void PantsColor(TerrColor color) => Outval.PantsColor = color;
        public void ShoeColor(TerrColor color) => Outval.ShoeColor = color;
        public void Difficulty(byte value) => Outval.Difficulty = value;

        public PlayerAppearanceBuilder(PlayerAppearance outval) : base(outval)
        {
        }
    }

    public sealed class PlayerDataBuilder : ClassBuilder<Player>
    {
        public void Appearance(PlayerAppearance appear) => Outval.Appearance = appear;
        public void Appearance(Action<PlayerAppearanceBuilder> builder)
            => builder(new PlayerAppearanceBuilder(Outval.Appearance = new PlayerAppearance()));
        public void Health(short value) => Outval.Health = new ValPidPair<short>(value, value);
        public void Health(short current, short max) => Outval.Health = new ValPidPair<short>(current, max);
        public void Mana(short value) => Outval.Mana = new ValPidPair<short>(value, value);
        public void Mana(short current, short max) => Outval.Mana = new ValPidPair<short>(current, max);
        public void Buffs(BuffList buffs) => Outval.Buffs = buffs;
        public void Buffs(Action<BuffBuilder> builder)
            => builder(new BuffBuilder(Outval.Buffs = new BuffList()));
        public void Inventory(PlayerInventory inv) => Outval.Inventory = inv;

        public void Inventory(Action<PlayerInventoryBuilder> builder)
            => builder(new PlayerInventoryBuilder(Outval.Inventory = new PlayerInventory()));

        public PlayerDataBuilder(Player outval) : base(outval)
        {
        }
    }

    public sealed class TerrariaClientConfigBuilder : ClassBuilder<TerrariaClientConfig>
    {
        public void Guid(Guid guid) => Outval.PlayerGuid = guid;
        public void Timeout(int timeout) => Outval.TimeoutMs = timeout;
        public void KeepaliveFreq(int freq) => Outval.KeepaliveFrequencyMs = freq;
        public void Player(Player player) => Outval.PlayerData = player;
        public void Player(Action<PlayerDataBuilder> builder) => builder(new PlayerDataBuilder(Outval.PlayerData = new Player()));
        public void Password(string pass) => Outval.Password = pass;
        public void TerrariaVersion(string version) => Outval.TerrariaVersion = version;
        public void TrackItems(bool value) => Outval.TrackItemData = value;
        public void TrackNpcs(bool value) => Outval.TrackNpcData = value;
        public void TrackProjectiles(bool value) => Outval.TrackProjectileData = value;

        public TerrariaClientConfigBuilder(TerrariaClientConfig outval) : base(outval)
        {
        }
    }
}
