using System;
using System.Security.Cryptography.X509Certificates;
using CalamityMod;
using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.ProtoPortalGun
{
    public class ProtoPortalGunProj : BaseHoldOutShooterProjectile
    {
        public static Asset<Texture2D> AperTex;
        private const int ItemTime = 20;
        public override void SetStaticDefaults()
        {
            AperTex = ModContent.Request<Texture2D>(Texture);
        }

        public override void InitProjectile()
        {
            Offset = new Vector2(30, 0);
        }

        public override void AI()
        {
            AI_UpdateOwnerAndProj();
            Projectile.rotation += MathHelper.PiOver4 * Projectile.spriteDirection;
            if (Projectile.owner == Main.myPlayer)
            {
                AI_ShootThings();
                Projectile.netUpdate = true;
            }
            if (Timer != -1)
            {
                if (Timer++ > ItemTime)
                    Timer = -1;

            }
            return;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PreDraw_DrawHeldProj(Owner.ArsenalLabs().aperCore ? TextureAssets.Projectile[Type].Value : AperTex.Value, lightColor);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Owner.Center - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.One / 2, 2, SpriteEffects.None);
            Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value, Projectile.Center + Projectile.velocity * (Offset.X - 10) - Main.screenPosition, new Rectangle(0, 0, 1, 1), Color.Red, 0, Vector2.One / 2, 2, SpriteEffects.None);
            return false;
        }

        protected override void AI_UpdateOwnerAndProj()
        {
            UpdateHoldOutProjectile(Owner, Projectile, Offset, RotationLerpAmount);
            Projectile.timeLeft++;

            Owner.direction = Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
            Owner.heldProj = Projectile.whoAmI;
        }

        protected override void AI_ShootThings()
        {
            if (Timer == -1)
            {
                Vector2 pos = Projectile.Center + Projectile.velocity * (Offset.X - 10);
                if (!Collision.CanHitLine(Owner.Center, 0, 0, pos, 0, 0))
                {
                    pos = Owner.Center;
                }
                Vector2 vel = Projectile.velocity * 3;
                int type = ModContent.ProjectileType<ProtoPortalShot>();
                if (Main.mouseLeft)
                {
                    SoundEngine.PlaySound(ArsenalLabsPortalEdits.portalShootList[0], Projectile.Center);
                    Projectile.NewProjectile(Owner.GetSource_ItemUse(SItem), Projectile.Center, vel, type, 0, 0, Projectile.owner, 0);
                    Timer = 0;
                }
                else if (Main.mouseRight)
                {
                    SoundEngine.PlaySound(ArsenalLabsPortalEdits.portalShootList[1], Projectile.Center);
                    Projectile.NewProjectile(Owner.GetSource_ItemUse(SItem), Projectile.Center, vel, type, 0, 0, Projectile.owner, 1);
                    Timer = 0;
                }
            }
        }
    }
}
