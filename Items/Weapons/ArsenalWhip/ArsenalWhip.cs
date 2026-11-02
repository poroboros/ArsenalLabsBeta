using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using ArsenalLabsBeta.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.ArsenalWhip
{
    // ArsenalWhip is a placeholder name
    public class ArsenalWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            // projectileid, damage, knockback, shootspeed, animationtotaltime
            Item.DefaultToWhip(ModContent.ProjectileType<ArsenalWhipProjectile>(), Item.damage, 4, 8, 50);
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Blue;
        }

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
