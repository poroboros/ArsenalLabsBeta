using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using Terraria.Audio;
using Terraria.ModLoader;

namespace ArsenalLabsBeta
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class ArsenalLabsBeta : Mod
	{

		public const string AssetsPath = "ArsenalLabsBeta/Assets/";
		public const string SoundsPath = AssetsPath + "Sounds/";
        public static ArsenalLabsBeta Instance { get; private set; }
        public static CalamityMod.CalamityMod Calamity { get; private set; }

		public static RogueDamageClass Rogue => ModContent.GetInstance<RogueDamageClass>();

		public static DateTime dateTime;
        public static bool AprilFools;

		public override void Load()
		{
			Instance = ModContent.GetInstance<ArsenalLabsBeta>();
			Calamity = ModContent.GetInstance<CalamityMod.CalamityMod>();

			AprilFools = dateTime.Month == 4 && dateTime.Day == 1;
		}

		public static SoundStyle GetSound(string name)
        {
            return new SoundStyle(SoundsPath + name);
        }
	}
}
