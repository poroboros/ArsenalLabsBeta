using System;
using System.IO;
using CalamityMod.Particles;
using ArsenalLabs.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.Magic.Florapedia
{
    public class FlorapediaFlower : ModProjectile, IFlorapediaHologram
    {
        public static Asset<Effect> FlowerEffect;
        public static Asset<Texture2D> Bloom;
        private static readonly Color flowerColor = new(231, 112, 205, 255);
        private const int MinTimeleftToDisappear = 30;
        private const int AppearingTime = 80;
        private const int AttackTime = 160;
        private Vector2 initVel;
        private Vector2 relativePos;

        private float AppearingProgress { get { return Projectile.localAI[0]; } set { Projectile.localAI[0] = value; } }
        private bool ShouldSync { get { return Projectile.localAI[1] == 1; } set { Projectile.localAI[1] = value ? 1 : 0; } }

        private Player Owner => Main.player[Projectile.owner];
        public override void Load()
        {
            if (!Main.dedServ)
            {
                FlowerEffect = ModContent.Request<Effect>("ArsenalLabs/Effects/FlorapediaFlower");
                Bloom = ModContent.Request<Texture2D>("ArsenalLabs/Assets/BloomCircle");
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.timeLeft = AttackTime + AppearingTime;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            initVel = Vector2.Zero;
            relativePos = Vector2.Zero;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[1] = Main.rand.NextFloat(0.85f, 0.95f);
        }

        public override bool? CanDamage() => Projectile.timeLeft <= AttackTime && Projectile.timeLeft > MinTimeleftToDisappear ? null : false;

        public override bool ShouldUpdatePosition() => Projectile.timeLeft <= AttackTime;

        public override void AI()
        {
            float ap = AppearingProgress;
            IFlorapediaHologram.UpdateVfx(Projectile, flowerColor, AppearingTime, AttackTime, MinTimeleftToDisappear, ref ap);
            AppearingProgress = ap;

            UpdateMovement();
            if (Main.myPlayer == Projectile.owner && ShouldSync)
            {
                ShouldSync = false;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (AppearingProgress >= 1)
            {
                float rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 4; i++)
                    GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(Projectile.Center, Vector2.One.RotatedBy(i * MathHelper.PiOver2 + rotation) * Main.rand.NextFloat(1, 4), Main.rand.NextFloat(1, 2), flowerColor, 25));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            Disappear();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Disappear();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int hitboxSize = 10;
            hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(hitboxSize));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBloom();
            if (AppearingProgress < 1)
            {
                IFlorapediaHologram.DrawHologram(Projectile, Projectile.timeLeft > AttackTime, AppearingProgress, flowerColor);
                return false;
            }
            else
                DrawSecondFlower();

            lightColor = flowerColor with { A = 0 } * 0.7f;
            return true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)Projectile.timeLeft);
            writer.WriteVector2(initVel);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadByte();
            initVel = reader.ReadVector2();
        }

        public void Disappear()
        {
            Projectile.timeLeft = MinTimeleftToDisappear;
            Projectile.tileCollide = false;
            if (Main.myPlayer == Projectile.owner)
                ShouldSync = true;
        }

        private void UpdateMovement()
        {
            if (Projectile.timeLeft > AttackTime)
                // appearing
                IFlorapediaHologram.UpdateAppearingMovement(Projectile, Owner, ref relativePos);
            else if (Projectile.timeLeft == AttackTime)
            {
                // switch state
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 vec2Mouse = Main.MouseWorld - Projectile.Center;
                    float length2Mouse = vec2Mouse.Length();
                    initVel = vec2Mouse.SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f);
                    Projectile.velocity = initVel * Main.rand.Next(12, 18);
                    Projectile.tileCollide = true;
                    Projectile.netUpdate = true;
                }
                if (Main.projectile[(int)Projectile.ai[0]].ModProjectile is FlorapediaHoldout book)
                    book.RemoveProj(Projectile.whoAmI);
                else
                {
                    Projectile.Kill();
                    return;
                }

                FlorapediaHoldout.RandomSound(Projectile.Center, FlorapediaHoldout.FlowerShoot1, FlorapediaHoldout.FlowerShoot2, FlorapediaHoldout.FlowerShoot3);
            }
            else if (Projectile.timeLeft > AttackTime - 12)
                Projectile.velocity += initVel * 0.5f;
            else if (Projectile.timeLeft > MinTimeleftToDisappear)
            {
                if (!Main.dedServ && Main.rand.NextBool(8))
                    Dust.NewDustPerfect(Projectile.Center - Projectile.velocity, ModContent.DustType<PetalDust>(), Projectile.velocity);

                int target = IFlorapediaHologram.FlorapediaFindTarget(Projectile, 600);
                if (target >= 0)
                {
                    Vector2 dir = Projectile.Center.DirectionTo(Main.npc[target].Center);
                    float velLen = Projectile.velocity.Length();
                    float homingFactor = Utils.Remap(-Projectile.timeLeft, -AttackTime, -MinTimeleftToDisappear, 1, 1.5f);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.Zero), dir.SafeNormalize(Vector2.Zero), 0.1f * homingFactor).SafeNormalize(Vector2.Zero) * velLen;
                }
            }
            else if (Projectile.timeLeft <= MinTimeleftToDisappear)
            {
                Projectile.velocity *= Projectile.ai[1];
                Projectile.alpha += 5;
                if (Projectile.alpha <= 0)
                    Projectile.alpha = 0;
                if (Projectile.scale > 0.8f)
                    Projectile.scale -= 0.03f;
            }
            if (Projectile.timeLeft < AttackTime && AppearingProgress >= 1)
                Projectile.rotation += Projectile.velocity.Length() * Math.Sign(Projectile.velocity.X) / 30f;
        }

        private void DrawBloom()
        {
            Texture2D bloomTexture = Bloom.Value;
            Rectangle source = bloomTexture.Bounds;
            Vector2 pos = Projectile.Center + new Vector2(0, Projectile.height / 2 * Owner.gravDir) - Main.screenPosition - new Vector2(0, Projectile.height * AppearingProgress / 2 * Owner.gravDir);
            float bloomScale = Projectile.scale * AppearingProgress * (Projectile.timeLeft > MinTimeleftToDisappear ? 1 : 0);

            Main.EntitySpriteDraw(bloomTexture, pos, source, flowerColor with { A = 0 } * AppearingProgress * 0.8f, 0, source.Size() / 2, bloomScale, SpriteEffects.None);
        }

        private void DrawSecondFlower()
        {
            Texture2D flower = TextureAssets.Projectile[Type].Value;
            float rot = ((float)Main.timeForVisualEffects + Projectile.ai[1]) / 15;
            float scale = ((float)Math.Sin(Main.timeForVisualEffects / 6 + Projectile.ai[1] * 16) * 0.3f + 1f) * Projectile.scale;
            Main.EntitySpriteDraw(flower, Projectile.Center - Main.screenPosition, flower.Bounds, (flowerColor * 0.5f) with { A = 100 } * Projectile.Opacity, rot, flower.Size() / 2, scale, SpriteEffects.None);
        }
    }
}
