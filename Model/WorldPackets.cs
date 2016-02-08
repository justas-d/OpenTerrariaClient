using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenTerrariaClient.Packet;

namespace OpenTerrariaClient.Model
{
    public sealed class SetNpcKillCount : PacketWrapper
    {
        public short NpcType { get; private set; }
        public int KillCount { get; private set; }

        internal SetNpcKillCount() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            NpcType = reader.ReadInt16();
            KillCount = reader.ReadInt32();
        }
    }

    public sealed class WorldTime : PacketWrapper
    {
        public bool IsDay { get; private set; }
        public int Time { get; private set; }
        public short SunModY { get; private set; }
        public short MoonModY { get; private set; }

        internal WorldTime() { }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            CheckForValidType(type, TerrPacketType.Time);
            IsDay = reader.ReadBoolean();
            Time = reader.ReadInt32();
            SunModY = reader.ReadInt16();
            MoonModY = reader.ReadInt16();
        }
    }

    public sealed class WorldInfo : PacketWrapper
    {
        private int _time;

        public int RawTime
        {
            get { return _time; }
            internal set
            {
                _time = value;

                // taken from the terraria src. 
                // i swear the devs must have been high when they designed the time system.
                if (!IsDay)
                    value += 54000;

                double time = value/86400.0*24.0 - 7.5 - 12.0;

                if (time < 0.0)
                    time += 24.0;

                int hour = (int) time;
                int minute = (int) ((time - hour)*60.0);

                Time = new TimeSpan(0, hour, minute);
            }
        }

        public TimeSpan Time { get; private set; }

        internal byte DayMoonInfo { get; private set; }
        public byte MoonPhase { get; private set; }
        public short MaxTilesX { get; private set; }
        public short MaxTilesY { get; private set; }
        public short SpawnX { get; private set; }
        public short SpawnY { get; private set; }
        public short WorldSurface { get; private set; }
        public short RockLayer { get; private set; }
        public int WorldId { get; private set; }
        public string WorldName { get; private set; }
        public byte MoonType { get; private set; }
        public byte TreeBackground { get; private set; }
        public byte CorruptionBackground { get; private set; }
        public byte JungleBackground { get; private set; }
        public byte SnowBackground { get; private set; }
        public byte HallowBackground { get; private set; }
        public byte CrimsonBackground { get; private set; }
        public byte DesertBackground { get; private set; }
        public byte OceanBackground { get; private set; }
        public byte IceBackStyle { get; private set; }
        public byte JungleBackStyle { get; private set; }
        public byte HellBackStyle { get; private set; }
        public float WindSpeedSet { get; private set; }
        public byte CloudNumber { get; private set; }
        public int Tree1 { get; private set; }
        public int Tree2 { get; private set; }
        public int Tree3 { get; private set; }
        public byte TreeStyle1 { get; private set; }
        public byte TreeStyle2 { get; private set; }
        public byte TreeStyle3 { get; private set; }
        public byte TreeStyle4 { get; private set; }
        public int CaveBack1 { get; private set; }
        public int CaveBack2 { get; private set; }
        public int CaveBack3 { get; private set; }
        public byte CaveBackStyle1 { get; private set; }
        public byte CaveBackStyle2 { get; private set; }
        public byte CaveBackStyle3 { get; private set; }
        public byte CaveBackStyle4 { get; private set; }
        public float Rain { get; private set; }
        public sbyte InvasionType { get; private set; }
        public ulong LobbyId { get; private set; }

        public short SunModY { get; internal set; }
        public short MoonModY { get; internal set; }

        public bool IsDay { get; internal set; }
        public bool IsBloodmoon { get; internal set; }
        public bool IsEclipse { get; internal set; }

        public bool IsShadowOrbSmashed { get; internal set; }
        public bool IsEyeOfCthuluDefeated { get; internal set; }
        public bool IsEowOrBrainDefeated { get; internal set; }
        public bool IsSkeletronDefeated { get; internal set; }
        public bool IsHardmode { get; internal set; }
        public bool IsClownDefeated { get; internal set; }
        public bool IsPlanteraDefeated { get; internal set; }

        public bool IsDestroyedDeafeated { get; internal set; }
        public bool AreTwinsDefeated { get; internal set; }
        public bool IsSkeletronPrimeDefeated { get; internal set; }
        public bool IsAnyMechBossDefeated { get; internal set; }
        public bool IsCrimson { get; internal set; }
        public bool IsPumpkinMoonActive { get; internal set; }
        public bool IsSnowMoonActive { get; internal set; }

        public bool IsExpertMode { get; internal set; }
        public bool IsFastForwardingTime { get; internal set; }
        public bool IsSlimeRainActive { get; internal set; }
        public bool IsSlimeKingDefeated { get; internal set; }
        public bool IsQueenBeeDefeated { get; internal set; }
        public bool IsFishronDefeated { get; internal set; }
        public bool AreMartiansDefeated { get; internal set; }
        public bool AreCultistsDefeated { get; internal set; }

        public bool IsMoonlordDefeated { get; internal set; }
        public bool IsHalloweenKingDefeated { get; internal set; }
        public bool IsHalloweenTreeDefeated { get; internal set; }
        public bool IsChristmasIceQueenDefeated { get; internal set; }
        public bool IsChristmasSantankDefeated { get; internal set; }
        public bool IsChristmasTreeDefeated { get; internal set; }
        public bool IsGolemDefeated { get; internal set; }

        public bool IsCloudBgActive { get; internal set; }

        public Dictionary<short, int> NpcKillCount { get; internal set; } = new Dictionary<short, int>();

        internal WorldInfo() { }

        internal void SetNpcKc(SetNpcKillCount kc)
        {
            if (!NpcKillCount.ContainsKey(kc.NpcType))
                NpcKillCount.Add(kc.NpcType, 0);

            NpcKillCount[kc.NpcType] = kc.KillCount;
        }

        protected override void WritePayload(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override void ReadPayload(PayloadReader reader, TerrPacketType type)
        {
            if (type != TerrPacketType.WorldInformation)
                throw new ArgumentException($"{nameof(type)} is not {TerrPacketType.WorldInformation}");

            int tempTime = reader.ReadInt32(); // store this value so we have the correct IsDay when evalutaing real time.

            DayMoonInfo = reader.ReadByte();
            BitArray timeInfo = new BitArray(new [] {DayMoonInfo});
            IsDay = timeInfo[0];
            IsBloodmoon = timeInfo[1];
            IsEclipse = timeInfo[2];

            RawTime = tempTime;

            MoonPhase = reader.ReadByte();
            MaxTilesX = reader.ReadInt16();
            MaxTilesY = reader.ReadInt16();
            SpawnX = reader.ReadInt16();
            SpawnY = reader.ReadInt16();
            WorldSurface = reader.ReadInt16();
            RockLayer = reader.ReadInt16();
            WorldId = reader.ReadInt32();
            WorldName = reader.ReadString();
            MoonType = reader.ReadByte();
            TreeBackground = reader.ReadByte();
            CorruptionBackground = reader.ReadByte();
            JungleBackground = reader.ReadByte();
            SnowBackground = reader.ReadByte();
            HallowBackground = reader.ReadByte();
            CrimsonBackground = reader.ReadByte();
            DesertBackground = reader.ReadByte();
            OceanBackground = reader.ReadByte();
            IceBackStyle = reader.ReadByte();
            JungleBackStyle = reader.ReadByte();
            HellBackStyle = reader.ReadByte();
            WindSpeedSet = reader.ReadSingle();
            CloudNumber = reader.ReadByte();
            Tree1 = reader.ReadInt32();
            Tree2 = reader.ReadInt32();
            Tree3 = reader.ReadInt32();
            TreeStyle1 = reader.ReadByte();
            TreeStyle2 = reader.ReadByte();
            TreeStyle3 = reader.ReadByte();
            TreeStyle4 = reader.ReadByte();
            CaveBack1 = reader.ReadInt32();
            CaveBack2 = reader.ReadInt32();
            CaveBack3 = reader.ReadInt32();
            CaveBackStyle1 = reader.ReadByte();
            CaveBackStyle2 = reader.ReadByte();
            CaveBackStyle3 = reader.ReadByte();
            CaveBackStyle4 = reader.ReadByte();
            Rain = reader.ReadSingle();

            BitArray events1 = new BitArray(new [] {reader.ReadByte()});
            IsShadowOrbSmashed = events1[0];
            IsEyeOfCthuluDefeated = events1[1];
            IsEowOrBrainDefeated = events1[2];
            IsSkeletronDefeated = events1[3];
            IsHardmode = events1[4];
            IsClownDefeated = events1[5];
            // events1[6] is Main.serverSideCharacter.
            IsPlanteraDefeated = events1[7];

            BitArray events2 = new BitArray(new [] {reader.ReadByte()});
            IsDestroyedDeafeated = events2[0];
            AreTwinsDefeated = events2[1];
            IsSkeletronPrimeDefeated = events2[2];
            IsAnyMechBossDefeated = events2[3];
            IsCloudBgActive = events2[4];
            IsCrimson = events2[5];
            IsPumpkinMoonActive = events2[6];
            IsSnowMoonActive = events2[7];

            BitArray events3 = new BitArray(new [] {reader.ReadByte()});
            IsExpertMode = events3[0];
            IsFastForwardingTime = events3[1];
            IsSlimeRainActive = events3[2];
            IsSlimeKingDefeated = events3[3];
            IsQueenBeeDefeated = events3[4];
            IsFishronDefeated = events3[5];
            AreMartiansDefeated = events3[6];
            AreCultistsDefeated = events3[7];

            BitArray events4 = new BitArray(new [] {reader.ReadByte()});
            IsMoonlordDefeated = events4[0];
            IsHalloweenKingDefeated = events4[1];
            IsHalloweenTreeDefeated = events4[2];
            IsChristmasIceQueenDefeated = events4[3];
            IsChristmasSantankDefeated = events4[4];
            IsChristmasTreeDefeated = events4[5];
            IsGolemDefeated = events4[6];

            InvasionType = reader.ReadSByte();
            LobbyId = reader.ReadUInt64();
        }
    }
}
