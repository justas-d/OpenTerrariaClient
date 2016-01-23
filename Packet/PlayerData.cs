using System;

namespace TerrariaBridge.Packet
{
    public class PlayerData
    {
        public const byte MaxBuffs = 22;

        public PlayerAppearanceData Appearance { get; }
        public Guid? PlayerGuid { get; }
        public short? CurrentLife { get; }
        public short? MaxLife { get; }
        public short? CurrentMana { get; }
        public short? MaxMana { get; }

        public byte[] Buffs { get; }
        public PlayerInventory Inventory { get; }

        public PlayerData(PlayerAppearanceData appearance = null, Guid? playerGuid = null,
            short? currentLife = null, short? maxLife = null,
            short? currentMana = null, short? maxMana = null,
            byte[] buffs = null, PlayerInventory inventory = null)
        {
            Appearance = appearance ?? new PlayerAppearanceData();
            PlayerGuid = playerGuid;
            Buffs = buffs?.Length == MaxBuffs ? buffs : null;
            Inventory = inventory;
            CurrentLife = currentLife;
            MaxLife = maxLife;
            CurrentMana = currentMana;
            MaxMana = maxMana;
        }
    }
}