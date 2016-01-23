using System;
using System.IO;

namespace TerrariaBridge.Packet
{
    public class PlayerInventory
    {
        public const byte InventorySize = 0xb4;

        public PlayerItem[] Items { get; }

        public PlayerInventory(PlayerItem[] items = null)
        {
            Items = items?.Length == InventorySize ? items : new PlayerItem[InventorySize];
        }

        public byte[] CreatePayload(byte pid, byte itemIndex)
        {
            if (itemIndex >= InventorySize)
                throw new ArgumentOutOfRangeException($"{nameof(itemIndex)}: {itemIndex} >= {nameof(InventorySize)}: {InventorySize}");

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(pid); // player id 
                    writer.Write(itemIndex); // slot id
                    writer.Write(Items[itemIndex].CreatePayload());
                }
                return stream.ToArray();
            }
        }
    }
}