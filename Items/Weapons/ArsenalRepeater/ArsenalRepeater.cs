using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using ArsenalLabsBeta.Items.BaseItem;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ArsenalLabsBeta.Projectiles.Ranged;

namespace ArsenalLabsBeta.Items.Weapons.Ranged.Repeaters
{

    public class ArsenalRepeater : BaseHoldOutProjItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            //Since this is a new weapon, these values are placeholder.
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 7;
            Item.shootSpeed = 10;
            Item.useAmmo = AmmoID.Arrow;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.value = CalamityGlobalItem.GetBuyPrice(Item.rare);
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArsenalRepeaterProj>();
        }
        public override bool AltFunctionUse(Player Player)
        {
            return true;
        }
    }
}
