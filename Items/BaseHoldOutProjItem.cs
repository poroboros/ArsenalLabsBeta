using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArsenalLabsBeta.Common.EntitySources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.BaseItem
{
    public abstract class BaseHoldOutProjItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[Item.shoot] == 0 && source is not EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot)
            {
                Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
