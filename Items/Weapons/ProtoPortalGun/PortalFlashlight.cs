using ArsenalLabsBeta.Items.Weapons.Typeless;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.Typeless;

public class PortalFlashlight : FlashlightConeFriendly
{
    public override void SetDefaults()
    {
        itemOwnerType = ModContent.ItemType<ProtoPortalGun>();
        holdoutOffset = 55;
        base.SetDefaults();
    }
}
