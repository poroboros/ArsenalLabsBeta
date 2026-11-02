using ArsenalLabsBeta.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Buffs.WhipTags
{
    public class DefaultTag : ModBuff
    {
        public short tagType;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ArsenalLabsTag>().tagged = true;
        }

    }
}
