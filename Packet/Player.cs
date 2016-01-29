using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrariaBridge.Client;

namespace TerrariaBridge.Packet
{
    public class Player
    {
        public class PlayerInventory
        {
            public const byte InventorySize = 0xb4;

            internal readonly PlayerItem[] InternalItems;

            ///<summary>Returns a list of items this player has. Use CurrentPlater.SetInventory to set inventory.</summary>
            public IEnumerable<PlayerItem> Items => InternalItems.Select(i => i);

            internal PlayerInventory(PlayerInventory value)
            {
                InternalItems = new PlayerItem[InventorySize];

                for (byte i = 0; i < InventorySize; i++)
                    InternalItems[i] = new PlayerItem(value.InternalItems[i]);
            }

            public PlayerInventory(PlayerItem[] items = null)
            {
                InternalItems = items?.Length == InventorySize ? items : new PlayerItem[InventorySize];

                // check if any of the objects in the array are null and replace them if they are. also assign corrent slot ids
                for (byte i = 0; i < InventorySize; i++)
                {
                    if (InternalItems[i] == null)
                        InternalItems[i] = new PlayerItem();

                    InternalItems[i].SlotId = i;
                }
            }
        }

        public class BuffList : PacketWrapper
        {
            public const byte MaxBuffs = 22;

            internal byte? PlayerId { get; set; }

            internal byte[] InternalBuffs;

            ///<summary>Returns the buffs this player has. Use CurrentPlayer.SetBuffs to set buffs.</summary>
            public IEnumerable<byte> Buffs => InternalBuffs.Select(b => b);

            private BuffList() { }

            public BuffList(BuffList value)
            {
                PlayerId = value.PlayerId;

                InternalBuffs = new byte[MaxBuffs];
                for (byte i = 0; i < MaxBuffs; i++)
                    InternalBuffs[i] = value.InternalBuffs[i];
            }

            public BuffList(byte[] buffs = null)
            {
                InternalBuffs = buffs?.Length == MaxBuffs ? buffs : new byte[MaxBuffs];
            }

            protected override void WritePayload(BinaryWriter writer)
            {
                if (PlayerId == null) throw new NullReferenceException($"{nameof(PlayerId)} doesn't have a value set.");

                writer.Write(PlayerId.Value);
                for (byte i = 0; i < MaxBuffs; i++)
                    writer.Write(InternalBuffs[i]);
            }

            protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
            {
                if (type != TerrPacketType.UpdatePlayerBuff)
                    throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.UpdatePlayerBuff}");

                if(InternalBuffs == null)
                    InternalBuffs = new byte[MaxBuffs];

                PlayerId = reader.ReadByte();
                for (byte i = 0; i < MaxBuffs; i++)
                    InternalBuffs[i] = reader.ReadByte();
            }
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

        ///<summary>Gets whether this player is the server represented as a player. (PlayerId == 0xff)</summary>
        public bool IsServer => PlayerId == 0xff;

        public PlayerAppearance Appearance { get; internal set; }
        public ValPidPair<short> Health { get; internal set; }
        public ValPidPair<short> Mana { get; internal set; }
        public BuffList Buffs { get; internal set; }
        public PlayerInventory Inventory { get; internal set; }

        public ValPair<float> Position { get; internal set; }
        public ValPair<float> Velocity { get; internal set; }

        public byte SelectedItem { get; internal set; }

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

    public class CurrentPlayer : Player
    {
        public Guid? Guid { get; internal set; }

        internal CurrentPlayer(Player player) : base(player) { }

        ///<summary>Sets the bot players health to the given value.</summary>
        public void SetHealth(ValPidPair<short> value)
        {
            Health = new ValPidPair<short>(value) {PlayerId = PlayerId};
            Client.Send(TerrPacketType.PlayerLife, Health.CreatePayload());
        }

        ///<summary>Sets the bot players mana to the given value.</summary>
        public void SetMana(ValPidPair<short> value)
        {
            Mana = new ValPidPair<short>(value) {PlayerId = PlayerId};
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

            for (byte i = 0; i < Player.PlayerInventory.InventorySize; i++)
                Client.Send(TerrPacketType.SetInventory, Inventory.InternalItems[i].CreatePayload());
        }

        ///<summary>Sets an item in the bot players inventory at the given slot id.</summary>
        public void SetItem(PlayerItem setItem, byte slotId)
        {
            // check for out of range slotid
            if (slotId >= PlayerInventory.InventorySize) throw new ArgumentOutOfRangeException(nameof(slotId));

            PlayerItem item = new PlayerItem(setItem)
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
            if(PlayerId == null) throw new ArgumentNullException(nameof(PlayerId));

            Client.Send(TerrPacketType.PlayerTeam, new byte[] {PlayerId.Value, team});
        }
    }
}
