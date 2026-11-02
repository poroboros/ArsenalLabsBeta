using System.Collections.Generic;
using ArsenalLabsBeta;
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using static Terraria.GameContent.ItemDropRules.Conditions;

namespace ArsenalLabsBeta.NPCs
{
    public class ArsenalLabsNPC : GlobalNPC
    {
        /// <summary>
        /// Gets the damage of a NPC prior Expert, Master, and Journey modifiers. Good for calculating projectile Damage. Ported from SOTS
        /// </summary>  
        public static int GetBaseDamage(NPC npc)
        {
            return (int)(npc.damage / (Main.GameModeInfo.EnemyDamageMultiplier * npc.strengthMultiplier));
        }
        public override bool InstancePerEntity => true;

        public static HashSet<int> IsWormBody { get; private set; }
        public static HashSet<int> IsWormSegment { get; private set; }
    


        public override void Load()
        {

            IsWormBody = new HashSet<int>()
                {
                    NPCID.EaterofWorldsBody,
                    NPCID.BoneSerpentBody,
                    NPCID.CultistDragonBody1,
                    NPCID.CultistDragonBody2,
                    NPCID.CultistDragonBody3,
                    NPCID.CultistDragonBody4,
                    NPCID.DevourerBody,
                    NPCID.DiggerBody,
                    NPCID.DuneSplicerBody,
                    NPCID.EaterofWorldsBody,
                    NPCID.GiantWormBody,
                    NPCID.LeechBody,
                    NPCID.SeekerBody,
                    NPCID.SolarCrawltipedeBody,
                    NPCID.StardustWormBody,
                    NPCID.TheDestroyerBody,
                    NPCID.TombCrawlerBody,
                    NPCID.WyvernBody,
                    NPCID.WyvernBody2,
                    NPCID.WyvernBody3,
                    NPCID.BoneSerpentTail,
                    NPCID.CultistDragonTail,
                    NPCID.DevourerTail,
                    NPCID.DiggerTail,
                    NPCID.DuneSplicerTail,
                    NPCID.EaterofWorldsTail,
                    NPCID.GiantWormTail,
                    NPCID.LeechTail,
                    NPCID.SeekerTail,
                    NPCID.SolarCrawltipedeTail,
                    NPCID.StardustWormTail,
                    NPCID.TheDestroyerTail,
                    NPCID.TombCrawlerTail,
                    NPCID.WyvernTail,
                    ModContent.NPCType<GulperEelBody>(),
                    ModContent.NPCType<GulperEelBodyAlt>(),
                    ModContent.NPCType<GulperEelTail>(),
                    ModContent.NPCType<DesertScourgeBody>(),
                    ModContent.NPCType<DesertScourgeTail>(),
                    ModContent.NPCType<DesertNuisanceBody>(),
                    ModContent.NPCType<DesertNuisanceTail>(),
                    ModContent.NPCType<PerforatorBodySmall>(),
                    ModContent.NPCType<PerforatorTailSmall>(),
                    ModContent.NPCType<PerforatorBodyMedium>(),
                    ModContent.NPCType<PerforatorTailMedium>(),
                    ModContent.NPCType<PerforatorBodyLarge>(),
                    ModContent.NPCType<PerforatorTailLarge>(),
                    ModContent.NPCType<AquaticScourgeBody>(),
                    ModContent.NPCType<AquaticScourgeBodyAlt>(),
                    ModContent.NPCType<AquaticScourgeTail>(),
                    ModContent.NPCType<AstrumDeusBody>(),
                    ModContent.NPCType<AstrumDeusTail>(),
                    ModContent.NPCType<StormWeaverBody>(),
                    ModContent.NPCType<StormWeaverTail>(),
                    ModContent.NPCType<StormWeaverBody>(),
                    ModContent.NPCType<StormWeaverTail>(),
                    ModContent.NPCType<DevourerofGodsBody>(),
                    ModContent.NPCType<DevourerofGodsTail>(),
                    ModContent.NPCType<SupremeCalamitas>(),
                    ModContent.NPCType<SoulSeekerSupreme>(),
                    ModContent.NPCType<SepulcherArm>(),
                    ModContent.NPCType<SepulcherBodyEnergyBall>(),
                    ModContent.NPCType<SepulcherBody>(),
                    ModContent.NPCType<SepulcherTail>(),
                    ModContent.NPCType<ThanatosBody1>(),
                    ModContent.NPCType<ThanatosBody2>(),
                    ModContent.NPCType<ThanatosTail>(),
                    ModContent.NPCType<EidolonWyrmHead>(),
                    ModContent.NPCType<EidolonWyrmBody>(),
                    ModContent.NPCType<EidolonWyrmBodyAlt>(),
                    ModContent.NPCType<EidolonWyrmTail>(),
                    ModContent.NPCType<PrimordialWyrmHead>(),
                    ModContent.NPCType<PrimordialWyrmBodyAlt>(),
                    ModContent.NPCType<PrimordialWyrmBody>(),
                    ModContent.NPCType<PrimordialWyrmTail>(),
                };

            IsWormSegment = new HashSet<int>(IsWormBody)
                {
                    NPCID.BoneSerpentHead,
                    NPCID.CultistDragonHead,
                    NPCID.DevourerHead,
                    NPCID.DiggerHead,
                    NPCID.DuneSplicerHead,
                    NPCID.EaterofWorldsHead,
                    NPCID.GiantWormHead,
                    NPCID.LeechHead,
                    NPCID.SeekerHead,
                    NPCID.SolarCrawltipedeHead,
                    NPCID.StardustWormHead,
                    NPCID.TombCrawlerHead,
                    NPCID.WyvernHead,
                    NPCID.TheDestroyer,
                    ModContent.NPCType<GulperEelHead>(),
                    ModContent.NPCType<DesertScourgeHead>(),
                    ModContent.NPCType<DesertNuisanceHead>(),
                    ModContent.NPCType<PerforatorHeadSmall>(),
                    ModContent.NPCType<PerforatorHeadMedium>(),
                    ModContent.NPCType<PerforatorHeadLarge>(),
                    ModContent.NPCType<AquaticScourgeHead>(),
                    ModContent.NPCType<AstrumDeusHead>(),
                    ModContent.NPCType<StormWeaverHead>(),
                    ModContent.NPCType<DevourerofGodsHead>(),
                    ModContent.NPCType<ThanatosHead>(),
                    ModContent.NPCType<EidolonWyrmHead>(),
                    ModContent.NPCType<PrimordialWyrmHead>(),
                };



           
          
        }





        public static int FindTarget_Basic(Vector2 center, float minDistance = 2000f, object attacker = null)
        {
            return FindTarget_Basic(center, out float _, minDistance, attacker);
        }

        public static int FindTarget_Basic(Vector2 center, out float dist, float minDistance = 2000f, object attacker = null)
        {
            int target = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(attacker))
                {
                    float distance = (center - Main.npc[i].Center).Length();
                    if (distance < minDistance)
                    {
                        target = i;
                        minDistance = distance;
                    }
                }
            }
            dist = minDistance;
            return target;
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            var Player = spawnInfo.Player;
            var arsenal = Player.ArsenalLabs();
            var calamity = Player.Calamity();


            //arsenal event spawning
            // if (arsenal.ZoneArsenal)
            // {
            //     pool.Clear();
            //     // pool.Add(ModContent.NPCType<DisposalUnitNaturalSpawn>(), .8f);

            // }


        }





        // public override void OnKill(NPC npc)
        // {
        //     if (ArsenalFactoryCombatTrial.TrialActive)
        //         ArsenalFactoryCombatTrial.OnEnemyKill(npc);
        //     ;
        // }
    }
}
