using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Server.MirNetwork;
using Server.MirEnvir;
using C = ClientPackets;

namespace Server.MirDatabase
{
    public class AchievementsStats
    {
        byte version = 0;

        // ---------- Stats
        // ----- General
        public UInt64 GoldPickedUp = 0;

        // ----- Monsters
        public UInt64
                        HenKilled = 0,
                        DeerKilled = 0;


        // ---------- Achievements

        // ---------------------------------------------------------------------------

        public AchievementsStats(BinaryReader reader)
        {
            byte tempVersion = reader.ReadByte();

            GoldPickedUp = reader.ReadUInt64();

            HenKilled = reader.ReadUInt64();
            DeerKilled = reader.ReadUInt64();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(version);

            writer.Write(GoldPickedUp);

            writer.Write(HenKilled);
            writer.Write(DeerKilled);
        }
    }
}