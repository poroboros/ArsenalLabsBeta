using System.Collections.Generic;
using ArsenalLabsBeta.WorldGeneration.Schematics.ArsenalFactory;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ArsenalLabsBeta.WorldGeneration
{
    public class ArsenalLabsGenManager : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {


            int finalCleanupIndex = tasks.FindIndex(g => g.Name == "Gem Depth Adjustment");
            if (finalCleanupIndex != -1)
            {
                string taskKey = "Mods.ArsenalLabsBeta.WorldGen.SurfaceLabTask";
                string taskName = Language.GetTextValue(taskKey);
                tasks.Insert(++finalCleanupIndex, new PassLegacy(taskName, SurfaceLab.PlaceSurfaceLab));
            }
        }
    }
}
