using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace NPCs.Arsenal.Projectiles;

public class ArsenalBomb : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 600; // 5 in-game frames per sprite frame. 12 sprite frames 
        Projectile.penetrate = 2;
        Projectile.scale = 1f;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;

        Projectile.Opacity = 0;

    }

    void Explode()
    {
        int type = ModContent.ProjectileType<ArsenalExplosion>();
        int proj = Projectile.NewProjectile(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Zero, type, Projectile.damage, 10f, Projectile.owner);
        Main.projectile[proj].friendly = false;

        Projectile.netUpdate = true;
        Projectile.Kill();
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity += new Vector2(0, .1f);

        Projectile.Opacity = Projectile.Opacity < 1 ? Projectile.Opacity + .05f : 1;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Vector2 foot = new Vector2(0, 32);

        Tile til = Main.tile[(Projectile.Center + foot).ToTileCoordinates()];


        if (til.IsTileSolid())
        {

            if (Projectile.penetrate < 2)
            {
                Explode();
            }
            else
            {
                Projectile.penetrate--;
            }
        }

        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > .01f)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > .01f)
        {
            Projectile.velocity.Y = -oldVelocity.Y;

        }
        Projectile.velocity *= 1.01f;


        Projectile.netUpdate = true;
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        Explode();
    }
}