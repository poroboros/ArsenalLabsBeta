using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.ArsenalWhip
{
    public class ArsenalMineExplosion : ModProjectile
    {
        public int frameCheck = 0;
        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 6;
        public override void SetDefaults()
        {
            Projectile.height = 17;
            Projectile.width = 17;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1.6f;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                frameCheck++;
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
            if (frameCheck >= Main.projFrames[Projectile.type])
            {
                frameCheck = 0;
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.67f, 0.88f) * 0.75f);

        }
    }
}
