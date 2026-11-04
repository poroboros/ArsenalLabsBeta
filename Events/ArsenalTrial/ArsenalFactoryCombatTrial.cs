using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Schematics;
using ArsenalLabsBeta.Biomes;
using ArsenalLabsBeta.Items.Tiles.PuzzleElements;
using ArsenalLabsBeta.NPCs.Arsenal;
using ArsenalLabsBeta.NPCs.Arsenal.Miniboss;
using ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using EnemyWaveSpawner = ArsenalLabsBeta.Items.Tiles.PuzzleElements.EnemyWaveSpawner;

namespace ArsenalLabsBeta.Events.ArsenalTrial;

//All of this is modified off of the calamity acid rain code :P

public class ArsenalFactoryCombatTrial : ModSystem
{
    public static bool TrialActive;

    public static int SpawnDelay = 0;

    public static int AccumulatedKillPoints;

    public static int NeededEnemyKills;

    public static int AttemptedPhase;

    public static float ArsenalCompletionRatio => MathHelper.Clamp(AccumulatedKillPoints / (float)NeededEnemyKills, 0f, 1f);

    public static List<int> possibleEnemies = new();

    public static int WaveIndex;

    public static List<ArsenalTrialWave> WaveObjects = null;

    public static ArsenalTrialWave CurrentWave => WaveObjects[WaveIndex] ?? null;

    public static Color TextColor = Color.LightCoral;

    public static Point SpawnerPos;

    public static void BroadcastEventText(string localizationKey) => CalamityUtils.BroadcastLocalizedText(localizationKey, new Color?(TextColor));



    public override void OnModLoad()
    {
        possibleEnemies = new List<int>()
        {
            ModContent.NPCType<RustCreep>(),
            ModContent.NPCType<DisposalUnit>(),
            ModContent.NPCType<InspectionCrawlerUnit>(),
            ModContent.NPCType<PipeCleanerHead>(),
            ModContent.NPCType<ArsenalJuggernaut>(),
        };
    }

    public override void PostUpdateWorld()
    {

        SpawnDelay++;
        if (TrialActive)
        {

            CurrentWave.OnUpdate(SpawnDelay);
        }
    }

    public override void OnWorldLoad()
    {
        ResetInvasion();
        AttemptedPhase = 0;

    }

    public static void ResetInvasion()
    {
        TrialActive = false;


        WaveObjects = new List<ArsenalTrialWave>
        {
            new ArsenalWaveEmpty(),
            new ArsenalWaves1(),
            new ArsenalWaves2(),
            new ArsenalWaves3(),
            new ArsenalWaves4(),
            new ArsenalWaves5(),
            new ArsenalWaves6(),
            new ArsenalWaves7(),

        };

        int defeats = WorldDefeats.arsenalTrialPhase;

        if (defeats == 0)
        {
            WaveIndex = 0;
        }
        else if (defeats == 1)
        {
            WaveIndex = 3;
            CurrentWave.Completed = true;
        }
        else if (defeats == 2)
        {
            WaveIndex = 6;
            CurrentWave.Completed = true;
        }
    }



    public static void StartNewWave(Point pos)
    {
        SpawnDelay = 0;
        SpawnerPos = pos;
        TrialActive = true;
        CurrentWave.OnStart();
    }

    public static void RemoveSpawner()
    {
        for (int i = -4; i < 4; i++)
        {
            for (int j = -2; j < 4; j++)
            {

                if (Main.tile[SpawnerPos + new Point(i, j)].TileType == ModContent.TileType<EnemyWaveSpawner>())
                    Main.tile[SpawnerPos + new Point(i, j)].Active(false);
            }
        }
    }

    public static void PlaceSpawner(Point placement)
    {
        bool specialCondition = false;
        SchematicManager.PlaceSchematic("EnemySpawner", placement, SchematicAnchor.Center, ref specialCondition, ChestFunctionDummy);
    }

    static void ChestFunctionDummy(Chest chest, int Type, bool firstItem)
    {

    }

    public static void BlockExits(bool block)
    {

        block = !block;

        if (block)
        {
            if (AttemptedPhase == 0 && WorldDefeats.arsenalTrialPhase == 0)
            {
                return;
            }
            if (AttemptedPhase == 1 && WorldDefeats.arsenalTrialPhase == 1)
            {
                return;
            }
        }
        // This should be around the center of the arena, that way it unlocks all exists
        int pointX = SurfaceLab.FactoryX - (SurfaceLab.FactoryWidth)/2;
        int pointY = WorldDefeats.arsenalTrialPhase == 0 ? SurfaceLab.FactoryY + SurfaceLab.Arena1Depth : SurfaceLab.FactoryY + SurfaceLab.Arena2Depth;

        Point corner = new Point(pointX, pointY);

        for (int i = -20; i < SurfaceLab.FactoryWidth + 20; i++)
        {
            for (int j = -5; j < SurfaceLab.ArenaHeight + 5; j++)
            {
                Point pos = new Point(corner.X + i, corner.Y + j);

                //Debug, but its kinda cool having the arena flash in a big electric wave, thoughts?
                //Dust.NewDust(pos.ToWorldCoordinates(), 5, 5, DustID.Electric);
                if (Main.tile[pos] != null && Main.tile[pos].HasActuator)
                {
                    Tile tile = Main.tile[pos];
                    tile.IsActuated = block;
                }
            }

        }
    }

    public static void OnEnemyKill(NPC npc)
    {

        if (TrialActive)
        {


            if (possibleEnemies.Contains(npc.type))
            {

                AccumulatedKillPoints -= 1;
            }

            if (npc.type == ModContent.NPCType<ArsenalJuggernaut>())
            {

                AccumulatedKillPoints = 0;
            }


        }


        AccumulatedKillPoints = (int)MathHelper.Max(0, AccumulatedKillPoints); // To prevent negative completion ratios
    }

    public static void SpawnEnemies(int enemyType)
    {

        if (Main.dedServ)
            return;


        int randDir = Main.rand.NextBool() ? 1 : -1;
        int randX = 0;
        int randY = 0;


        Vector2 spawnPos = new Point(SpawnerPos.X + randX,
                    SpawnerPos.Y + randY).ToWorldCoordinates();


        for (int i = 0; i < 3; i++)
            Dust.NewDust(spawnPos, 5, 5, DustID.Electric);
        NPC.NewNPCDirect(new EntitySource_SpawnNPC(), spawnPos, enemyType);



    }
}
