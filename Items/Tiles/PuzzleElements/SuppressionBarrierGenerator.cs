using System.Collections.Generic;
using CalamityMod.Items.Placeables.DraedonStructures;
using ArsenalLabsBeta.Items.Unobtainables;
using ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArsenalLabsBeta.Items.Tiles.PuzzleElements
{
    public class SuppressionBarrierGenerator : ModTile
    {
        public static int CheckPlayerList;
        public static List<int> PlayersToDoEffects { get; private set; }

        public bool active = true;

        public int width = 20;
        public int height = 20;
        public int yOffset = 5;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
            TileObjectData.newTile.Width = 7;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            //TileObjectData.newTile.CoordinateHeights = new int[] { 24, 32, 32, 32, 32, 32, 24 };


            AnimationFrameHeight = 72;
            TileObjectData.addTile(Type);
            AddMapEntry(ArsenalLabsHelpers.FurnitureColor, CreateMapEntryName());
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 6)
            {
                frameCounter = 0;
                frame++;
                if (frame >= 8)
                {
                    frame = 0;
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            SurfaceLab.SuppressionActive = !SurfaceLab.SuppressionActive;

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;

            Main.cursorOverride = CursorOverrideID.Magnifiers;
            Main.cursorScale *= 1f;
            if (SurfaceLab.SuppressionActive)
                player.cursorItemIconID = ModContent.ItemType<UnLockFieldIcon>();
            else
            {
                player.cursorItemIconID = ModContent.ItemType<LockFieldIcon>();
            }

        }
    }
}
