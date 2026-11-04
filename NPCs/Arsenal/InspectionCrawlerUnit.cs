using System;
using CalamityMod;
using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ArsenalLabsBeta.NPCs.Arsenal.Projectiles;

namespace ArsenalLabsBeta.NPCs.Arsenal
{

    //TODO: Add supercharged ability
    public class InspectionCrawlerUnit : SuperChargeEnemy
    {
        /// <summary>
        /// spotlight projectile, very important for this enemy
        /// </summary>
        public WulfrumEnemySpotlight spotlight = null;

        //col1 is the default color, co12 is the color for max detection, lerps between the two
        public Color col1 = Color.Green;
        public Color col2 = Color.Red;

        public Vector2 spotlightOffset = new Vector2(-4, -2);

        /// <summary>
        /// At what level of "aggression" should the sub attack
        /// </summary>
        public int attackThreshold = 400;




        //Lambda expressions for the NPC.ai variables, for better readability 

        /// <summary>
        /// Rotation of it's spotlight, derived from NPC.ai[0]. In degrees and bounded between 0 and 360
        /// </summary>
        float spotlightDeg => (NPC.ai[0] + 360) % 360;

        /// <summary>
        /// Rotation in radians, bounded between 0 and 2pi
        /// </summary>
        public float spotlightRotation => MathHelper.ToRadians(spotlightDeg);

        /// <summary>
        /// Index of the player being targeted, derived from NPC.ai[1]
        /// 
        /// This is also used to check if the spotlight should go clockwise or not, when there is no player. 
        /// If == -2, then it goes counterclockwise, otherwise it is clockwise
        /// </summary>
        public int playerIndex => (int)(NPC.ai[1]);

        /// <summary>
        /// Player the sub is focused on, if index is OOB, it will be null
        /// </summary>
        public Player targetPlayer => playerIndex > -1 && playerIndex < 256 ? Main.player[playerIndex] : null;

        /// <summary>
        /// Level of focus, which increases or decreases respectively, based on if a player is in the spotlight or not
        /// </summary>
        public float focusLevel => (NPC.ai[2]);

        public bool focused => focusLevel > 40;

        //after the sub fires, it will stop targeting for a bit
        bool hasFiredMissiles = false;

        public const float MAX_MOVEMENT_SPEED = 4;
        public const float MAX_FALL_SPEED = 6;

        public float moveAcceleration = .1f;
        public float fallAcceleration = .2f;

        private Texture2D tex;
        private Texture2D glowTex;

        private const int FRAME_NUM = 12;
        private float frameSpeed = 6;

        private float chargeRadius = 350;
        private bool firstFocus = true;


