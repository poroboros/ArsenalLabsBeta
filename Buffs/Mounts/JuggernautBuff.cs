using ArsenalLabsBeta.Items.Mounts;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ArsenalLabsBeta.Buffs.Mounts
{
    public class JuggernautBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(MountType<JuggernautMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
