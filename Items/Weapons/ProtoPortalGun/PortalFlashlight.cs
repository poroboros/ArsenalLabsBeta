using ArsenalLabsBeta.Items.Flashlight;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.ProtoPortalGun
{


    public class PortalFlashlight : FlashlightConeFriendly
    {
        public override void SetDefaults()
        {
            itemOwnerType = ModContent.ItemType<ProtoPortalGun>();
            holdoutOffset = 55;
            base.SetDefaults();
        }
    }
}