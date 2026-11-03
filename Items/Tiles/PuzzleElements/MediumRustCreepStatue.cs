using ArsenalLabsBeta.NPCs.Arsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArsenalLabsBeta.Tiles.PuzzleElements
{
    public class MediumRustCreepStatue : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item 
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(26, 26, 26), name);
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];


            foreach (var oNpc in Main.npc)
            {
                if (oNpc.type == ModContent.NPCType<PacifiedRustCreep>() && oNpc.active)
                    oNpc.active = false;
            }


            Vector2 pos = new Point(i, j).ToWorldCoordinates();
            var npc = NPC.NewNPCDirect(new EntitySource_Wiring(i, j), pos, ModContent.NPCType<PacifiedRustCreep>());

        }
    }
}
