using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal
{
    public class PipeCleanerPellet : ModNPC
    {

        public override void SetDefaults()
        {
            NPC.damage = 8;
            NPC.width = 8;
            NPC.height = 8;
            NPC.defense = 2;
            NPC.lifeMax = 25;
            NPC.knockBackResist = 0f;
            AnimationType = 0;
            NPC.value = (float)Item.buyPrice(0, 0, 1, 15);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;

        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)NPC.ai[0]];


            if (!npc.active || npc.type != ModContent.NPCType<PipeCleanerHead>())
                NPC.active = false;
        }

        public override void OnKill()
        {
            NPC npc = Main.npc[(int)NPC.ai[0]];

            if (npc.active && npc.type == ModContent.NPCType<PipeCleanerHead>())
            {

                //Main.NewText("ANGER");
                npc.ai[2] = NPC.FindClosestPlayer();
                npc.ai[3] = 300;
            }

        }
    }
}
