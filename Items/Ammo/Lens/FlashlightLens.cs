using System.Collections.Generic;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace ArsenalLabsBeta.Items.Ammo.Lens
{
    public class LensColors
    {
        public static Color WulfrumColor => new Color(186 / 255f * .7f, 255 / 255f * .7f, 249 / 255f * .6f);
        public static Color AmethystCol => new Color(179, 76, 239);
        public static Color TopazCol => new Color(218 / 255f * .8f, 220 / 255f * .7f, 0);
        public static Color EmeraldColor => new Color(79 / 255f * .8f, 255 / 255f * .8f, 89 / 255f * .8f);
        public static Color SapphireCol => new Color(79, 102, 255);
        public static Color RubyCol => new Color(255, 79, 79);
        public static Color DiamondCol => new Color(240 / 255f, 240 / 255f, 247 / 255f);
        public static Color AmberColor => new Color(255 / 255f * .9f, 145 / 255f * .9f, 79 / 255f * .9f);

        public static List<int> LensTypes = new List<int>
        {
            ModContent.ItemType<AmethystFlashlightLens>(),
            ModContent.ItemType<TopazFlashlightLens>(),
            ModContent.ItemType<SapphireFlashlightLens>(),
            ModContent.ItemType<EmeraldFlashlightLens>(),
            ModContent.ItemType<RubyFlashlightLens>(),
            ModContent.ItemType<DiamondFlashlightLens>(),
            ModContent.ItemType<AmberFlashlightLens>(),
            ModContent.ItemType<DiamondFlashlightLens>(),
            ModContent.ItemType<TeamFlashlightLens>(),
            ModContent.ItemType<RainbowFlashlightLens>()
        };

        public static Color GetColorByItemType(int type, Player owner)
        {
            if (type == ModContent.ItemType<AmethystFlashlightLens>())
                return AmethystCol;
            if (type == ModContent.ItemType<TopazFlashlightLens>())
                return TopazCol;
            if (type == ModContent.ItemType<SapphireFlashlightLens>())
                return SapphireCol;
            if (type == ModContent.ItemType<EmeraldFlashlightLens>())
                return EmeraldColor;
            if (type == ModContent.ItemType<RubyFlashlightLens>())
                return RubyCol;
            if (type == ModContent.ItemType<DiamondFlashlightLens>())
                return DiamondCol;
            if (type == ModContent.ItemType<AmberFlashlightLens>())
                return AmberColor;
            if (type == ModContent.ItemType<RainbowFlashlightLens>())
                return Main.DiscoColor;
            if (type == ModContent.ItemType<TeamFlashlightLens>())
            {
                switch (owner.team)
                {
                    case 0:
                        return DiamondCol;
                    case 1:
                        return RubyCol;
                    case 2:
                        return EmeraldColor;
                    case 3:
                        return SapphireCol;
                    case 4:
                        return TopazCol;
                    case 5:
                        return AmethystCol;
                }
            }
            return WulfrumColor;
        }
    }

    public class AmethystFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amethyst, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class TopazFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Topaz, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class RainbowFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<TopazFlashlightLens>())
                .AddIngredient(ModContent.ItemType<AmethystFlashlightLens>())
                .AddIngredient(ModContent.ItemType<DiamondFlashlightLens>())
                .AddIngredient(ModContent.ItemType<EmeraldFlashlightLens>())
                .AddIngredient(ModContent.ItemType<RubyFlashlightLens>())
                .AddIngredient(ModContent.ItemType<SapphireFlashlightLens>())
                .AddIngredient(ModContent.ItemType<TopazFlashlightLens>())
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SapphireFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class EmeraldFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Emerald, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class RubyFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ruby, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DiamondFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Diamond, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class TeamFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Green;
        }
    }

    public class AmberFlashlightLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amber, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SpelunkerChip : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            player.ArsenalLabs().spelunkChip = true;
        }


    }

    public class ApertureCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 1;
            Item.consumable = false; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            player.ArsenalLabs().aperCore = true;
        }
    }

}
