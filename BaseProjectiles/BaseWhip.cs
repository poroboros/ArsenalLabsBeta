using System.Collections.Generic;
using ArsenalLabsBeta.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.BaseProjectiles
{
    /// <summary>
    /// Base class for a whip that handles drawing, AI, and onHit. To make a simple whip, you only need to specify stats in setWhipStats
    /// </summary>
    public abstract class BaseWhip : ModProjectile
    {
        public override string Texture => this.GetPath().Replace("ArsenalLabsWhip", "CatharsisProjectile");
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.>
            ProjectileID.Sets.IsAWhip[Type] = true;
        }


        //whip stats
        public Color fishingLineColor = Color.White;
        public Color lightingColor = Color.Transparent;
        public Color? drawColor = null;
        public int? swingDust = null;
        public int dustAmount = 0;
        public SoundStyle? whipCrackSound = SoundID.Item153;
        public Texture2D whipSegment;
        public Texture2D whipTip;

        public List<Vector2> whipPoints = new List<Vector2>();

        public int? tagType = null;
        public int tagDuration = 240;
        public float multihitModifier = .8f;

        public float segmentRotation = 0;


        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true; // This prevents the projectile from hitting through solid tiles.
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;

            whipSegment = (Texture2D)ModContent.Request<Texture2D>(this.GetPath().Replace("ArsenalLabsWhip", "CatharsisProjectile") + "_Segment");
            whipTip = (Texture2D)ModContent.Request<Texture2D>(this.GetPath().Replace("ArsenalLabsWhip", "CatharsisProjectile") + "_Tip");

            SetWhipStats();
        }

        public override bool PreAI()
        {
            if (Timer % 2 < .001)
            {
                whipPoints.Clear();
                Projectile.FillWhipControlPoints(Projectile, whipPoints);
            }
            return true;
        }

        /// <summary>
        /// Function is use to control custom whip stats, called in the parent class's set defaults
        /// </summary>
        public virtual void SetWhipStats()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.WhipSettings.Segments = 30;
            Projectile.WhipSettings.RangeMultiplier = 1f;
        }

        internal float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.

        public override bool PreDraw(ref Color lightColor)
        {
            return DrawWhip(fishingLineColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            WhipOnHit(target);
        }

        /// <summary>
        /// Applies tag buff if there is one, applies multihit penalty, and focuses minions on target. 
        /// Called in OnHitNPC
        /// </summary>
        /// <param name="target"></param>
        public virtual void WhipOnHit(NPC target)
        {
            if (tagType != null)
            {
                ArsenalLabsTag.ApplyTagDebuff(target, (int)tagType, tagDuration);
            }
            Projectile.damage = (int)(Projectile.damage * multihitModifier);
            if (Projectile.damage < 1)
            {
                Projectile.damage = 1;
            }
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }

        /// <summary>
        /// Draws whip based on example mod, override if you want custom. 
        /// Called in PreDraw
        /// </summary>
        /// <param name="lineColor"> What color the fishing line is</param>
        /// <returns></returns>
        internal bool DrawWhip(Color lineColor)
        {
            //Gets every segment of the whip
            if (whipPoints == null || whipPoints.Count < 1)
                return false;

            ArsenalLabsHelpers.DrawFishingLineBetweenPoints(whipPoints, lineColor);

            SpriteEffects flip = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.instance.LoadProjectile(Type);

            //Load projectiles using file paths
            var texture = TextureAssets.Projectile[Type].Value;


            //Sets the frame which will be displayed
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            //TODO: currently the whip handle has a spritesheet of identicle sprites as otherwise it would animate
            //when it should not. At somepoint, this should be correct so that segment 0 (the handle) does not animate.


            Vector2 pos = whipPoints[0];
            //Repeats for each whip point
            for (int i = 0; i < whipPoints.Count - 1; i++)
            {

                float scale = 1;

                //Tip of the whip
                if (i == whipPoints.Count - 2)
                {
                    //Sets image to tip texture
                    texture = whipTip;

                    //Moves the frame with the animation
                    sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                    origin = sourceRectangle.Size() / 2f;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 0)
                {

                    //Sets image to segment texture
                    texture = whipSegment;
                    //sets the frame accordingly
                    sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                    origin = sourceRectangle.Size() / 2f;


                }

                Vector2 element = whipPoints[i];
                Vector2 diff = whipPoints[i + 1] - element;

                //For this projectile, rotation along the whip is disabled for aesthetic reasons
                //Normally it would be more similiar to the rotation in the if statment
                float rotation = diff.ToRotation() + segmentRotation;
                // float rotation = diff.ToRotation();

                //Rotate the handle
                if (i == 0)
                {
                    //diff.toRotation makes it follow the rotation of the whip anim
                    //MathHelper.Pi can be subtracted / added to adjust where the handle is
                    //Use multiplication as needed to tweak that
                    rotation = diff.ToRotation();
                }

                //Main.NewText(segmentRotation);
                Color color = Lighting.GetColor(element.ToTileCoordinates());
                if (drawColor != null)
                {
                    color = (Color)drawColor;
                }

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, sourceRectangle, color, rotation, origin, scale, flip, 0);
                pos += diff;
            }
            return false;
        }

        bool runOnce = true;

        /// <summary>
        /// Runs whip AI similar to example mod, but the center is now on the whip tip. Called in AI
        /// </summary>
        public virtual void WhipAIMotion()
        {
            Player owner = Main.player[Projectile.owner];
            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (runOnce)
            {
                Projectile.WhipSettings.Segments = (int)((owner.whipRangeMultiplier + 1) * Projectile.WhipSettings.Segments);
                runOnce = false;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // Without PiOver2, the rotation would be off by 90 degrees counterclockwise.





            Projectile.Center = Vector2.Lerp(Projectile.Center, whipPoints[whipPoints.Count - 1], 1);


            // Vanilla uses Vector2.Dot(Projectile.velocity, Vector2.UnitX) here. Dot Product returns the difference between two vectors, 0 meaning they are perpendicular.
            // However, the use of UnitX basically turns it into a more complicated way of checking if the projectile's velocity is above or equal to zero on the X axis.
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            Timer++;


            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {

                Projectile.Kill();
                return;
            }


        }

        /// <summary>
        /// Plays sound and runs dust, all the parameters should be set in whip stats, though you can override them. 
        /// Called in AI
        /// </summary>
        /// <param name="lightingCol"></param>
        /// <param name="dustID"></param>
        /// <param name="dustNum"></param>
        /// <param name="sound"></param>
        public virtual void WhipSFX(Color lightingCol, int? dustID, int dustNum, SoundStyle? sound)
        {
            Player owner = Main.player[Projectile.owner];
            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            //Main.NewText(lightingCol);



            owner.heldProj = Projectile.whoAmI;
            Vector2 tip = GetTipPosition();
            if (Timer == swingTime / 2 && sound != null)
            {
                // Plays a whipcrack sound at the tip of the whip.
                SoundEngine.PlaySound(sound, tip);

            }
            if ((Timer >= swingTime * .5f))
            {
                if (dustID != null)
                {
                    for (int i = 0; i < dustNum; i++)
                    {
                        Dust.NewDust(tip, 2, 2, (int)dustID, 0, 0, Scale: .5f);
                    }
                }
                if (lightingCol != Color.Transparent)
                {
                    Lighting.AddLight(tip, lightingCol.R / 255f, lightingCol.G / 255f, lightingCol.B / 255f);
                }

            }
        }

        public override void AI()
        {
            WhipAIMotion();
            WhipSFX(lightingColor, swingDust, dustAmount, whipCrackSound);
        }


        internal Vector2 GetTipPosition()
        {
            return whipPoints[whipPoints.Count - 2];
        }
    }
}
