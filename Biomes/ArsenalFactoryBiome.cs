using CalamityMod.Schematics;
using ArsenalLabsBeta.Buffs;
using ArsenalLabsBeta.Events;
using ArsenalLabsBeta.Events.ArsenalTrial;
using ArsenalLabsBeta.Items.Tiles.PuzzleElements;
using ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using EnemyWaveSpawner = ArsenalLabsBeta.Items.Tiles.PuzzleElements.EnemyWaveSpawner;

namespace ArsenalLabsBeta.Biomes
{
    public class ArsenalFactoryBiome : ModBiome
    {
        public override string BestiaryIcon => "ArsenalLabsBeta/Assets/UI/ArsenalIcon";

        // Play the Calamity BioLab music
        public override int Music => MusicLoader.GetMusicSlot(ArsenalLabsBeta.CalamityMusic, "Sounds/Music/BioLab");

        public bool NoPlayers = false;

        private bool FirstTime = true;

        public override bool IsBiomeActive(Player player)
        {

            Point pos = player.Center.ToTileCoordinates();



            return (SurfaceLab.MainFactoryBounds.Contains(pos) || SurfaceLab.RightTreasureCaveBounds.Contains(pos)
                    || SurfaceLab.LeftTreasureCaveBounds.Contains(pos) || SurfaceLab.CoreCaveBounds.Contains(pos));
        }


        public override void OnInBiome(Player player)
        {


            if (FirstTime)
            {
                FirstTime = false;
                RegenerateSpawner();
            }
            if (SurfaceLab.SuppressionActive)
            {
                player.AddBuff(ModContent.BuffType<SuppressionField>(), 20);
            }

            Point pos = player.Center.ToTileCoordinates();
            if ((SurfaceLab.Arena1Bounds.Contains(pos) && WorldDefeats.arsenalTrialPhase < 1)
                || (SurfaceLab.Arena2Bounds.Contains(pos) && WorldDefeats.arsenalTrialPhase < 2))
            {
                player.AddBuff(ModContent.BuffType<SuppressionField>(), 20);
            }

            NoPlayers = true;
            foreach (var play in Main.player)
            {
                if (play.active)
                {
                    Point placement = new Point(SurfaceLab.FactoryX, SurfaceLab.FactoryY + SurfaceLab.ArenaHeight - 2);
                    if (WorldDefeats.arsenalTrialPhase < 1)
                    {
                        placement.Y += SurfaceLab.Arena1Depth;
                    }
                    else
                    {
                        placement.Y += SurfaceLab.Arena2Depth;
                    }

                    if (play.Distance(placement.ToWorldCoordinates()) < 1000)
                    {
                        NoPlayers = false;

                    }
                }
            }

            if (NoPlayers)
            {
                //ArsenalFactoryCombatTrial.TrialActive = false;
                RegenerateSpawner();

            }



        }

        public override void OnLeave(Player player)
        {
            NoPlayers = true;

            foreach (var play in Main.player)
            {
                if (play.active)
                {
                    Vector2 pos = new Point(SurfaceLab.FactoryX, SurfaceLab.FactoryY + SurfaceLab.Arena1Depth)
                        .ToWorldCoordinates();
                    if (play.Distance(pos) < 1000)
                    {
                        NoPlayers = false;

                    }
                }
            }

            if (NoPlayers)
            {
                RegenerateSpawner();

            }
        }

        public static void RegenerateSpawner()
        {

            if (!SurfaceLab.HasFactory || WorldDefeats.arsenalTrialPhase > 2) //Don't run this in a world without the arsenal factory
                return;

            ArsenalFactoryCombatTrial.BlockExits(false);
            ArsenalFactoryCombatTrial.ResetInvasion();
            Point placement = new Point(SurfaceLab.FactoryX, SurfaceLab.FactoryY + SurfaceLab.ArenaHeight - 2);
            if (WorldDefeats.arsenalTrialPhase < 1)
            {
                placement.Y += SurfaceLab.Arena1Depth;
            }
            else
            {
                placement.Y += SurfaceLab.Arena2Depth;
            }

            if (Main.tile[placement].TileType == ModContent.TileType<EnemyWaveSpawner>() && Main.tile[placement].HasTile)
            {
                return;
            }
            var specialCondition = false;
            SchematicManager.PlaceSchematic("EnemySpawner", placement, SchematicAnchor.Center, ref specialCondition, ChestFunctionDummy);


        }


        //Just here to make the schematic manager happy, probably a better way to do this lol
        static void ChestFunctionDummy(Chest chest, int Type, bool firstItem)
        {

        }
    }
}
