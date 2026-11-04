using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;

using ArsenalLabsBeta.Common.EntitySources;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ArsenalLabsBeta.Biomes;


namespace ArsenalLabsBeta
{
    public class ArsenalLabsPlayer : ModPlayer
    {
        public static int ArsenalWhipHitCount;
        public bool aperCore;
        public bool spelunkChip;
        public bool hasFlashlight;

        public bool ZoneArsenal => Player.InModBiome(ModContent.GetInstance<ArsenalFactoryBiome>());

        public Dictionary<int, int> projectileCooldowns = new Dictionary<int, int>();


        /// <summary>
        /// Check if there is a cooldown for a certain projectile type
        /// </summary>
        /// <param name="key">key for the dictionary, typicially use a ModProjectile type</param>
        /// <param name="minValue">value that the cooldown must be less than to return true</param>
        /// <returns>returns true if the value is less than min value, or the key is not in the dictionary</returns>
        public bool SafeCheckProjectileCooldown(int key, int minValue = 0)
        {
            int value = 0;
            bool exists = projectileCooldowns.TryGetValue(key, out value);
            if (!exists)
                return true;
            return value < minValue;
        }
    

#nullable enable
        public IEntitySource GetSource_ItemUse_WithPotentialAmmo_ShouldNotTriggerShoot(Item item, int ammoItemId, string? context = null)
        => new EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot(Player, item, ammoItemId, context);
#nullable disable
    }
}