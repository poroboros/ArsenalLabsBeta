using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal
{
    public class RustCreep : SuperChargeEnemy
    {

        private Texture2D tex;
        private Texture2D glowTex;

        private Texture2D angryTex;
        private Texture2D angryGlowTex;

        /// <summary>
        /// distance from center to check for a tile for the climbing ability
        /// </summary>
        public Vector2 tileCheckOffset = new Vector2(25, 0);

        /// <summary>
        /// How long between movements
        /// </summary>
        public float rotationDelay = 90;

        /// <summary>
        /// How much it rotates during one frame when moving
        /// </summary>
        public static float angularVel = 0.002f;


        /// <summary>
        /// How much to offset the sprite when rotated on it's corner
        /// </summary>
        public float drawOffsetRotated = 6;


        /// <summary>
        /// Allows you to disable supercharge effect (for pacified)
        /// </summary>
        public bool noSuperCharge = false;

        private float rotationAmount = 0;

        public float movementSpeed = 5f;

        private int immuneTime;

        private int curWall = 0;



        private int climbTime = 0;


        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.BlazingWheel;
            NPC.damage = 8;
            NPC.width = 24;
            NPC.height = 24;
            NPC.defense = 2;
            NPC.lifeMax = 18;
            NPC.knockBackResist = 1;
            AnimationType = 0;
            NPC.value = (float)Item.buyPrice(0, 0, 1, 15);
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            immuneTime = 30;
            //NPC.HitSound = new SoundStyle?(SoundID.NPCHit1);
            // NPC.DeathSound = new SoundStyle?(SoundID.NPCDeath1);

            tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
            glowTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "_Glow");

            angryTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "_Angry");
            angryGlowTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "_AngryGlow");

        }

        bool firstTime = true;

        float oldRot = 0;
        float rotStep = 0;


        private int oldCurWall;

        public override bool PreAI()
        {

            oldCurWall = curWall;
            angularVel = .005f;
            immuneTime--;

            NPC.immortal = immuneTime > 0;

            Vector2 foot = tileCheckOffset;

            //This checks to see if the creep has a tile in the 4 cardinal directions
            //If so, it sticks to it (aka no gravity)
            NPC.noGravity = false;


            climbTime--;



            List<int> possible = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                Point pos = (NPC.Center + foot).ToTileCoordinates();


                if (Main.tile[pos].IsTileSolid())
                {

                    possible.Add(i);
                    curWall = i;



                    if (climbTime > 0)
                    {
                        climbTime = 60;
                    }
                    else
                    {
                        climbTime += 2;
                    }

                    //NPC.noGravity = true;
                    //break;

                }


                foot = foot.RotatedBy(MathHelper.PiOver2);

            }


            Player play = Main.player[NPC.FindClosestPlayer()];


            //If supercharged, drop on players below
            if (play.Distance(NPC.Center) < 1000 && play.Center.Y > NPC.Center.Y + 10 &&
                MathF.Abs(play.Center.X - NPC.Center.X) < 100 && climbTime > 0 && curWall == 3 && Supercharged && !noSuperCharge)
            {
                climbTime = -10;
            }

            if (climbTime < 0)
            {
                if (MathHelper.ToDegrees(NPC.rotation) % 90 is < 1 and > -1)
                {
                }
                else
                {

                    NPC.rotation += .05f * NPC.direction;
                }
                //NPC.velocity.X = 0;
                curWall = -1;
                return false;
            }

            if (possible.Count == 1)
            {
                curWall = possible[0];
            }
            else if (possible.Count > 1)
            {

                //helps avoid getting stuck, may need custom cases 
                foreach (int wall in possible)
                {
                    if (wall != oldCurWall)
                    {
                        curWall = wall;
                        break;
                    }
                }
            }


            if (curWall == 3)
                NPC.GravityMultiplier *= -1;
            if (curWall % 2 == 0)
                NPC.GravityMultiplier *= 0;

            //faster if ICU bonus
            int bonus = Supercharged && !noSuperCharge ? 5 : 1;

            //moves the creep
            NPC.ai[2] += movementSpeed * bonus;

            SuperchargeTimer--;


            if (firstTime)
            {
                NPC.rotation = 0;
                firstTime = false;
            }


            if (NPC.ai[2] >= rotationDelay)
            {
                //Saves rotation and position
                //Main.NewText(rotStep);
                if (rotStep == 0)
                {
                    oldRot = NPC.rotation;
                    rotStep = 0;
                }

                //allows AI to run, rotating the creep
                return true;

            }
            else if (NPC.ai[2] < rotationDelay - 30)
            {
                //freeze if not rotating
                NPC.velocity = Vector2.Zero;

                //NPC.Center = NPC.Center.ToTileCoordinates().ToWorldCoordinates();
            }



            //only run AI if rotating
            return false;
        }

        public override void AI()
        {


            int dir = -NPC.direction;


            int rotMod = curWall is 2 or 3 ? -1 : 1;
            dir *= rotMod;

            NPC.rotation = MathHelper.Lerp(oldRot, oldRot - (MathHelper.PiOver2 * dir), rotStep);


            //bounds rotation

            //NPC.rotation = NPC.rotation > MathHelper.TwoPi-.05f ? 0 : NPC.rotation;
            //Main.NewText(NPC.rotation);

            rotStep += rotationAmount;

            rotationAmount += angularVel;


            //have velocity make sense with rotation
            NPC.velocity *= rotationAmount * 4;



            //flip rotation on odd numbered walls for visual consistency
            if (climbTime > 0)
            {
                if (curWall is 3 or 1)
                    NPC.velocity.Y = 0;
            }

            //NPC.Center += new Vector2(0,-MathF.Sin(rotStep*MathHelper.TwoPi));


            if (rotStep > 1)
            {

                rotationAmount = 0;


                rotStep = 0;
                NPC.ai[2] = 0;

                //round to nearest 90 degree interval
                int degree = -dir * (int)MathHelper.ToDegrees(NPC.rotation);

                int mDegree = degree % 360;
                if (mDegree > -45 && mDegree < 45)
                    NPC.rotation = -dir * MathHelper.ToRadians(0);
                if (mDegree > 45 && mDegree < 135)
                    NPC.rotation = -dir * MathHelper.ToRadians(90);
                if (mDegree > 135 && mDegree < 180 + 45)
                    NPC.rotation = -dir * MathHelper.ToRadians(180);
                if (mDegree > 180 + 45 && mDegree < 270)
                    NPC.rotation = -dir * MathHelper.ToRadians(270);
                if (mDegree > 270 + 45)
                    NPC.rotation = -dir * MathHelper.ToRadians(0);


            }

        }

        public override bool? CanFallThroughPlatforms()
        {
            //So it doesn't get stuck on platforms (It behaves weird with stair platforms)
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //CatalystProjectile.DrawCircularGlow(glowColor, Projectile, frame, drawOrigin, effect: effects);

            SpriteEffects effect = SpriteEffects.None;

            var texture = Supercharged && !noSuperCharge ? angryTex : tex;
            var glowTexture = Supercharged && !noSuperCharge ? angryGlowTex : glowTex;
            if (NPC.IsABestiaryIconDummy)
            {
                return true;
            }

            Vector2 rotOffset = new Vector2(-drawOffsetRotated * MathF.Sin(rotStep * (MathF.PI)), 0).RotatedBy(MathHelper.PiOver2 * curWall);

            Vector2 drawPos = NPC.Center - Main.screenPosition + rotOffset;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);
            Main.spriteBatch.Draw(glowTexture, drawPos, frame, Color.White, NPC.rotation, drawOrigin, NPC.scale, effect, 0);


            return false;
        }
    }
}
