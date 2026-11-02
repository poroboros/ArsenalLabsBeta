using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;


namespace ArsenalLabsBeta.Items.Mounts
{
    public class Juggernut : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 28;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.useAnimation = 17;
            Item.useTime = 120;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.consumable = true;
            Item.value = 0;
        }
        public override void UseItemFrame(Player player)
        {
            int time = CalamityUtils.SecondsToFrames(2160000f);

            if (player.itemAnimation == 1)
            {
                player.AddBuff(BuffID.Venom, time);
                player.AddBuff(BuffID.Poisoned, time);
                player.AddBuff(BuffID.Electrified, time);
                player.AddBuff(BuffID.Blackout, time);
                player.AddBuff(BuffID.Slow, time);
                player.AddBuff(BuffID.Weak, time);
                player.AddBuff(BuffID.Hunger, time);
                player.AddBuff(ModContent.BuffType<BurningBlood>(), time);
                player.AddBuff(ModContent.BuffType<Irradiated>(), time);
                player.AddBuff(ModContent.BuffType<WhisperingDeath>(), time);
            }
        }
    }
}
