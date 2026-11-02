using System;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.Bloomhammer
{
    // BloomPetal is a placeholder name

    public class BloomPetal : ModProjectile
    {

        public override void SetDefaults()
        {

            Projectile.scale = 1f;

            Projectile.tileCollide = true;
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.Opacity = .8f;

            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;


        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }

        private bool reverse = false;

        private bool firstTime = true;

        public delegate float trigFunction(float x);

        public trigFunction func;
        public override void AI()

        {
            if (Projectile.timeLeft < 200)
                Projectile.Opacity -= .005f;

            if (Main.tile[Projectile.Center.ToTileCoordinates()].IsTileSolid() || Projectile.Opacity < 0)
                Projectile.Kill();

            if (firstTime)
            {
                Projectile.ai[2] = Main.rand.NextFloat(.1f, .2f);

                //me learning how delegates works
                //Main.NewText((int)(Projectile.ai[2]*100));

                //randomly chooses if this is a sin or cos motion
                if (((int)(Projectile.ai[2] * 100)) % 2 == 0)
                {
                    func = delegate (float x)
                    {
                        return MathF.Sin(x);
                    };
                }
                else
                {
                    func = delegate (float x)
                    {
                        return MathF.Cos(x);
                    };
                }


                Projectile.netUpdate = true;

                firstTime = false;
            }

            //fall speed
            Projectile.position.Y += Projectile.ai[2] * 4;

            //move back and forth
            Projectile.position.X += (func(Projectile.ai[0]) + (1 * Main.windSpeedCurrent)) * 4;

            //make em wiggle a bit
            Projectile.rotation = -(func(Projectile.ai[0]) + (1 * Main.windSpeedCurrent));


            //This is so the "velocity" of the leaf itself is variable
            Projectile.ai[0] += MathF.Sin((Projectile.ai[1] / (60f) * Projectile.ai[2]));



            //This section makes the "velocity" go back and forth
            Projectile.ai[1] = reverse ? Projectile.ai[1] - .2f : Projectile.ai[1] + .2f;

            if (Projectile.ai[1] > 10)
            {
                reverse = true;
            }
            if (Projectile.ai[1] < -10)
            {
                reverse = false;
            }



        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.Opacity < .2f)
                return;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.Center, 5, 5, DustID.Plantera_Pink);
            }
        }
    }
}
