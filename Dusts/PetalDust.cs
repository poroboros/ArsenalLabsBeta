using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabs.Dusts
{
    public class PetalDust : ModDust
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.FlowerPetal}";
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 20 * Main.rand.Next(3), 20, 20);
            dust.alpha = 80;
            dust.scale = Main.rand.NextFloat(1f, 1.3f);
            dust.color = Color.White;
            dust.fadeIn = Main.rand.NextFloat(-0.02f, 0.02f);
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn *= 0.995f;
            dust.velocity = dust.velocity.RotatedBy(dust.fadeIn);
            dust.velocity *= 0.94f;
            dust.velocity.Y += 0.08f;
            dust.scale *= 0.99f;
            dust.alpha += 2;
            if (dust.alpha >= 255)
                dust.active = false;
            dust.rotation = dust.velocity.ToRotation();
            dust.position += dust.velocity;
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Color color = Color.White with { A = 50 };
            Main.EntitySpriteDraw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, color * ((255f - dust.alpha) / 255), dust.rotation, new Vector2(10f), dust.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
