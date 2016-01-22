using System;

namespace TerrariaBridge.Packet
{
    public class WorldInfoData
    {
        public int Time { get; }
        public byte DayMoonInfo { get; }
        public byte MoonPhase { get; }
        public short MaxTilesX { get; }
        public short MaxTilesY { get; }
        public short SpawnX { get; }
        public short SpawnY { get; }
        public short WorldSurface { get; }
        public short RockLayer { get; }
        public int WorldId { get; }
        public string WorldName { get; }
        public byte MoonType { get; }
        public byte TreeBackground { get; }
        public byte CorruptionBackground { get; }
        public byte JungleBackground { get; }
        public byte SnowBackground { get; }
        public byte HallowBackground { get; }
        public byte CrimsonBackground { get; }
        public byte DesertBackground { get; }
        public byte OceanBackground { get; }
        public byte IceBackStyle { get; }
        public byte JungleBackStyle { get; }
        public byte HellBackStyle { get; }
        public Single WindSpeedSet { get; }
        public byte CloudNumber { get; }
        public int Tree1{ get; }
        public int Tree2 { get; }
        public int Tree3{ get; }
        public byte TreeStyle1{ get; }
        public byte TreeStyle2 { get; }
        public byte TreeStyle3 { get; }
        public byte TreeStyle4 { get; }
        public int CaveBack1{ get; }
        public int CaveBack2{ get; }
        public int CaveBack3{ get; }
        public byte CaveBackStyle1 { get; }
        public byte CaveBackStyle2 { get; }
        public byte CaveBackStyle3 { get; }
        public byte CaveBackStyle4 { get; }
        public Single Rain { get; }
        public byte EventInfo1 { get; }
        public byte EventInfo2 { get; }
        public byte EventInfo3{ get; }
        public byte EventInfo4 { get; }
        public sbyte InvasionType { get; }
        public ulong LobbyId { get; }

        public WorldInfoData(TerrPacket packet)
        {
            if (packet.Type != TerrPacketType.WorldInformation) throw new ArgumentException($"{nameof(packet.Type)} is not {TerrPacketType.WorldInformation}");

            using (PayloadReader reader = new PayloadReader(packet.Payload))
            {
                Time = reader.ReadInt32();
                DayMoonInfo = reader.ReadByte();
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
                EventInfo1 = reader.ReadByte();
                EventInfo2 = reader.ReadByte();
                EventInfo3 = reader.ReadByte();
                EventInfo4 = reader.ReadByte();
                InvasionType = reader.ReadSByte();
                LobbyId = reader.ReadUInt64();
            }
        }
    }
}
