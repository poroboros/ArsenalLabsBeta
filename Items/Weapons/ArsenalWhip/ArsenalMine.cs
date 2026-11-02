using System;
using CalamityMod.Particles;
using ArsenalLabsBeta.Buffs.WhipTags;
using ArsenalLabsBeta.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.ArsenalWhip
{
    // ArsenalMine is a placeholder name
    public class ArsenalMine : ModProjectile
    {
        static readonly SoundStyle ExplosionSound = new SoundStyle("CalamityMod/Sounds/Item/ELRFire");
        static readonly Color SmokeColor = new Color(0.615f, 0.36f, 0.471f);
        public Vector2 stickingPos
        {
            get { return new Vector2(Projectile.ai[1], Projectile.ai[2]); }
            set
            {
                Projectile.ai[1] = value.X;
                Projectile.ai[2] = value.Y;
            }
        }
        public NPC Target => Main.npc[(int)Projectile.ai[0]];
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 300;
        }
        public override void OnSpawn(IEntitySource source)
        {
            stickingPos = Projectile.position - Target.position;
        }

        public override void AI()
        {
            Projectile.position = Target.position + stickingPos;
            Projectile.localAI[0] = MathF.Sin(20 * MathF.Pow(Projectile.timeLeft, 0.3f)) + 1;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);
            if (!Target.active || Target.life <= 0)
                Projectile.Kill();
        }
        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft == 0 && target.whoAmI == Projectile.ai[0];

        public override void OnKill(int timeLeft)
        {
            // remove tag from target
            ArsenalLabsTag.SafeDeleteBuff(Target, ModContent.BuffType<DefaultTag>());
            ArsenalLabsTag taggedTarget = Target.GetGlobalNPC<ArsenalLabsTag>();
            taggedTarget.tagType = -1;
            taggedTarget.mineindex = -1;
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X - 1.65f, Projectile.Center.Y - 5.25f), Vector2.Zero, ModContent.ProjectileType<ArsenalMineExplosion>(), 0, Projectile.knockBack, Projectile.owner);
            for (int i = 0; i < 15; i++)
            {
                float rotation = MathHelper.TwoPi * i / 15;
                // Many rands to keep things looking fluid:
                Vector2 velocityRand = new Vector2(Main.rand.NextFloat(4, 10), Main.rand.NextFloat(4, 10));
                float scaleRand = Main.rand.NextFloat(0.35f, 0.65f);
                float opacityRand = Main.rand.NextFloat(0.2f, 0.45f);
                int lifetimeRand = (int)Main.rand.NextFloat(65f, 85f);
                HeavySmokeParticle smoke = new HeavySmokeParticle(new Vector2(Projectile.Center.X - 1.65f, Projectile.Center.Y - 5.25f), velocityRand.RotatedBy(rotation), SmokeColor, lifetimeRand, scaleRand, opacityRand, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            Projectile.Damage();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // for testing
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, $"Timeleft: {Projectile.timeLeft}", Target.Top + new Vector2(0, -30) - Main.screenPosition, Color.White);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Bounds, Color.White, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);

            Color glowColor = Color.White;
            glowColor.A = (byte)(glowColor.A * Projectile.localAI[0] * 0.5f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Bounds, glowColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
