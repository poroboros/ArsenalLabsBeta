using System.Collections.Generic;
using ArsenalLabsBeta.NPCs.Arsenal;
using ArsenalLabsBeta.NPCs.Arsenal.Miniboss;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Events.ArsenalTrial;


public class ArsenalWaveEmpty : ArsenalTrialWave
{
    public ArsenalWaveEmpty()
    {
        Completed = true;
    }

    public override void OnUpdate(int timer)
    {

    }

    public override void OnEnd()
    {

    }

    public override void OnStart()
    {


    }
}
public class ArsenalWaves1 : ArsenalTrialWave
{
    public ArsenalWaves1()
    {
        NeededEnemyKills = 3;
        EnemyQueue = new Queue<int>();



        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());

    }

    public override void OnStart()
    {
        ArsenalFactoryCombatTrial.BroadcastEventText("Mods.ArsenalLabsBeta.Announcement.ArsenalTrial1Start");
        ArsenalFactoryCombatTrial.BlockExits(true);


        base.OnStart();
    }
}

public class ArsenalWaves2 : ArsenalTrialWave
{

    public ArsenalWaves2()
    {
        NeededEnemyKills = 4;
        EnemyQueue = new Queue<int>();

        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
    }
}

public class ArsenalWaves3 : ArsenalTrialWave
{

    public ArsenalWaves3()
    {
        NeededEnemyKills = 4;
        EnemyQueue = new Queue<int>();

        EnemyQueue.Enqueue(ModContent.NPCType<InspectionCrawlerUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());

    }

    public override void OnEnd()
    {
        ArsenalFactoryCombatTrial.BroadcastEventText("Mods.ArsenalLabsBeta.Announcement.ArsenalTrial1End");
        if (WorldDefeats.arsenalTrialPhase < 3)
        {
            ArsenalFactoryCombatTrial.BlockExits(false);
            ArsenalFactoryCombatTrial.TrialActive = false;
            ArsenalFactoryCombatTrial.RemoveSpawner();
            WorldDefeats.arsenalTrialPhase =
                WorldDefeats.arsenalTrialPhase == 0 ? (byte)1 : WorldDefeats.arsenalTrialPhase;

            ArsenalFactoryCombatTrial.SpawnEnemies(ModContent.NPCType<ArsenalMinibossAnimationDummy>());
        }

        base.OnEnd();
    }
}

public class ArsenalWaves4 : ArsenalTrialWave
{

    public ArsenalWaves4()
    {
        NeededEnemyKills = 5;
        EnemyQueue = new Queue<int>();

        EnemyQueue.Enqueue(ModContent.NPCType<InspectionCrawlerUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());

    }

    public override void OnStart()
    {
        if (WorldDefeats.arsenalTrialPhase < 3)
        {
            ArsenalFactoryCombatTrial.BroadcastEventText("Mods.ArsenalLabsBeta.Announcement.ArsenalTrial1Start");
            ArsenalFactoryCombatTrial.BlockExits(true);
        }

        base.OnStart();
    }
}

public class ArsenalWaves5 : ArsenalTrialWave
{

    public ArsenalWaves5()
    {
        NeededEnemyKills = 7;
        EnemyQueue = new Queue<int>();

        EnemyQueue.Enqueue(ModContent.NPCType<InspectionCrawlerUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<InspectionCrawlerUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<PipeCleanerHead>());

    }

}

public class ArsenalWaves6 : ArsenalTrialWave
{

    public ArsenalWaves6()
    {
        NeededEnemyKills = 9;
        EnemyQueue = new Queue<int>();

        EnemyQueue.Enqueue(ModContent.NPCType<InspectionCrawlerUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<DisposalUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<InspectionCrawlerUnit>());
        EnemyQueue.Enqueue(ModContent.NPCType<PipeCleanerHead>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());
        EnemyQueue.Enqueue(ModContent.NPCType<RustCreep>());

    }

    public override void OnEnd()
    {
        WorldDefeats.arsenalTrialPhase = WorldDefeats.arsenalTrialPhase == 1 ? (byte)2 : WorldDefeats.arsenalTrialPhase;

        base.OnEnd();
    }
}

public class ArsenalWaves7 : ArsenalTrialWave
{

    public ArsenalWaves7()
    {
        NeededEnemyKills = 99;
        EnemyQueue = new Queue<int>();

        EnemyQueue.Enqueue(ModContent.NPCType<ArsenalJuggernaut>());

    }

    public override void OnStart()
    {
        ArsenalFactoryCombatTrial.RemoveSpawner();
        ArsenalFactoryCombatTrial.SpawnDelay = 50;
        base.OnStart();
    }

    public override void OnEnd()
    {
        WorldDefeats.arsenalTrialPhase = WorldDefeats.arsenalTrialPhase == 2 ? (byte)3 : WorldDefeats.arsenalTrialPhase;
        ArsenalFactoryCombatTrial.BroadcastEventText("Mods.ArsenalLabsBeta.Announcement.ArsenalTrial1End");
        ArsenalFactoryCombatTrial.TrialActive = false;
        ArsenalFactoryCombatTrial.BlockExits(false);
        ArsenalFactoryCombatTrial.ResetInvasion();

        base.OnEnd();
    }
}