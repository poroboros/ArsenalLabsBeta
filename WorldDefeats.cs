using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ArsenalLabsBeta
{
    public class WorldDefeats : ModSystem
    {
        public static byte arsenalTrialPhase;


        public void ResetFlags()
        {

            arsenalTrialPhase = 0;
        }

        public override void OnWorldLoad()
        {
            ResetFlags();
        }

        public override void OnWorldUnload()
        {
            ResetFlags();
        }

        public override void SaveWorldData(TagCompound tag)
        {

            tag["ArsenalTrialPhase"] = arsenalTrialPhase;
        }

        public override void LoadWorldData(TagCompound tag)
        {

            arsenalTrialPhase = tag.Get<byte>("ArsenalTrialPhase");
        }

        public override void NetSend(BinaryWriter writer)
        {

            writer.Write(arsenalTrialPhase);
        }

        public override void NetReceive(BinaryReader reader)
        {
            arsenalTrialPhase = reader.ReadByte();
        }
    }
}
