using ArsenalLabsBeta.NPCs.Arsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ArsenalLabsBeta.Items.Tiles.PuzzleElements
{
    public class RustCreepStatue : ModTile
    {
        // private bool Inactive = false;


        public override void SetStaticDefaults()
        {
            //Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileNoSunLight[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;


            AddMapEntry(new Color(26, 26, 26));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            //TileObjectData.addTile(Type);
            DustType = 33;
            //ItemDrop = ModContent.ItemType<Items.Placeable.PuzzleElements.EmancipationGrid>();
            TileID.Sets.GemsparkFramingTypes[Type] = Type;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];


            foreach (var oNpc in Main.npc)
            {
                if (oNpc.type == ModContent.NPCType<BabyRustCreep>() && oNpc.active)
                    oNpc.active = false;
            }


            Vector2 pos = new Point(i, j).ToWorldCoordinates();
            var npc = NPC.NewNPCDirect(new EntitySource_Wiring(i, j), pos, ModContent.NPCType<BabyRustCreep>());

        }

    }
}
