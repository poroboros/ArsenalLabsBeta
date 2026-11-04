using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Buffs
{
    public class SuppressionField : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true; // set as a debuff so you can't right click to disable,
                                      // even though it reapplies every frame it's still better to prevent the option entirely
        }

        public override void Update(Player Player, ref int buffIndex)
        {
            Player.noBuilding = true;

            //disables flight and extra jumps
            Player.wingTime = 0;
            Player.carpetTime = 0;
            Player.rocketTime = 0;
            for (int i = 0; i < Player.ExtraJumps.Length; i++)
            {
                Player.ExtraJumps[i].Disable();
            }

            Player.RemoveAllGrapplingHooks();
            Player.moveSpeed = 1;
            if (Player.mount != null)
                Player.mount.Dismount(Player);
        }
    }
}
