using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using ArsenalLabsBeta.Items.BaseItem;
using ArsenalLabsBeta.Projectiles.Magic.Florapedia;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.Magic
{
    public class Florapedia : BaseHoldOutProjItem
    {

        private static Asset<Texture2D> glowTexture;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            if (!Main.dedServ)
                glowTexture = ModContent.Request<Texture2D>(this.GetPath() + "_Glow");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 13;
            Item.DamageType = DamageClass.Magic;
            Item.width = 32;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.knockBack = 7;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.GetBuyPrice(Item.rare);
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.mana = 5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlorapediaHoldout>();
            Item.shootSpeed = 12f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            float alpha = (float)Math.Sin(Main.timeForVisualEffects / 12) / 4 + 0.75f;
            Color color = Color.White * alpha * (alphaColor.A / 255f);
            Texture2D texture = glowTexture.Value;
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, texture.Bounds, color, rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);

            Lighting.AddLight(Item.Center, FlorapediaHoldout.ScreenColor.ToVector3() / 2 * alpha);
        }
    }
}
