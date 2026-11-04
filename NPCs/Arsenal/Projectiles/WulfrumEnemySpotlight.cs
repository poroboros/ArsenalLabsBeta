using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal.Projectiles
{
    public class WulfrumEnemySpotlight : SpotlightBase
    {

        public override void SetFlashlightDefaults()
        {
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.netImportant = true;

            Projectile.hide = true;
        }

        public override void AI()
        {
        }
    }
}
