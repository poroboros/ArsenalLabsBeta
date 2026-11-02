using System;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using ArsenalLabsBeta.Buffs.WhipTags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ArsenalLabsBeta.Projectiles.Summon.Whips.Arsenal;

namespace ArsenalLabsBeta.NPCs
{

    /// <summary>
    /// A seperate global NPC class to contain all whip effect code so ArsenalLabsNPC isn't bloated
    /// Call "ApplyTagDebuff()" to properly give a minion a tag
    /// </summary>
    public class ArsenalLabsTag : GlobalNPC
    {
        public override bool InstancePerEntity => true;


        public int tagType;
        public bool tagged;



        public int mineindex; // index of mine attached to tagged npc

        public override void SetDefaults(NPC entity)
        {
            mineindex = -1;
        }
        public override void ResetEffects(NPC NPC)
        {
            tagged = false;
        }


        //Draw animations for some tags
 

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {

            int removeType = ModContent.BuffType<DefaultTag>();

            //all whip "tag effects" occur here, including tag damage

            // How tf do I add Crit tag - ClockworkShade

            //only triggers a tag on a hit from a minion
            if (!projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]) && tagged)
            {
                ArsenalLabsPlayer arsenalLabsPlayer = Main.player[projectile.owner].ArsenalLabs();
                float summonMod = arsenalLabsPlayer.Player.GetDamage<SummonDamageClass>().Additive;


                if (arsenalLabsPlayer.SafeCheckProjectileCooldown(tagType)) //checks to make sure none of these are on cooldown
                {
                    
                    if (tagType == ArsenalLabsTagID.Arsenal)
                    {
                        modifiers.FlatBonusDamage += 3;
                        Projectile mine = Main.projectile[mineindex];
                        mine.timeLeft = (int)(mine.timeLeft * 0.5f); // reduce time left until explosion
                    }
                }
            }
        }

        /// <summary>
        /// Removes a buff without chance of index errors
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="buff"></param>
        static internal void SafeDeleteBuff(NPC npc, int buff)
        {
            int index = npc.FindBuffIndex(buff);
            if (index >= 0 && index < npc.buffImmune.Length)
            {
                npc.DelBuff(index);
            }
        }


        /// <summary>
        /// Puts a tag debuff on an enemy. Does not allow stacking, therefore we used the same debuff for each tag, though the debuff is only there to count it as tagged
        /// </summary>
        /// <param name="npc">The npc which the tag should apply to</param>
        /// <param name="tag">The tag to apply, use ArsenalLabsTagID</param>
        /// <param name="duration">Buff Duration</param>
        public static void ApplyTagDebuff(NPC npc, int tag, int duration)
        {
            int type = ModContent.BuffType<DefaultTag>();
            npc.buffImmune[type] = false;
            if (!npc.HasBuff(type))
            {
                npc.AddBuff(type, duration);
                npc.GetGlobalNPC<ArsenalLabsTag>().tagType = tag;
            }
        }

    }



    /// <summary>
    /// Class which contains the valid catalys tag IDs, used by ApplyTag Debuff
    /// </summary>
    public class ArsenalLabsTagID
    {
        public static int Arsenal = ModContent.ProjectileType<ArsenalWhipProjectile>();

    }

}