        private int immuneTime;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Wulfrum Mine");
            Main.npcFrameCount[NPC.type] = 1;
        }
        public override void SetDefaults() // Defaults copied from BigJellyPink.cs and edited
        {
            NPC.damage = 35; //Balance later
            NPC.width = 60;
            NPC.height = 50;
            NPC.defense = 10; //Balance later
            NPC.lifeMax = 350; //Balance later
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            AIType = -1;
            for (int k = 0; k < NPC.buffImmune.Length; k++) //Balancer later this means immune to EVERYTHNG
            {
                NPC.buffImmune[k] = true;
            }
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit4;
            //NPC.DeathSound = SoundID.NPCDeath37;
            NPC.value = 100 * 16f; //Balance later

            immuneTime = 30;

            tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
            glowTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "_Glow");
        }

        private int oldDir = 0;

        public override bool PreAI()
        {

            immuneTime--;

            NPC.immortal = immuneTime > 0;

            Color col = focused ? Color.Red : Color.Green;
            Lighting.AddLight(NPC.Center, col.ToVector3());
            //set up spotlight before anything else
            if (spotlight == null && !hasFiredMissiles)
            {
                int proj = Projectile.NewProjectile(new EntitySource_Parent(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<WulfrumEnemySpotlight>(), 0, 0);
                spotlight = Main.projectile[proj].ModProjectile as WulfrumEnemySpotlight;

                //set angle and length of spotlight
                spotlight.coneAngle *= 2f;
                spotlight.length = chargeRadius;

                //default to no player, going clockwise
                NPC.ai[1] = -1;

                NPC.direction = 1;
                NPC.velocity = new Vector2(2, 0);


            }



            //whether to rotate clockwise or not, if there is a player, it will instead chase them
            if (playerIndex == -2)
                NPC.ai[0] -= 0.5f;
            else if (playerIndex < 0)
                NPC.ai[0] += 0.5f;


            //bounds the rotation, maybe unneccessary


            if (playerIndex < 0)
            {

                //This properly "flips" the spotlight angle when it turns around, in a way that makes sense
                if (oldDir != NPC.spriteDirection)
                {
                    if (NPC.spriteDirection < 0)
                    {
                        if (NPC.ai[0] < 180)
                        {

                            if (NPC.ai[0] > 90)
                                NPC.ai[0] -= 90;
                        }
                        else
                        {
                            NPC.ai[0] = (90 - (270 - NPC.ai[0])) * -1;
                        }
                    }
                    else
                    {
                        if (NPC.ai[0] > 0)
                        {
                            if (NPC.ai[0] < 90)
                                NPC.ai[0] += 180;
                        }
                        else
                        {
                            NPC.ai[0] = (90 + (NPC.ai[0] - 270)) * -1;
                        }
                    }

                }





                oldDir = NPC.spriteDirection;



                if (NPC.spriteDirection > 0)
                {

                    //if angle is less than 90, set to 90 and turn clockwise
                    if (NPC.ai[0] < 75)
                    {

                        NPC.ai[0] = 75;
                        NPC.ai[1] = -1;
                    }
                    else if (NPC.ai[0] > 255)
                    {

                        NPC.ai[0] = 255;
                        NPC.ai[1] = -2;
                    }
                }
                else
                {
                    if (NPC.ai[0] > 75)
                    {

                        NPC.ai[0] = 75;
                        NPC.ai[1] = -2;
                    }
                    else if (NPC.ai[0] < -75)
                    {

                        NPC.ai[0] = -75;
                        NPC.ai[1] = -1;
                    }
                }
            }


            //NPC.ai[0] %= 180;

            return true;

        }

        /// <summary>
        /// Control spotlight behavior in this class, never runs if the spotlight is null
        /// </summary>
        void UpdateSpotlightPos()
        {
            if (spotlight == null)
                return;

            spotlight.Projectile.rotation = spotlightRotation;

            Vector2 sOff = new Vector2(spotlightOffset.X * NPC.spriteDirection, spotlightOffset.Y);
            Vector2 frameOffset = Vector2.Zero;

            if (focused)
            {
                frameOffset.Y = -4 * (overrideFrame - 8);
            }

            spotlight.Projectile.Center = NPC.Center + (sOff) + frameOffset;
            spotlight.Projectile.timeLeft = 10;


            //Lerp between the two colors based 
            spotlight.drawColor = Color.Lerp(col1, col2, MathF.Max(0, (NPC.ai[2] - 50)) / 100);


            if (targetPlayer != null)
            {

                Vector2 pos = targetPlayer.position;

                Vector2 vel = (-spotlight.Projectile.Center + pos).SafeNormalize(Vector2.Zero);


                // If the focus exceeds the attack threshole, shoot a missile every 15 frames until the attack has ended
                if (focusLevel > attackThreshold && NPC.ai[2] % 15 < 1 && !hasFiredMissiles)
                {
                    int proj2 = Projectile.NewProjectile(new EntitySource_Parent(NPC), NPC.Center + vel * 2, vel * 8, ProjectileID.MiniRetinaLaser, 10, 0);

                    Main.projectile[proj2].friendly = false;
                    //spotlight.Projectile.active = false;
                    //spotlight = null;

                    if (NPC.ai[2] > attackThreshold + 60)
                    {
                        hasFiredMissiles = true;
                    }
                }



                //Rotation of the player, relative to the sub

                float playerRad = vel.ToRotation();
                float playerRot = (360 + MathHelper.ToDegrees(playerRad)) % 360;


                //It can only chase the player slightly beyond the way it's facing
                if (NPC.spriteDirection > 0)
                {

                    if (NPC.ai[0] < playerRot && NPC.ai[0] < 285)
                    {
                        NPC.ai[0]++;
                    }
                    else if (NPC.ai[0] > 75)
                    {
                        NPC.ai[0]--;
                    }
                }
                else
                {
                    playerRot = MathHelper.ToDegrees(playerRad);
                    if (NPC.ai[0] < playerRot && NPC.ai[0] < 115)
                    {
                        NPC.ai[0]++;
                    }
                    else if (NPC.ai[0] > -115)
                    {
                        NPC.ai[0]--;
                    }
                }



                //Increase focus (NPC.ai[2]) if the player is in the cone

                Rectangle playerRect = new Rectangle((int)pos.X, (int)pos.Y, 10, 10);

                if ((spotlight.Projectile.Colliding(spotlight.Projectile.getRect(), playerRect) || NPC.ai[2] > attackThreshold) && !hasFiredMissiles)
                {
                    //NPC.ai[2]+= 2;
                    NPC.ai[2] += 1;

                }
                else
                {
                    //else reduce it
                    NPC.ai[2] -= .75f * (hasFiredMissiles ? 5 : 1);


                    if (NPC.ai[2] < 0)
                    {
                        NPC.ai[2] = -50;

                        NPC.netUpdate = true;

                        int playerDir = targetPlayer.Center.X > NPC.Center.X ? 1 : -1;

                        NPC.direction = playerDir;

                        hasFiredMissiles = false;

                        firstFocus = true;

                        if (playerRot > NPC.ai[1])
                            NPC.ai[1] = -1;
                        else
                            NPC.ai[1] = -2;
                    }
                }


            }
            else //if it doesn't have a player, check for one in the cone. (Notably,it ignores all other players if it has one chosen)
            {
                foreach (Player p in Main.player)
                {
                    if (SpotlightBase.InCone(p.position, spotlight.Projectile, spotlight.length, spotlight.coneAngle))
                    {
                        if (playerIndex < 0 || playerIndex > 255)
                        {
                            NPC.ai[1] = p.whoAmI;
                            NPC.netUpdate = true;
                        }
                        break;
                    }
                }
            }

        }


        /// <summary>
        /// Move the sub around
        /// </summary>
        void UpdateMotion()
        {


            if (!focused && Main.rand.NextBool(1000))
            {
                NPC.direction *= -1;
                NPC.netUpdate = true;
            }

            //"Gravity" of sorts
            NPC.velocity.Y += fallAcceleration;

            if (targetPlayer != null)
            {
                int playerDir = targetPlayer.Center.X > NPC.Center.X ? 1 : -1;
                NPC.velocity.X += moveAcceleration * playerDir;

                if (focused)
                {
                    if (firstFocus)
                    {
                        SoundStyle toPlay = ArsenalLabsBeta.GetSound("Item/Plasmilegion/PlasmilegionOverheatWarning").WithVolume(0.8f);
                        SoundEngine.PlaySound(toPlay, NPC.Center);

                        firstFocus = false;
                    }
                    NPC.velocity.X = 0;

                    //NPC.direction = playerDir;
                    ArsenalLabsNPC.MimicWulfrumAmplifier(NPC, chargeRadius * 10, dustType: DustID.Electric, 999999f);

                    /*if (Utils.NextBool(Main.rand, 4))
                    {
                        float num4 = (float)(6.2831854820251465 * (double)chargeRadius / 8.0);
                        for (int index = 0; (double)index < (double)num4; ++index)
                        {
                            float num5 = 6.28318548f * (float)index / num4;
                            Dust dust = Dust.NewDustPerfect(((Entity)this.NPC).Center, DustID.RedTorch, new Vector2?(), 0, new Color(), 1f);
                            dust.position = ((Entity)this.NPC).Center + (Utils.ToRotationVector2(num5) * chargeRadius);
                            dust.scale = 0.7f;
                            dust.noGravity = true;
                            dust.velocity = ((Entity)this.NPC).velocity;
                        }
                    }*/
                }
                else
                {
                    NPC.netUpdate = true;
                }
            }
            else
            {

                NPC.velocity.X += moveAcceleration * NPC.direction;
            }


            //Jump effect. 
            if (Main.rand.NextBool((100)) && !focused)
            {
                Vector2 foot = NPC.Center - new Vector2(0, -40);
                Point tilePos = CalamityUtils.ToSafeTileCoordinates(foot);
                if (CalamityUtils.IsTileSolidGround(Main.tile[tilePos]))
                {
                    NPC.velocity.Y = -MAX_FALL_SPEED;
                }

                NPC.netUpdate = true;
            }

            //Point p = new Point((int)NPC.Center.X, (int)NPC.Center.Y);
            //int w = NPC.width;
            //int h = NPC.height;

            Vector2 wallCheck = new Vector2(50, 0);

            Point pos = (NPC.Center + wallCheck).ToTileCoordinates();

            if (Main.tile[pos].IsTileSolid())
            {
                NPC.direction = -1;
            }
            pos = (NPC.Center - wallCheck).ToTileCoordinates();

            if (Main.tile[pos].IsTileSolid())
            {
                NPC.direction = 1;
            }


            NPC.spriteDirection = -NPC.direction;

            //NPC.rotation = NPC.velocity.ToRotation();



            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -2, 2);
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MAX_FALL_SPEED, MAX_FALL_SPEED);

        }

        public override void AI()
        {
            UpdateSpotlightPos();

            UpdateMotion();

        }


        public override bool? CanFallThroughPlatforms()
        {
            if (focused)
                return null;
            foreach (var player in Main.player)
            {
                if (player == null || !player.active)
                    continue;

                if (player.Center.Distance(NPC.Center) < 100 && player.Center.Y - NPC.Center.Y > 50)
                {
                    return true;
                }
            }

            return null;

            //return targetPlayer.Center.Y + NPC.Center.Y < -10;

        }

        int overrideCounter = 0;
        int overrideFrame = 0;
        public override void FindFrame(int frameHeight)
        {

            overrideCounter++;
            if (overrideCounter > frameSpeed)
            {
                overrideCounter = 0;
                overrideFrame++;
                overrideFrame = overrideFrame % (FRAME_NUM - 4);

                if (focused)
                {
                    overrideFrame = (overrideFrame % 4) + 8;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effect = SpriteEffects.None;

            var texture = tex;
            var glowTexture = glowTex;
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


            Vector2 drawOrigin = new Vector2(texture.Width / 2, (texture.Height / (2 * FRAME_NUM)));
            if (NPC.spriteDirection == -1)
            {
                drawOrigin.X = texture.Width - drawOrigin.X;
                effect = SpriteEffects.FlipHorizontally;
            }
            int height = texture.Height / FRAME_NUM;
            height += 1;
            int startingH = overrideFrame % FRAME_NUM;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);
            Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);
            Main.spriteBatch.Draw(glowTexture, drawPos, frame, Color.White, NPC.rotation, drawOrigin, NPC.scale, effect, 0);


            return false;
        }
    }
}
