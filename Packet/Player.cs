using System;

namespace TerrariaBridge.Packet
{
    public class Player
    {
        public const byte MaxBuffs = 22;

        public PlayerAppearance Appearance { get; }
        public Guid? PlayerGuid { get; }
        public short? CurrentLife { get; }
        public short? MaxLife { get; }
        public short? CurrentMana { get; }
        public short? MaxMana { get; }

        public byte[] Buffs { get; }
        public PlayerInventory Inventory { get; }

        public Player(PlayerAppearance appearance = null, Guid? playerGuid = null,
            short? currentLife = null, short? maxLife = null,
            short? currentMana = null, short? maxMana = null,
            byte[] buffs = null, PlayerInventory inventory = null)
        {
            Appearance = appearance ?? new PlayerAppearance();
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