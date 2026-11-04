using System.IO;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs
{
    public abstract class SuperChargeEnemy : ModNPC
    {



        //supercharge code based off calamity, every wulfrum enemy needs this probably
        public float SuperchargeTimer;

        public bool Supercharged => SuperchargeTimer > 0.0;

        /// <summary>
        /// Put extra AI in this function instead, as to not overwrite the Supercharge Netcode
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SuperchargeWriteAI(BinaryWriter writer)
        {

        }

        /// <summary>
        /// Put extra AI in this function instead, as to not overwrite the Supercharge Netcode
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SuperchargeReadAI(BinaryReader reader)
        {

        }

        //don't overwrite these two pls
        public override void SendExtraAI(BinaryWriter writer)
        {
            SuperchargeWriteAI(writer);
            writer.Write(SuperchargeTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SuperchargeReadAI(reader);
            SuperchargeTimer = reader.ReadSingle();
        }

    }
}
