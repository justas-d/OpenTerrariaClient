using System;
using TerrariaBridge.Packet;

namespace TerrariaBridge.Client
{
    public class ClassBuilder<T> where T : class
    {
        protected readonly T Outval;

        protected ClassBuilder(T outval)
        {
            Outval = outval;
        }

        public virtual T FinalizeBuild()
        {
            return Outval;
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
        public void Buffs(Player.BuffList buffs) => Outval.Buffs = buffs;
        public void Inventory(Player.PlayerInventory inv) => Outval.Inventory = inv;

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
        public void ItemTable(string dir) => Outval.ItemTableDir = dir;

        public TerrariaClientConfigBuilder(TerrariaClientConfig outval) : base(outval)
        {
        }
    }
}
