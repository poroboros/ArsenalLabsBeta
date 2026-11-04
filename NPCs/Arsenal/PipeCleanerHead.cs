using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal
{
    public class PipeCleanerHead : ModNPC
    {
        /// <summary>
        /// ai0: next body segment NPC index 
        /// ai1: movement cooldown, how many frames between snake movement
        /// ai2: target, if positive, player index, if negative, npc index
        /// ai3: aggro cooldown
        ///
        /// </summary>

        public NPC nextSegment => (int)NPC.ai[0] > -1 ? Main.npc[(int)NPC.ai[0]] : null;
        public bool angry => NPC.ai[3] > 0;

        private int lastSegment;

        Texture2D defaultTex;
        Texture2D angryTex;

        int frameNum = 3;

        Vector2 targetPos;

        // bool justSpawned = true;

        int spawnCount = 5;

        private static int pelletHeal = 10;

        public static float moveCooldown = 16;
        public static float angerTime = 240;

        int pelletTargetingPause = 0;
        private int pelletingPauseTime = 3;
        public override void SetStaticDefaults()
        {
            // this.DisplayName.SetDefault("Wulfrum Slime");
            Main.npcFrameCount[this.NPC.type] = frameNum;
        }


        public override void SetDefaults()
        {
            NPC.damage = 8;
            NPC.width = 8;
            NPC.height = 8;
            NPC.defense = 9999;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 1;
            AnimationType = 0;
            NPC.value = (float)Item.buyPrice(0, 0, 1, 15);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            //NPC.HitSound = new SoundStyle?(SoundID.NPCHit1);
            // NPC.DeathSound = new SoundStyle?(SoundID.NPCDeath1);

            defaultTex = (Texture2D)ModContent.Request<Texture2D>(Texture);
            angryTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "Angry");

        }

        bool firstTime = true;

        public int SpawnPellet()
        {
            int npc = 0;
            bool retry = false;
            do
            {
                retry = false;
                int negX = Main.rand.NextBool(2) ? -1 : 1;
                int negY = Main.rand.NextBool(2) ? -1 : 1;

                Vector2 spawnOffset = new Vector2(Main.rand.Next(50, 100) * negX, Main.rand.Next(50, 100) * negY);

                Vector2 spawnPos = new Vector2(NPC.Center.X + spawnOffset.X, NPC.Center.Y + spawnOffset.Y);

                Point tile = spawnPos.ToTileCoordinates();

                if (Main.tile[tile].IsTileSolid())
                {
                    retry = true;
                }
                else
                {
                    npc = NPC.NewNPC(NPC.InheritSource(NPC), (int)(spawnPos.X), (int)(spawnPos.Y),
                        ModContent.NPCType<PipeCleanerPellet>(), ai0: NPC.whoAmI);

                    NPC.ai[2] = -npc - 1;
                }
            } while (retry);

            return npc;
        }

        public void AddSegment()
        {
            int next = (int)lastSegment;

            if (next % 2 == 0)
                pelletingPauseTime += 1;
            int nextAi = next > 0 ? next : NPC.whoAmI;

            lastSegment = NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PipeCleanerBody>(), ai0: -1, ai1: nextAi);

            Main.npc[lastSegment].realLife = NPC.whoAmI;

            if (next > 0 && next < 255)
            {
                NPC body = Main.npc[next];


                if (body != null && body.ModNPC is PipeCleanerBody)
                {
                    body.ai[0] = lastSegment;
                }
            }
        }


        public override bool PreAI()
        {


            NPC.velocity = Vector2.Zero;
            FindFrame(0);

            //Setup
            if (firstTime)
            {

                lastSegment = -1;
                firstTime = false;

                //spawns the first tail and pellet

                AddSegment();
                NPC.ai[0] = lastSegment;

                SpawnPellet();



                for (int i = 0; i < 3; i++)
                {
                    AddSegment();

                }
            }

            if (spawnCount < 0)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.ModNPC is PipeCleanerBody && npc.whoAmI != nextSegment.whoAmI)
                    {
                        if (npc.Hitbox.Intersects(NPC.Hitbox))
                        {
                            Rectangle rect = new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, NPC.width, NPC.height);
                            //int text = CombatText.NewText(rect, Color.OrangeRed, "Danger! Overheating");
                            //CombatText.NewText(rect, Color.AliceBlue, "OVERLAP");
                            //NPC.StrikeInstantKill();
                        }
                    }
                }
            }

            return true;
        }

        public override void AI()
        {
            //Only run movement every few frames determined by move cooldown
            NPC.defense = angry ? 2 : 50;

            Point pos = NPC.Center.ToTileCoordinates();

            if (Main.tile[pos].IsTileSolid())
                NPC.StrikeInstantKill();

            Vector2 oldPos = NPC.Center;
            if (NPC.ai[1] < 0)
            {

                //set target as either the player or a pellet





                Entity target = NPC.ai[2] >= 0 ? Main.player[(int)NPC.ai[2]] : Main.npc[((int)-NPC.ai[2]) - 1];





                if (pelletTargetingPause < 1)
                {
                    targetPos = target.Center;
                }


                pelletTargetingPause--;



                //If reaches a pellet, spawn a new one and become longers
                if (target != null && target.Distance(NPC.Center) < 20)
                {

                    if (NPC.ai[2] < 0)
                    {
                        target.active = false;


                        int healAmount = NPC.life + pelletHeal > NPC.lifeMax ? NPC.lifeMax - NPC.life : pelletHeal;


                        if (healAmount > 0)
                        {
                            NPC.life += healAmount;

                            NPC.HealEffect(healAmount, true);
                            NPC.netUpdate = true;
                        }

                        SpawnPellet();
                        AddSegment();


                        pelletTargetingPause = pelletingPauseTime;
                    }
                    else
                    {
                        NPC.ai[3] = 0;
                    }

                    targetPos = oldPos + new Vector2(-15f, 15).RotatedBy(NPC.rotation) * 5;

                    if (Main.tile[targetPos.ToTileCoordinates()].IsTileSolid())
                        targetPos = oldPos + new Vector2(15f, -15).RotatedBy(NPC.rotation) * 5;

                    pelletTargetingPause = pelletingPauseTime;

                }




                //Pathing
                int dir1 = targetPos.X < NPC.Center.X ? 1 : -1;
                int dir2 = 0;

                if (MathF.Abs(targetPos.X - NPC.Center.X) < 10f)
                {
                    dir1 = 0;
                    dir2 = targetPos.Y < NPC.Center.Y ? 1 : -1;
                }


                Vector2 prevPos = (NPC.Center - nextSegment.Center).SafeNormalize(Vector2.Zero);

                Vector2 towards = new Vector2(dir1, dir2);


                if (prevPos == towards)
                {
                    towards = new Vector2(dir2, dir1) * -1;
                    if (Main.tile[targetPos.ToTileCoordinates()].IsTileSolid())
                        targetPos = oldPos + new Vector2(15, -15).RotatedBy(NPC.rotation) * 5;

                    pelletTargetingPause = (int)(pelletingPauseTime * .75f);


                }


                NPC.rotation = towards.ToRotation();

                Dust.NewDust(targetPos, 5, 5, DustID.PurpleTorch);
                NPC.Center -= new Vector2(15f * towards.X, 15f * towards.Y);


                //Updated other segment positions
                if (nextSegment != null && nextSegment.ModNPC is PipeCleanerBody)
                {
                    PipeCleanerBody body = (PipeCleanerBody)nextSegment.ModNPC;
                    body.UpdateSegment(oldPos);
                }


                NPC.ai[1] = NPC.ai[3] > 0 ? moveCooldown / 2 : moveCooldown;

                spawnCount--;

            }
            NPC.ai[1]--;
            NPC.ai[3]--;

            //If no longer angry, go after the pellet
            if (NPC.ai[3] <= 0 && NPC.ai[2] >= 0)
            {


                // bool isPellet = false;
                // foreach (NPC npc in Main.npc)
                // {
                //     if (npc.type == ModContent.NPCType<PipeCleanerPellet>() && npc.ai[0] == NPC.whoAmI)
                //     {
                //
                //         isPellet = true;
                //         NPC.ai[2] = (-npc.whoAmI) - 1;
                //         pelletTargetingPause = pelletingPauseTime;
                //
                //         targetPos = oldPos + new Vector2(-15f, 15).RotatedBy(NPC.rotation) * 5;
                //
                //     }
                // }
                //
                // if (!isPellet)
                // {
                int npc = SpawnPellet();
                NPC.ai[2] = (-npc) - 1;
                pelletTargetingPause = pelletingPauseTime;

                targetPos = oldPos + new Vector2(-15f, 15).RotatedBy(NPC.rotation) * 5;



            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {

            // if (NPC.ai[3] < -angerTime)
            // {
            //
            //     NPC.netUpdate = true;
            //     NPC.ai[2] = projectile.owner;
            //     NPC.ai[3] = angerTime;
            // }

        }


        int overrideCounter = 0;
        int overrideFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            overrideCounter++;
            if (overrideCounter > 12)
            {
                overrideCounter = 0;
                overrideFrame++;
                overrideFrame = overrideFrame % frameNum;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //CatalystProjectile.DrawCircularGlow(glowColor, Projectile, frame, drawOrigin, effect: effects);

            SpriteEffects effect = SpriteEffects.None;

            var texture = angry ? angryTex : defaultTex;
            if (NPC.IsABestiaryIconDummy)
            {
                /*var texture = awakeTexture1;
                int height = texture.Height / frameNum;
                int startingH = overrideFrame % frameNum;
                Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);
                NPC.frame = frame;*/
                return true;
            }

            Vector2 drawPos = NPC.Center - Main.screenPosition;


            Vector2 drawOrigin = new Vector2(texture.Width / 2, (texture.Height / (2 * frameNum)));
            /*if (NPC.spriteDirection == -1)
            {
                drawOrigin.X = texture.Width - drawOrigin.X;
            }*/
            int height = texture.Height / frameNum;
            int startingH = overrideFrame % frameNum;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);
            Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);


            return false;
        }

    }
}
