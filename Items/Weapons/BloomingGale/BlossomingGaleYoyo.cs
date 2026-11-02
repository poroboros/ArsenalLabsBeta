using System;
using System.Collections.Generic;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.BloomingGale
{
    public class BlossomingGaleYoyo : ModProjectile

    {

        //Based on Air Spinner Yoyo
        public const int MaxUpdates = 2;



        //Yoyo stats
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 30f * MaxUpdates;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 270f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 16f / MaxUpdates;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = MaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15 * MaxUpdates;
        }



        public int orbitalCount = 5;
        private bool firstTime = true;
        private int cooldown = -1;
        private int maxCooldown = 60;


        //rotate orbital petals
        private float orbitDistance = 60;
        private float thetaDegree = 0;
        private float ThetaAngle => MathHelper.ToRadians(thetaDegree);


        //stores orbital leaves
        private List<int> orbitalIndices = new List<int>();


        public override void AI()
        {


            //instantiate list with -1s
            if (firstTime)
            {
                for (int i = 0; i < orbitalCount; i++)
                {
                    orbitalIndices.Add(-1);
                }

                firstTime = false;
            }

            //reduce cooldown and increase rotation
            cooldown = (int)MathHelper.Max(-5, cooldown - 1);
            thetaDegree = thetaDegree < 360 ? thetaDegree + 1 : 0;


            //rotate the orbital leaves
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < orbitalCount; i++)
                {
                    int index = orbitalIndices[i];

                    if (index > -1 && index < 255)
                    {
                        Projectile proj = Main.projectile[index];

                        if (proj == null || !proj.active)
                        {
                            orbitalIndices[i] = -1;
                            continue;
                        }


                        //rotation is offset by it's index in the list
                        float rot = ThetaAngle + ((MathHelper.TwoPi / orbitalCount) * i);


                        Vector2 offset = new Vector2(MathHelper.Min(orbitDistance, proj.ai[2]), 0).RotatedBy(rot);

                        proj.Center = Projectile.Center + offset;

                        proj.rotation = rot + MathHelper.PiOver2;


                        //hopefully this just works in multiplayer
                        proj.netUpdate = true;
                    }
                }
            }

            //make orbitals wave in and out a bit
            orbitDistance += MathF.Sin(ThetaAngle) * .2f;

            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor,
                1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            //spawn new orbital on hit
            if (cooldown < 0)
            {
                SpawnOrbitingPetal();
                cooldown = maxCooldown;
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// Creates an orbital petal and assigns it to an index in orbitalIndices
        ///
        ///
        /// </summary>
        void SpawnOrbitingPetal()
        {

            for (int i = 0; i < orbitalIndices.Count; i++)
            {
                int index = orbitalIndices[i];
                if (index == -1)
                {
                    orbitalIndices[i] = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<BlossomGaleOrbital>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner,
                        ai0: Projectile.whoAmI, ai1: index);
                    break;

                }

            }
        }
    }

    //Orbital projectile
    //Very basic, most of the AI is actually controlled by the yoyo
    public class BlossomGaleOrbital : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.Opacity = 0;
        }

        //Have opacity fade in and kill it if yoyo is gone
        public override void AI()
        {
            Projectile.ai[2] += 1;
            Projectile.Opacity = MathHelper.Min(1, Projectile.Opacity + .02f);
            int index = (int)Projectile.ai[0];

            if (index < -1 || index > 255 || Main.projectile[index] == null || !Main.projectile[index].active)
            {
                Projectile.Kill();
            }

        }

        public override void OnKill(int timeLeft)
        {
            int index = (int)Projectile.ai[0];

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.Center, 5, 5, DustID.Plantera_Pink);
            }
        }
    }
}
