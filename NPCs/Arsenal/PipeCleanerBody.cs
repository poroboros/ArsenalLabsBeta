using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal
{
    public class PipeCleanerBody : ModNPC
    {


        //References to the other pipecleaner body parts
        public NPC nextSegment => NPC.ai[0] > 0 && NPC.ai[0] < 255 ? Main.npc[(int)NPC.ai[0]] : null;
        public NPC prevSegment => NPC.ai[1] > 0 && NPC.ai[1] < 255 ? Main.npc[(int)NPC.ai[1]] : null;


        Texture2D defaultTex;
        Texture2D cornerTex;
        Texture2D tailTex;

        Texture2D activeTex;
        public override void SetDefaults()
        {
            NPC.damage = 8;
            NPC.width = 8;
            NPC.height = 8;
            NPC.defense = 50;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 1;
            AnimationType = 0;
            //NPC.value = (float)Item.buyPrice(0, 0, 1, 15);
            NPC.lavaImmune = false;
            NPC.noGravity = true;
            NPC.noTileCollide = false;

            NPC.ShowNameOnHover = false;

            NPC.immortal = false;
            //NPC.HitSound = new SoundStyle?(SoundID.NPCHit1);
            // NPC.DeathSound = new SoundStyle?(SoundID.NPCDeath1);

            defaultTex = (Texture2D)ModContent.Request<Texture2D>(Texture);
            cornerTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "Bend");
            tailTex = (Texture2D)ModContent.Request<Texture2D>(Texture + "Tail");

            activeTex = defaultTex;

        }


        //Called whenever the head moves, this does not move independently. Starts from the head and recursively moves through each part
        public void UpdateSegment(Vector2 newPosition)
        {

            //Set the position to the old position of the last segment
            Vector2 oldPos = NPC.Center;
            Vector2 direction = (-newPosition + NPC.Center).SafeNormalize(Vector2.Zero);

            activeTex = defaultTex;

            NPC.Center = newPosition;



            bool xBigger = MathF.Abs(direction.X) > MathF.Abs(direction.Y);


            //rotate segment properly
            Vector2 towards = new Vector2(0, 1 * (direction.Y / MathF.Abs(direction.Y)));

            if (xBigger)
            {
                towards = new Vector2(1 * (direction.X / MathF.Abs(direction.X)), 0);


            }

            NPC.rotation = towards.ToRotation();

            if (nextSegment != null && nextSegment.ModNPC is PipeCleanerBody)
            {
                PipeCleanerBody body = (PipeCleanerBody)nextSegment.ModNPC;
                body.UpdateSegment(oldPos);



                //Draw the corner texture is segments are perpendicular
                if (prevSegment != null && prevSegment.rotation != NPC.rotation && !Double.IsNaN(NPC.rotation))
                {

                    activeTex = cornerTex;


                    Vector2 dir1 = (nextSegment.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    Vector2 dir2 = (prevSegment.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    Vector2 dir3 = (prevSegment.Center - nextSegment.Center).SafeNormalize(Vector2.Zero);
                    Vector2 dir4 = (nextSegment.Center - prevSegment.Center).SafeNormalize(Vector2.Zero);



                    float rot1 = dir1.ToRotation() < MathHelper.PiOver4 ? dir1.ToRotation() + MathHelper.TwoPi : dir1.ToRotation();
                    float rot2 = dir2.ToRotation() < MathHelper.PiOver4 ? dir2.ToRotation() + MathHelper.TwoPi : dir2.ToRotation();



                    //Main.NewText(rot1 + " " + rot2 + " " + MathHelper.ToDegrees(dir3.ToRotation()) + " " + + MathHelper.ToDegrees(dir4.ToRotation()));


                    float extra = MathHelper.PiOver2;

                    if (rot1 > 6.28 && MathHelper.ToDegrees(dir3.ToRotation()) > 90)
                    {
                        rot1 -= MathHelper.TwoPi;
                    }
                    if (rot2 > 6.28 && MathHelper.ToDegrees(dir4.ToRotation()) > 90)
                    {
                        rot2 -= MathHelper.TwoPi;
                    }

                    //NPC.rotation = dir1.ToRotation() + MathHelper.PiOver4 + extra;

                    NPC.rotation = MathHelper.Max(rot1, rot2);
                    NPC.rotation += extra;
                    //Main.NewText(MathHelper.ToDegrees(rot1) + " " + MathHelper.ToDegrees(rot2) + " " + MathHelper.ToDegrees(NPC.rotation));

                }
            }
            else
            {
                //Set the tail segment if it does not have a next segment
                if (prevSegment != null)
                {
                    direction = (-prevSegment.Center + NPC.Center).SafeNormalize(Vector2.Zero);

                    xBigger = MathF.Abs(direction.X) > MathF.Abs(direction.Y);

                    towards = new Vector2(0, 1 * (direction.Y / MathF.Abs(direction.Y)));


                    if (xBigger)
                    {
                        towards = new Vector2(1 * (direction.X / MathF.Abs(direction.X)), 0);

                    }

                    NPC.rotation = towards.ToRotation();

                    activeTex = tailTex;
                    NPC.rotation += MathHelper.PiOver2;
                }
            }
        }

        //Zero out velocity, this AI sets position directly
        public override bool PreAI()
        {
            NPC.ShowNameOnHover = false;
            NPC.dontCountMe = true;
            NPC.velocity = Vector2.Zero;
            return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (prevSegment == null || prevSegment.active == false)
            {
                NPC.active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            SpriteEffects effect = SpriteEffects.None;

            var texture = activeTex;
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


            Vector2 drawOrigin = new Vector2(texture.Width / 2, (texture.Height / 2));

            Main.spriteBatch.Draw(texture, drawPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);


            return false;
        }
    }
}
