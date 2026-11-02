using System;
using ArsenalLabsBeta.Common.EntitySources;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta
{
    public class ArsenalLabsProjectile : GlobalProjectile
    {
        public const float ArsenalLabsFlamethrowerRotationLagParameter = 0.89f;
        
        internal static void CombindHookShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            CombinedHooks.ModifyShootStats(player, item, ref position, ref velocity, ref type, ref damage, ref knockBack);
            CombinedHooks.Shoot(player, item, source, position, velocity, type, damage, knockBack);
        }
        internal static void CombindHookShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot source, Vector2 position, Vector2 velocity)
        {
            int type = item.shoot;
            int damage = item.damage;
            float knockBack = item.knockBack;
            CombindHookShoot(player, item, source, position, velocity, type, damage, knockBack);
        }
        internal static void CombindHookShoot(Player player, Item item, int usedAmmoItemID, Vector2 position, Vector2 velocity)
        {
            var source = player.ArsenalLabs().GetSource_ItemUse_WithPotentialAmmo_ShouldNotTriggerShoot(item, usedAmmoItemID) as EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot;
            CombindHookShoot(player, item, source, position, velocity);
        }

        internal static void CombindHookShoot(Player player, Item item, int usedAmmoItemID, int damage, Vector2 position, Vector2 velocity)
        {
            var source = player.ArsenalLabs().GetSource_ItemUse_WithPotentialAmmo_ShouldNotTriggerShoot(item, usedAmmoItemID) as EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot;
            int type = item.shoot;
            float knockBack = item.knockBack;
            CombindHookShoot(player, item, source, position, velocity, type, damage, knockBack);
        }

        #nullable enable
        internal static void DrawCircularGlow(Color activeColor, Projectile projectile, Rectangle frame, Vector2 origin, SpriteEffects effect = SpriteEffects.None, Texture2D? overrideTexture = null, float glowRadius = 4, int glowIterations = 12, int glowDivisor = 8, Vector2? offset = null, float extraRotation = 0)
        {
            if (offset == null)
            {
                offset = Vector2.Zero;
            }
            var texture = TextureAssets.Projectile[projectile.type].Value;
            if (overrideTexture != null)
            {
                texture = overrideTexture;
            }
            for (int i = 0; i < glowIterations; i++)
            {
                Color color = new Color(activeColor.R / glowDivisor, activeColor.G / glowDivisor, activeColor.B / glowDivisor, 0);
                Vector2 circular = new Vector2(glowRadius, 0).RotatedBy(MathHelper.ToRadians(i * 30 + Main.GameUpdateCount * 2f));
                Main.spriteBatch.Draw(texture, circular + projectile.Center - Main.screenPosition + (Vector2)offset, frame, color, projectile.rotation + extraRotation, origin, projectile.scale, effect, 0);
            }
        }
        #nullable disable
    }
}
