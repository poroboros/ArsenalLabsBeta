using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Flashlight
{

    //Base used to draw the flamethrowers
    public abstract class FlashlightHoldOutProjectile : ModProjectile
    {

        public Color lightColor = Color.White;

        //References the Catalyst projectile holdouts
        internal void UpdateHeldProjDoVelocity(Player player, Vector2 rotatedRelativePoint, Projectile projectile, float? offsetAmount = null, float? lagSpeed = null)
        {
            ArsenalLabsProjectile.UpdateHeldProjDoVelocity(player, rotatedRelativePoint, projectile, offsetAmount, lagSpeed);
        }
        internal void UpdateHeldProjDoVelocity(Vector2 rotatedRelativePoint, float? offsetAmount = null, float? lagSpeed = null)
        {
            Player player = Main.player[Projectile.owner];
            ArsenalLabsProjectile.UpdateHeldProjDoVelocity(player, rotatedRelativePoint, Projectile, offsetAmount, lagSpeed);
        }

        internal void UpdateHeldProj(Player player, Vector2 rotatedRelativePoint, float offsetAmount, Projectile projectile, bool setTimeleft = true, bool updateArm = true)
        {
            ArsenalLabsProjectile.UpdateHeldProj(player, rotatedRelativePoint, offsetAmount, projectile, setTimeleft, updateArm);
        }

        internal void UpdateHeldProj(Vector2 rotatedRelativePoint, float offsetAmount, bool setTimeleft = true, bool updateArm = true)
        {
            Player player = Main.player[Projectile.owner];
            ArsenalLabsProjectile.UpdateHeldProj(player, rotatedRelativePoint, offsetAmount, Projectile, setTimeleft, updateArm);
        }

        //Default draw for holdout projectile
        internal void DrawHoldOutProjectile(Color lightColor, int? frames = null, Texture2D overrideTexture = null, float scaleMod = 1, Vector2? additionalOffset = null)
        {
            if (Projectile.hide)
                return;
            Texture2D texture = overrideTexture ?? TextureAssets.Projectile[Type].Value;
            additionalOffset ??= Vector2.Zero;
            Vector2 origin = texture.Size() / 2;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            /*
            if (Projectile.spriteDirection == -1)
            {
                origin.X = texture.Width - origin.X;
            }
            */

            Rectangle? rectFrame = frames is null ? null : new Rectangle(0, texture.Height / (int)frames * Projectile.frame, texture.Width, texture.Height / (int)frames);
            origin = rectFrame is null ? origin : rectFrame.Value.Size() / 2;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + (Vector2)additionalOffset, rectFrame, lightColor, Projectile.rotation, origin, Projectile.scale * scaleMod, effects, 0f);
            return;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int? frames = Main.projFrames[Projectile.type] > 1 ? Main.projFrames[Projectile.type] : null;
            DrawHoldOutProjectile(lightColor, frames);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
