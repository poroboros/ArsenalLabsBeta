using System;
using System.Collections.Generic;
using CalamityMod.NPCs.DesertScourge;
using ArsenalLabsBeta.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.Bloomhammer
{

    public class BloomhammerExplosion : ModProjectile
    {
        private int frameNum = 7;

        private int dustAmt = 90;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wulfrum Explosion");
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 7 * 12;
            Projectile.penetrate = -1;
            Projectile.DamageType = ArsenalLabsBeta.Rogue;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft;

        }

        bool runOnce = true;

        public override void AI()
        {


            Projectile.ai[0]++;
            Projectile.frame = (int)Math.Floor(Projectile.ai[0] / 35 * 7);

            if (Projectile.frame == 5 && runOnce)
            {
                runOnce = false;
                SoundEngine.PlaySound(SoundID.NPCDeath14.WithVolumeScale(.8f), Projectile.Center);

                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 circular = new Vector2(2, 0).RotatedBy(MathHelper.ToRadians(i * 360 / dustAmt));
                    Vector2 spawnPos = Projectile.Center + circular;
                    Vector2 spawnPos2 = Projectile.Center + (circular * 10);



                    if (i % 18 == 0)
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos2, new Vector2(Main.rand.NextFloat(-1, 1), 0), ModContent.ProjectileType<BloomPetal>(), 0, 0, Projectile.owner);


                    circular = circular.RotatedBy(MathF.PI / 2);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            Dust dust = Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust2>());
                            dust.velocity *= 0.5f;
                            dust.velocity += circular * 4f + Projectile.velocity * Main.rand.NextFloat(2, 5f);
                            //dust.scale *= 1.9f;
                            dust.alpha = 0;
                            dust.color = Color.Lerp(Color.MistyRose, Color.Magenta, 0.1f);
                            dust.fadeIn = 0.1f;
                            dust.noGravity = true;
                        }
                    }

                }

            }

            if (Projectile.frame > frameNum)
            {

                Projectile.Kill();

            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                modifiers.FinalDamage -= Projectile.numHits * 0.095f;
            if (target.type == ModContent.NPCType<DesertScourgeBody>())
                modifiers.FinalDamage -= 0.2f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / (2 * frameNum));
            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                drawOrigin.X = texture.Width - drawOrigin.X;
                effects = SpriteEffects.FlipHorizontally;
            }
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int height = texture.Height / frameNum;
            int startingH = Projectile.frame % frameNum;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);

            float glowOpacity = 0f;
            float glowScaleMod = .1f;

            if (Projectile.frame < 5)
            {
                glowScaleMod += 1f;
                glowOpacity += .15f;
            }
            else
            {
                glowOpacity = glowOpacity > 0 ? glowOpacity - .02f : 0;
            }
            Color gOpacity = new Color(glowOpacity, glowOpacity, glowOpacity, glowOpacity);
            Color glowColor = new Color(0 / 255f, 116 / 255f, 255 / 255f, 255).MultiplyRGBA(gOpacity);
            ArsenalLabsProjectile.DrawCircularGlow(glowColor, Projectile, frame, drawOrigin, effect: effects);

            // Vector2 radialOrig = new Vector2(radial.Width / 2, radial.Height / 2);
            // Main.spriteBatch.Draw(radial, drawPos, null, glowColor, Projectile.rotation, radialOrig, Projectile.scale * glowScaleMod, effects, 0);

            Main.spriteBatch.Draw(texture, drawPos, frame, Color.White.MultiplyRGBA(new Color(.8f, .8f, .8f, .8f)), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) <= Projectile.width)
            {
                return null;
            }
            return false;
        }


        /// <summary>
        /// Draws a "flower" in dust using a parametric equations.
        /// Most parameters are vectors so you can have seperate x and y values
        /// 
        /// </summary>
        /// <param name="center">Center where the flower is drawn</param>
        /// <param name="width"></param>
        /// <param name="circleNumber">Number of circles in the flower,</param>
        /// <param name="dotsPer">How many dots of dust can this draw</param>
        /// <param name="color"></param>
        /// <param name="dustScale"></param>
        public static void LineFlower(Vector2 center, Vector2 width, Vector2 circleNumber, int dotsPer, Color color, float dustScale = 1)
        {
            //DESMOS EQUATIONS
            //\left(1.5\cos\left(t\right)-\cos\left(15t\right),1.5\sin\left(t\right)-\sin\left(15t\right)\right)
            //\left(2\cos\left(t\right)-\cos\left(10t\right),2\sin\left(t\right)-\sin\left(10t\right)\right)

            float tPi = MathHelper.TwoPi;

            dotsPer = dotsPer > 0 ? dotsPer : 1;

            float t = 0;

            List<Vector2> points = new List<Vector2>();



            while (t < tPi)
            {

                Vector2 pos = Vector2.Zero;

                pos.X = (width.X * (MathF.Cos(t) - MathF.Cos(circleNumber.X * t)));
                pos.Y = (width.Y * (MathF.Sin(t) - MathF.Sin(circleNumber.Y * t)));

                points.Add(center + pos);

                t += tPi / dotsPer;
            }

            ArsenalLabsHelpers.DrawFishingLineBetweenPoints(points, color, false, dustScale);



        }

    }
}
