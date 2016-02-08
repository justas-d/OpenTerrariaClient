using System;
using OpenTerrariaClient.Packet;

namespace OpenTerrariaClient.Model
{
    public class CurrentPlayer : Player
    {
        public Guid? Guid { get; internal set; }

        internal CurrentPlayer(Player player) : base(player) { }

        ///<summary>Sets the bot players health to the given value.</summary>
        public void SetHealth(ValPidPair<short> value)
        {
            Health = new ValPidPair<short>(value) { PlayerId = PlayerId };
            Client.Send(TerrPacketType.PlayerLife, Health.CreatePayload());
        }

        ///<summary>Sets the bot players mana to the given value.</summary>
        public void SetMana(ValPidPair<short> value)
        {
            Mana = new ValPidPair<short>(value) { PlayerId = PlayerId };
            Client.Send(TerrPacketType.PlayerMana, Mana.CreatePayload());
        }

        ///<summary>Sets the bot players buffs</summary>
        public void SetBuffs(BuffList buffs)
        {
            Buffs = buffs ?? new BuffList();
            Client.Send(TerrPacketType.UpdatePlayerBuff, Buffs.CreatePayload());
        }

        ///<summary>Sets a buff at the given index on the bot player.</summary>
        public void SetBuff(byte buff, byte index)
        {
            if (index >= BuffList.MaxBuffs) throw new ArgumentOutOfRangeException(nameof(index));
            Buffs.InternalBuffs[index] = buff;
            SetBuffs(Buffs);
        }

        ///<summary>Sets the bot players inventory</summary>
        public void SetInventory(PlayerInventory inventory)
        {
            Inventory = inventory ?? new PlayerInventory();

            for (byte i = 0; i < PlayerInventory.InventorySize; i++)
                Client.Send(TerrPacketType.SetInventory, Inventory.InternalItems[i].CreatePayload());
        }

        ///<summary>Sets an item in the bot players inventory at the given slot id.</summary>
        public void SetItem(GameItem setItem, byte slotId)
        {
            // check for out of range slotid
            if (slotId >= PlayerInventory.InventorySize) throw new ArgumentOutOfRangeException(nameof(slotId));

            GameItem item = new GameItem(setItem)
            {
                PlayerId = PlayerId,
                SlotId = slotId
            };
            Inventory.InternalItems[slotId] = item;

            Client.Send(TerrPacket.Create(TerrPacketType.SetInventory, item.CreatePayload()));
        }

        ///<summary>Sets the bot players team.</summary>
        public void SetTeam(byte team)
        {
            if (PlayerId == null) throw new ArgumentNullException(nameof(PlayerId));

            Client.Send(TerrPacketType.PlayerTeam, new[] { PlayerId.Value, team });
        }
        ///<summary>Toggles the pvp state or explicitly sets it.</summary>
        public void TogglePvp(bool? state = null)
        {
            if (state == null)
                TogglePvp(!IsPvp);
            else
                Client.Send(TerrPacketType.TogglePvp, new TogglePvp(PlayerId.Value, state.Value));
        }

        public void SetPos(ValPair<float> pos, bool requestTileData = false)
        {
            Position = pos;
            Client.Send(TerrPacketType.UpdatePlayer, new UpdatePlayer(this));

            if (requestTileData)
                Client.Send(TerrPacketType.RequestInitialTileData, new ValPair<int>((int) pos.Val1, (int) pos.Val2));
        }

        public void SendMessage(string text)
            => Client.Send(TerrPacketType.ChatMessage, new ChatMessage(PlayerId.Value, text));

        public void Killme(string deathText = "", byte hitDirection = 0, bool pvp = false)
            => Client.Send(TerrPacketType.KillMe, new KillMe(PlayerId.Value, hitDirection, Health.Val2, pvp, deathText));
    }
}
