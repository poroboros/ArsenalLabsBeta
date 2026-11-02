
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.BaseProjectiles
{
    public abstract class BaseHoldOutProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public Item SItem => Owner.ActiveItem();

        /// <summary>
        /// Main.myPlayer == Projectile.owner
        /// </summary>
        public bool OwnerClient => Main.myPlayer == Projectile.owner;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
        }

        /// <summary>
        /// Kill the projectile immediately if returns to false.
        /// </summary>
        /// <returns></returns>
        protected virtual bool AI_ProjectileIsActive() => OwnerCheck() && ProjCheck();

        public override bool PreAI()
        {
            if (!AI_ProjectileIsActive())
            {
                Projectile.Kill();
                return false;
            }
            return true;
        }



        /// <summary>
        /// In ArsenalLabs, the behavior of most holdout projectiles is determined by a directional vector. This is always <see cref="Entity.velocity"/> with <see cref="ModProjectile.ShouldUpdatePosition"/> overridden to return false. 
        /// This method interpolates between the initial orientation vector and the vector pointing from player.center to the target. The interpolation simulates the projectile's rotation towards the target point. 
        /// It's crucial to execute this method client-side; thus, confirm that <see cref="Projectile.owner"/> == <see cref="Main.myPlayer"/> before proceeding.
        /// </summary>
        /// <param name="initialOrientation">Initial orientation vector, normalization is not required</param>
        /// <param name="center">Center of the projectile's owner</param>
        /// <param name="rotationLerp">Interpolation factor, accepts a value between 0 and 1. Values outside this range are permissible if you understand the implications. A smaller value results in quicker rotation; 0 implies instantaneous rotation towards the cursor. The default value is 0.89, which is the standard for most ArsenalLabs holdout flamethrowers.</param>
        /// <returns>Rotated vector, normalized</returns>
        public static Vector2 GetInterpolatedRotationVector(Vector2 initialOrientation, Vector2 center, float rotationLerp = ArsenalLabsProjectile.ArsenalLabsFlamethrowerRotationLagParameter)
        {
            Vector2 aim = (Main.MouseWorld - center).SafeNormalize(-Vector2.UnitY);
            return Vector2.Lerp(aim, initialOrientation.SafeNormalize(Vector2.Zero), rotationLerp).SafeNormalize(Vector2.Zero);
        }

        public static bool GetLerpRotationVectorWithOffset(Vector2 initialOrientation, Vector2 center, Vector2 offset, float rotationLerp, out Vector2 resultVector)
        {
            resultVector = Vector2.Zero;
            Vector2 target = Main.MouseWorld;
            Vector2 aim = target - center;
            float length = (center - target).Length();
            float radius = Math.Abs(offset.Y);
            if (Math.Abs(aim.X) < 2 * radius || radius == 0)
                return false;
            float theta = aim.ToRotation();
            float phi = MathF.Acos(radius / length);
            float angle = theta + (phi - MathHelper.PiOver2) * (aim.X > 0.1f ? -1 : 1);
            resultVector = Vector2.Lerp(angle.ToRotationVector2(), initialOrientation.SafeNormalize(Vector2.Zero), rotationLerp).SafeNormalize(Vector2.Zero);
            return true;
        }

        public static Vector2 GetArmRotationCenter(int direction) => new Vector2(-4 * direction, -2);

        protected bool OwnerCheck()
        {
            return Owner.active && !Owner.CCed && !Owner.dead && !Owner.noItems;
        }

        protected bool ProjCheck()
        {
            return SItem.shoot == Type && Owner.ownedProjectileCounts[Type] <= 1;
        }
    }
}
