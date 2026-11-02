using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace ArsenalLabsBeta.Common.EntitySources
{
    public class EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot : EntitySource_ItemUse_WithAmmo
    {
        public EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot(Player player, Item item, int ammoItemId, string context = null) : base(player, item, ammoItemId, context)
        {
        }
    }
}
