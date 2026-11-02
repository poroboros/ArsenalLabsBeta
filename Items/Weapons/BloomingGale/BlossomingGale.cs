using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using ArsenalLabsBeta.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.Melee
{
    public class BlossomingGale : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 12;
            Item.knockBack = 4f;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<BlossomingGaleYoyo>();
            Item.shootSpeed = 14f;

            Item.rare = ItemRarityID.Blue;
        }


    }
}
