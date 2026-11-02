using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CalamityMod.Schematics;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.WorldGeneration.Schematics
{
    // We need a struct and their answer was "use reflection" but maybe just copying this from main Cal can work
    // IT FUCKING DOES!!!! - ClockworkShade
    public struct ChestItem
    {
        internal int Type;
        internal int Stack;
        internal ChestItem(int type, int stack)
        {
            Type = type;
            Stack = stack;
        }
    }

    public class ArsenalLabsSchematicLoaderSystem : ModSystem
    {


        //TODO Make this not hardcoded lol
        string schematicDir = "WorldGeneration/Schematics/";

        //These lines are based of the infernum mod
        /// <summary>
        /// Accesses the calamity schematic dictionary
        /// </summary>
        internal static Dictionary<string, SchematicMetaTile[,]> TileMaps =>
            typeof(SchematicManager).GetField("TileMaps", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, SchematicMetaTile[,]>;

        internal static readonly MethodInfo ImportSchematicMethod = typeof(CalamitySchematicIO).GetMethod("ImportSchematic", BindingFlags.NonPublic | BindingFlags.Static);

        public override void OnModLoad()
        {
            //Load a new structure into the calamity tileMap Dictionary
            TileMaps["ArsenalLabMain"] = LoadExternalSchematic(schematicDir + "ArsenalFactory/FactoryMid.csch");
            TileMaps["ArsenalLabTop"] = LoadExternalSchematic(schematicDir + "ArsenalFactory/FactoryTop.csch");
            TileMaps["ArsenalLabBottom"] = LoadExternalSchematic(schematicDir + "ArsenalFactory/FactoryBottom.csch");

            TileMaps["EnemySpawner"] = LoadExternalSchematic(schematicDir + "ArsenalFactory/EnemySpawner.csch");
            TileMaps["BarrierGenerator"] = LoadExternalSchematic(schematicDir + "ArsenalFactory/BarrierGenerator.csch");


            TileMaps["LabAboveClearance"] = LoadExternalSchematic(schematicDir + "ArsenalFactory/LabAboveClearance.csch");
            //Loads surface lab rooms
            //SurfaceLab.LoadRoomSchematics(schematicDir + "ArsenalFactory/");


        }

        public static SchematicMetaTile[,] LoadExternalSchematic(string filename)
        {
            SchematicMetaTile[,] toReturn = null;
            using (Stream st = ArsenalLabsBeta.Instance.GetFileStream(filename, true))
                toReturn = (SchematicMetaTile[,])ImportSchematicMethod.Invoke(null, new object[] { st });

            return toReturn;
        }
    }
}
