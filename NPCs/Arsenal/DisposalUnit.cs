using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ArsenalLabsBeta.NPCs.Arsenal
{
    public class DisposalUnit : SuperChargeEnemy
    {

        //Constants

        //For animation coordinates thingy
        private const int sheetHeight = 544;
        private int timer = 0;

        private enum DUSuperState
        {
            grounded,
            jump
        }

        private DUSuperState DUState = DUSuperState.grounded;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
        }

        private int immuneTime;

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 26;
            NPC.defense = 5;
            NPC.lifeMax = 30;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.direction = -1;

            DrawOffsetY = 3;

            //Needs balancing
            NPC.damage = 10;

            immuneTime = 30;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath37;


        }

        public override void FindFrame(int frameHeight)
        {

            //We adjust the width and height of the frame itself.
            int width = 60; int height = 42;

            NPC.frame.Width = width; NPC.frame.Height = height;

            //Where we start from and where we end to
            int startFrame = 0, finalFrame = 7;

            //Then we set the speed for how much frames (of FPS fame) it should pass per... NPC frame
            int frameSpeed = 6;

            //Apparently there's an internal counter for that purpose?? regardless, do use 
            if (!Supercharged) NPC.frameCounter += 0.5;

            if (Supercharged)
            {
                if (DUState == DUSuperState.grounded)
                {
                    if (timer > 120 - (frameSpeed * 3))
                    {
                        NPC.frameCounter += 1;
                        startFrame = 8; finalFrame = 11;
                    }
                    else
                        NPC.frame.Y = 8 * (height);
                }
                else if (DUState == DUSuperState.jump)
                {
                    if (NPC.velocity.Y > 0) NPC.frame.Y = 12 * (height);
                    else NPC.frame.Y = 11 * (height);
                }
            }

            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += height;

                if (NPC.frame.Y > finalFrame * height)
                {
                    NPC.frame.Y = startFrame * height;
                }
            }


        }


        public override bool PreAI()
        {

            immuneTime--;

            NPC.immortal = immuneTime > 0;
            if (!Supercharged)
            {
                NPC.velocity.X = 0.3f;

                //Make it able to walk back and forth 
                if (NPC.collideX)
                {
                    NPC.direction *= -1;
                    NPC.spriteDirection *= -1;
                }
                NPC.velocity.X *= NPC.direction;
            }
            else if (Supercharged || timer > 0)
            {
                NPC.TargetClosestUpgraded(true);
                NPC.spriteDirection = NPC.direction;
                if (timer == 1) { NPC.velocity = new Vector2(NPC.direction * 6, -5f); DUState = DUSuperState.jump; }
                if (timer > 30) if (NPC.collideY) { NPC.velocity.X = 0; DUState = DUSuperState.grounded; }
                if (timer > 120) { timer = 0; DUState = DUSuperState.grounded; }
                timer++;
                SuperchargeTimer--;
            }

            return true;
        }

    }
}
