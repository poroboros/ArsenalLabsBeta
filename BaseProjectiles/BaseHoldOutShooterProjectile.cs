using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using System;
using CalamityMod;

namespace ArsenalLabsBeta.Projectiles.BaseProjectiles
{
    public abstract class BaseHoldOutShooterProjectile : BaseHoldOutProjectile
    {
        /// <summary>
        /// Internal timer for attacking, is automatically set to 0 when <see cref="ResetProjectile"/> is called.
        /// </summary>
        public int Timer;

        /// <summary>
        /// Cooldown for each attack, is automatically updated according to weapon item's usetime when <see cref="ResetProjectile"/> is called.
        /// </summary>
        public int WeaponUsetime;

        /// <summary>
        /// Should this projectile be killed AFTER current attack behavior. It must be set manually. If you want to kill the projectile immediately, uses <see cref="Projectile.Kill"/>.
        /// </summary>
        protected bool ShouldKill = false;

        /// <summary>
        /// Can be used for behaviors that are not suitable to be placed in the <see cref="ModProjectile.OnSpawn(IEntitySource)"/> method.
        /// To prevent the first attack from consuming ammo twice, set <see cref="ItemID.Sets.gunProj"/>
        /// </summary>
        protected bool FirstFrame = true;

        /// <summary>
        /// The offset of weapon's sprite when used by the player. Almost the same as <see cref="ModItem.HoldoutOffset"/>.
        /// </summary>
        public Vector2 Offset = Vector2.Zero;

        /// <summary>
        /// See <see cref="BaseHoldOutProjectile.GetInterpolatedRotationVector(Vector2, Vector2, float)"/>.
        /// </summary>
        public float RotationLerpAmount = 0;
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.manualDirectionChange = true;
            Projectile.timeLeft = 60;
            FirstFrame = true;
            ShouldKill = false;
            InitProjectile();
        }

        /// <summary>
        /// Called after <see cref="SetDefaults"/>
        /// </summary>
        public virtual void InitProjectile() { }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void OnSpawn(IEntitySource source)
        {
            // Initialize Timer and WeaponUsetime
            ResetProjectile();
            // Initialize stats
            AI_UpdateOwnerAndProj();
        }
        public override void AI()
        {
            AI_StateTransition();
            AI_ShootThings();
            AI_UpdateOwnerAndProj();
            if (Projectile.owner == Main.myPlayer)
                Projectile.netUpdate = true;

            if (Timer++ >= WeaponUsetime)
                ResetProjectile();
            FirstFrame = false;
        }

        /// <summary>
        /// Called first in AI(). It can be used to implement weapons with multiple states. Check <see cref="Projectiles.Ranged.Flamethrowers.AstragelgunProj"/> for example.
        /// </summary>
        protected virtual void AI_StateTransition()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void AI_UpdateOwnerAndProj()
        {
            UpdateHoldOutProjectile(Owner, Projectile, Offset, RotationLerpAmount);
            Projectile.timeLeft++;

            Owner.itemAnimation = 2;
            Owner.itemTime = 2;
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.direction);
            float armRotation = (Projectile.rotation - MathHelper.PiOver2) * Owner.gravDir + (Owner.gravDir == -1 ? MathHelper.Pi : 0f);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
        }

        /// <summary>
        /// Place the shooting code here, note that it is not guaranteed to run on the owner's client
        /// </summary>
        protected abstract void AI_ShootThings();

        public override bool PreDraw(ref Color lightColor)
        {
            PreDraw_DrawHeldProj(TextureAssets.Projectile[Type].Value, lightColor);
            return false;
        }

        protected void PreDraw_DrawHeldProj(Texture2D texture, Color lightColor, Vector2? drawPos = null, Rectangle? source = null, Vector2? origin = null)
        {
            drawPos ??= Projectile.Center - Main.screenPosition;
            source ??= texture.Bounds;
            origin ??= source.Value.Size() / 2;
            SpriteEffects effects = (Projectile.spriteDirection * Owner.gravDir == 1) ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, drawPos.Value, source, lightColor, Projectile.rotation, origin.Value, Projectile.scale, effects, 0);
        }

        protected virtual void ResetProjectile()
        {
            if (ShouldKill)
                Projectile.Kill();
            else
            {
                Timer = 0;
                WeaponUsetime = CombinedHooks.TotalUseTime(SItem.useTime, Owner, SItem);
            }
        }

        /// <summary>
        /// Wrapping of <see cref="BaseHoldOutProjectile.GetInterpolatedRotationVector(Vector2, Vector2, float)"/>, directly updating the relevant data of the projectile.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="proj"></param>
        /// <param name="offset"></param>
        /// <param name="rotationLerp"></param>
        public static void UpdateHoldOutProjectile(Player player, Projectile proj, Vector2 offset, float rotationLerp = ArsenalLabsProjectile.ArsenalLabsFlamethrowerRotationLagParameter)
        {
            Vector2 ownerCenter = player.RotatedRelativePoint(player.MountedCenter);
            Vector2 armOffset = GetArmRotationCenter(proj.direction);
            Vector2 initVelo = proj.velocity;
            if (player.whoAmI == Main.myPlayer)
            {
                if (!GetLerpRotationVectorWithOffset(initVelo, ownerCenter + armOffset, offset, rotationLerp, out proj.velocity))
                    proj.velocity = GetInterpolatedRotationVector(initVelo, ownerCenter + armOffset, rotationLerp);
            }
            else
                proj.velocity = proj.velocity.SafeNormalize(Vector2.Zero);

            int olddir = proj.direction;
            proj.direction = proj.spriteDirection = proj.velocity.X > 0 ? 1 : -1;
            if (olddir != proj.direction)
            {
                // If the direction changes, update the projectile again to prevent a one-frame flicker when turning around
                armOffset = GetArmRotationCenter(proj.direction);
                if (player.whoAmI == Main.myPlayer)
                {
                    initVelo = proj.velocity;
                    if (!GetLerpRotationVectorWithOffset(initVelo, ownerCenter + armOffset, offset, rotationLerp, out proj.velocity))
                        proj.velocity = GetInterpolatedRotationVector(initVelo, ownerCenter + armOffset, rotationLerp);
                }
                else
                    proj.velocity = proj.velocity.SafeNormalize(Vector2.Zero);
            }

            proj.rotation = proj.velocity.ToRotation();
            Vector2 offsetUnit = proj.rotation.ToRotationVector2();
            proj.Center = ownerCenter + armOffset + (offsetUnit * offset.X + new Vector2(-offsetUnit.Y, offsetUnit.X) * offset.Y * proj.direction * player.gravDir) * proj.scale;
        }

        public static void AttachToProj(Player player, Projectile proj, Projectile shooter, Vector2 offset)
        {
            proj.Center = player.RotatedRelativePoint(player.MountedCenter) + GetArmRotationCenter(player.direction);
            proj.velocity = shooter.velocity.SafeNormalize(Vector2.Zero);
            proj.rotation = proj.velocity.ToRotation();
            proj.Center += (proj.velocity * offset.X + new Vector2(-proj.velocity.Y, proj.velocity.X) * offset.Y * shooter.direction * player.gravDir);
        }

        public static void ApplyOffset(Player player, Projectile proj, Vector2 offsetDir, Vector2 offset)
        {
            proj.Center += (offsetDir * offset.X + new Vector2(-offsetDir.Y, offsetDir.X) * offset.Y * proj.direction * player.gravDir);
        }
    }
}
