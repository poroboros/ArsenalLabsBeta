using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.Bloomhammer
{
    public class Bloomhammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<BloomhammerProj>();
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = ArsenalLabsBeta.Rogue;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
        }

        //Rogue weapons must have this or otherwise they don't get reforges
        public override bool WeaponPrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityGlobalItem calamityGlobalItem = base.Item.Calamity();

            if (player.Calamity().StealthStrikeAvailable() && calamityGlobalItem.Charge > 0)
            {
                //This thing becomes an absolute monstrosity if its damage isn't heavily nerfed for the Stealth Strike
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (p.WithinBounds(1000))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                return false;
            }

            return calamityGlobalItem.Charge > 0;


        }

    }
}
