using System;
using ArsenalLabsBeta;
using ArsenalLabsBeta.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NPCs.Arsenal.Projectiles
{

    public class ArsenalExplosion : ModProjectile
    {
        public override string Texture => "ArsenalLabsBeta/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wulfrum Explosion");
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5; // 5 in-game frames per sprite frame. 12 sprite frames 
            Projectile.penetrate = -1;

        }

        bool runOnce = true;

        public override void AI()
        {
            Projectile.ai[0]++;

            if (runOnce)
            {
                runOnce = false;
                SoundEngine.PlaySound(SoundID.NPCDeath14.WithVolume(.8f), Projectile.Center);
                float dustAmt = 60;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 circular = new Vector2(1.5f, 0).RotatedBy(MathHelper.ToRadians(i * 360 / dustAmt));
                    //circular.X *= 0.5f;
                    circular = circular.RotatedBy(MathF.PI / 2);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Vector2 spawnPos = Projectile.Center + circular;
                        if (Main.rand.NextBool(4))
                        {
                            Dust dust = Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust2>());
                            dust.velocity *= 0.5f;
                            dust.velocity += circular * 4f + Projectile.velocity * Main.rand.NextFloat(2, 5f);
                            //dust.scale *= 1.9f;
                            dust.alpha = 0;
                            dust.color = Color.Lerp(Color.Orange, Color.DarkOrange, 0.1f);
                            dust.fadeIn = 0.1f;
                            dust.noGravity = true;
                        }
                        spawnPos = Projectile.Center + circular;
                        if (Main.rand.NextBool(6))
                        {
                            Dust dust = Dust.NewDustPerfect(spawnPos, ModContent.DustType<MonoDust2>());
                            dust.velocity *= 0.5f;
                            dust.velocity += circular * 2f + Projectile.velocity * Main.rand.NextFloat(-0.5f, 4.0f);
                            //dust.scale *= 2f;
                            dust.alpha = 0;
                            dust.color = Color.Lerp(Color.OrangeRed, Color.DarkOrange, 0.1f);
                            dust.fadeIn = 0.1f;
                            dust.noGravity = true;
                        }
                    }
                }
            }

            Projectile.frame = (int)Math.Floor(Projectile.ai[0] / 35 * 7);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(Color.White, Color.DarkSeaGreen, Projectile.ai[0] / 35);
        }
    }
}
