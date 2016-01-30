using System.Collections.Generic;
using System.Linq;

namespace TerrariaBridge.Model
{
    public class PlayerInventory
    {
        public const byte InventorySize = 0xb4;

        internal readonly GameItem[] InternalItems;

        ///<summary>Returns a list of items this player has. Use CurrentPlater.SetInventory to set inventory.</summary>
        public IEnumerable<GameItem> Items => InternalItems.Select(i => i);

        internal PlayerInventory(PlayerInventory value)
        {
            InternalItems = new GameItem[InventorySize];

            for (byte i = 0; i < InventorySize; i++)
                InternalItems[i] = new GameItem(value.InternalItems[i]);
        }

        public PlayerInventory(GameItem[] items = null)
        {
            InternalItems = items?.Length == InventorySize ? items : new GameItem[InventorySize];

            // check if any of the objects in the array are null and replace them if they are. also assign corrent slot ids
            for (byte i = 0; i < InventorySize; i++)
            {
                if (InternalItems[i] == null)
                    InternalItems[i] = new GameItem();

                InternalItems[i].SlotId = i;
            }
        }
    }
}
