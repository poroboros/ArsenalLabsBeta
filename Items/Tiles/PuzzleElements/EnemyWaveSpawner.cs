using CalamityMod.Items.Placeables.DraedonStructures;
using ArsenalLabsBeta.Events;
using ArsenalLabsBeta.Events.ArsenalTrial;
using ArsenalLabsBeta.Items.Mounts;
using ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArsenalLabsBeta.Items.Tiles.PuzzleElements
{
    public class EnemyWaveSpawner : ModTile
    {
        public int width = 5;
        public int height = 3;
        public int yOffset = 5;

        public static int oldWaveIndex;

        public override void SetStaticDefaults()
        {
            MinPick = 1000;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.addTile(Type);
            AddMapEntry(CatalystHelpers.FurnitureColor, CreateMapEntryName());

            AnimationFrameHeight = 54;
        }

        private bool firstTime = true;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {

            if (((ArsenalFactoryCombatTrial.SpawnDelay > 0 && ArsenalFactoryCombatTrial.SpawnDelay < 18 * 6) || frame > 0))
            {
                if (firstTime)
                {
                    frame = 0;
                    frameCounter = 0;
                    firstTime = false;
                }


                frameCounter++;



                if (frameCounter > 6)
                {
                    frameCounter = 0;
                    frame++;
                    if (frame >= 18)
                    {
                        frame = 0;
                    }
                }
            }
        }

        public override bool RightClick(int i, int j)
        {

            if (WorldDefeats.arsenalTrialPhase == 0 && ArsenalFactoryCombatTrial.AttemptedPhase == 0)
            {
                ArsenalFactoryCombatTrial.AttemptedPhase = 1;
            }

            if (WorldDefeats.arsenalTrialPhase == 1 && ArsenalFactoryCombatTrial.AttemptedPhase == 1)
            {
                ArsenalFactoryCombatTrial.AttemptedPhase = 2;
            }

            if (WorldDefeats.arsenalTrialPhase > 2 && ArsenalFactoryCombatTrial.WaveIndex > 6)
            {
                ArsenalFactoryCombatTrial.WaveIndex = 0;
            }


            if (ArsenalFactoryCombatTrial.CurrentWave.Completed)
            {

                ArsenalFactoryCombatTrial.WaveIndex++;

                ArsenalFactoryCombatTrial.StartNewWave(new Point(i, j));
            }


            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            Main.cursorOverride = CursorOverrideID.Magnifiers;
            Main.cursorScale *= 1.5f;
            player.cursorItemIconID = ModContent.ItemType<JuggernautWheel>();

        }
    }
}
