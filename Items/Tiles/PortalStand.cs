using ArsenalLabsBeta.Items.Weapons;
using ArsenalLabsBeta.Items.Weapons.ProtoPortalGun;
using ArsenalLabsBeta.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArsenalLabsBeta.Items.Tiles
{
    public class PortalStand : ModTile
    {

        public int width = 3;
        public int height = 3;
        public int yOffset = 5;

        public override void SetStaticDefaults()
        {
            MinPick = 45;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileSpelunker[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };

            TileObjectData.addTile(Type);
            AddMapEntry(ArsenalLabsHelpers.FurnitureColor, CreateMapEntryName());
            RegisterItemDrop(ModContent.ItemType<ProtoPortalGun>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<ProtoPortalGun>(), Type, 0);
        }
        public override void MouseOver(int i, int j)
        {
            var Player = Main.LocalPlayer;
            Player.noThrow = 2;
            Player.cursorItemIconEnabled = true;
            Player.cursorItemIconID = ModContent.ItemType<ProtoPortalGun>();
        }
        public override bool RightClick(int i, int j)
        {
            WorldGen.KillTile(i, j);
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            ArsenalLabsTile.QuickGlowmask(this, spriteBatch, i, j);
        }
    }
}
