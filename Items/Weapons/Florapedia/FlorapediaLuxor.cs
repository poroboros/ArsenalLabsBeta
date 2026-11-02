using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace ArsenalLabsBeta.Projectiles.Magic.Florapedia
{
    public class FlorapediaLuxor : LuxorsGiftMagic, IFlorapediaHologram
    {
        public static Asset<Texture2D> bloom;
        private static Color projColor = new(242, 223, 148);
        private const int AppearingTime = 80;
        private const int MinTimeleftToDisappear = 30;
        private const int AttackTime = 160;
        private Vector2 relativePos;
        private bool shouldSync;

        private float AppearingProgress { get { return Projectile.localAI[2]; } set { Projectile.localAI[2] = value; } }
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Magic/LuxorsGiftMagic";

        public override void Load()
        {
            if (!Main.dedServ)
                bloom = ModContent.Request<Texture2D>("ArsenalLabs/Assets/BloomStrip");
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 34;
            Projectile.timeLeft = AppearingTime + AttackTime;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            relativePos = Vector2.Zero;
            shouldSync = false;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= AttackTime && Projectile.timeLeft > MinTimeleftToDisappear ? null : false;
        public override bool ShouldUpdatePosition() => Projectile.timeLeft <= AttackTime;
        public override void AI()
        {
            float ap = AppearingProgress;
            IFlorapediaHologram.UpdateVfx(Projectile, projColor, AppearingTime, AttackTime, MinTimeleftToDisappear, ref ap);
            AppearingProgress = ap;

            UpdateMovement();
            if (Main.myPlayer == Projectile.owner && shouldSync)
            {
                shouldSync = false;
                Projectile.netUpdate = true;
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = Utils.CenteredRectangle(Projectile.Center, new(Projectile.width));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate == Projectile.maxPenetrate)
            {
                Projectile.velocity /= 3;
                if (Main.myPlayer == Projectile.owner)
                    shouldSync = true;
            }
            else if (Projectile.penetrate == 1)
                Disappear();
        }

        public override void OnKill(int timeLeft)
        {
            if (AppearingProgress >= 1)
            {
                float rotation = Main.rand.NextFloat(MathHelper.PiOver4 / 2);
                for (int i = 0; i < 3; i++)
                    GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(Projectile.Center, Vector2.One.RotatedBy((i - 1) * rotation + Projectile.rotation - MathHelper.PiOver4 * 3) * Main.rand.NextFloat(10, 15), Main.rand.NextFloat(3, 5), projColor, 25));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Disappear();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBloom();
            if (AppearingProgress < 1)
            {
                IFlorapediaHologram.DrawHologram(Projectile, Projectile.timeLeft > AttackTime, AppearingProgress, projColor);
                return false;
            }

            Color drawColor = projColor with { A = 100 };
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], drawColor, 2, null, true);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadByte();
        }

        public void Disappear()
        {
            Projectile.penetrate = -1;
            Projectile.timeLeft = MinTimeleftToDisappear;
            Projectile.tileCollide = false;
            if (Main.myPlayer == Projectile.owner)
                shouldSync = true;
        }

        private void UpdateMovement()
        {
            if (Projectile.timeLeft > AttackTime)
                // appearing
                IFlorapediaHologram.UpdateAppearingMovement(Projectile, Owner, ref relativePos);
            else if (Projectile.timeLeft == AttackTime)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = (Projectile.Center - Main.MouseWorld).SafeNormalize(Vector2.Zero) * 10;
                    Projectile.netUpdate = true;
                }
                Projectile.tileCollide = true;
                if (Main.projectile[(int)Projectile.ai[0]].ModProjectile is FlorapediaHoldout book)
                    book.RemoveProj(Projectile.whoAmI);
                else
                {
                    Projectile.Kill();
                    return;
                }
            }
            else if (Projectile.timeLeft > AttackTime - 10)
            {
                Projectile.velocity *= 0.8f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2, 0.2f);
                    Projectile.ai[1] = Projectile.rotation;
                    Projectile.netUpdate = true;
                }
                else
                    Projectile.rotation = Projectile.ai[1];
            }
            else if (Projectile.timeLeft == AttackTime - 10)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * 12;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.timeLeft <= MinTimeleftToDisappear)
            {
                Projectile.alpha += 3;
                Projectile.velocity *= 0.9f;
                Projectile.alpha -= 15;
                if (Projectile.alpha <= 0)
                    Projectile.alpha = 0;
            }
            else
                base.AI();
        }

        private void DrawBloom()
        {
            Texture2D bloomTexture = bloom.Value;
            Rectangle source = bloomTexture.Bounds;
            Vector2 pos = Projectile.Center + new Vector2(0, Projectile.height / 2 * Owner.gravDir) - Main.screenPosition - new Vector2(0, Projectile.height * AppearingProgress / 2 * Owner.gravDir) - Projectile.velocity;
            float bloomScale = Projectile.scale * AppearingProgress * (Projectile.timeLeft > MinTimeleftToDisappear ? 1 : 0);

            Main.EntitySpriteDraw(bloomTexture, pos, source, projColor with { A = 0 } * AppearingProgress * Projectile.Opacity, Projectile.rotation, source.Size() / 2, bloomScale, SpriteEffects.None);
        }
    }
}
