using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Tools;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Flashlight
{
    public class WulfrumFlashlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 50;
            Item.height = 30;
            //Item.crit = 4;
            Item.knockBack = 2f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<FlashlightProj>();
            Item.shootSpeed = 8f;
            Item.noMelee = true;
            Item.autoReuse = true;
            //Item.channel = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.GetBuyPrice(Item.rare);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient<WulfrumMetalScrap>(5)
                .AddIngredient<EnergyCore>()
                .AddIngredient<WulfrumBattery>()
                .AddTile(TileID.Anvils); // possible recipe, just thought it would be cool to give iron an extra use
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ArsenalLabs().hasFlashlight)
            {
                return false;
            }
            player.ArsenalLabs().hasFlashlight = true;
            if (ArsenalLabsBeta.AprilFools)
            {
                SoundStyle toPlay = ArsenalLabsBeta.GetSound("Item/Flashlight/FlashlightCaramell").WithVolumeScale(1f);
                toPlay.MaxInstances = 1;
                SoundEngine.PlaySound(toPlay, player.Center);
            }
            return true;

        }
    }
}
