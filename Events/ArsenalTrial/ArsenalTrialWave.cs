using System.Collections.Generic;
using Terraria;

namespace ArsenalLabsBeta.Events.ArsenalTrial
{

    public abstract class ArsenalTrialWave
    {
        public Queue<int> EnemyQueue = null;

        public int NeededEnemyKills = 0;
        public int SpawnFrequency = 5;

        public bool Completed = false;

        public ArsenalTrialWave? NextWave;

        public virtual void OnUpdate(int timer)
        {
            if (timer % SpawnFrequency == 0 && timer > 5 * 6)
            {
                if (EnemyQueue is not null)
                {
                    if (EnemyQueue.Count > 0)
                    {
                        ArsenalFactoryCombatTrial.SpawnEnemies(EnemyQueue.Dequeue());
                    }
                }
            }

            if (ArsenalFactoryCombatTrial.AccumulatedKillPoints < 1 && !Completed)
            {
                Completed = true;
                OnEnd();
            }
        }

        public virtual void OnStart()
        {

            ArsenalFactoryCombatTrial.NeededEnemyKills = NeededEnemyKills;
            ArsenalFactoryCombatTrial.AccumulatedKillPoints = NeededEnemyKills;
        }

        public virtual void OnEnd()
        {

        }
    }
}