using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Tiles
{
    public class BrassLabPanel : ModTile
    {
        public static readonly SoundStyle MinePlatingSound = new("CalamityMod/Sounds/Custom/PlatingMine", 3);
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            AddMapEntry(new Color(187, 162, 127));
            TileID.Sets.GemsparkFramingTypes[Type] = Type;
            HitSound = MinePlatingSound;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            Point frame = new Point(tile.TileFrameX, tile.TileFrameY);

            //Samples
            Color top = Lighting.GetColor(i, j - 1);
            Color bottom = Lighting.GetColor(i, j + 1);
            Color left = Lighting.GetColor(i - 1, j);
            Color right = Lighting.GetColor(i + 1, j);
            Color topleft = Lighting.GetColor(i - 1, j - 1);
            Color topright = Lighting.GetColor(i + 1, j - 1);
            Color bottomleft = Lighting.GetColor(i - 1, j + 1);
            Color bottomright = Lighting.GetColor(i + 1, j + 1);

            //Environmental Light
            float envR = (top.R + bottom.R + left.R + right.R + topleft.R + topright.R + bottomleft.R + bottomright.R) / (8f * 255f);
            float envG = (top.G + bottom.G + left.G + right.G + topleft.G + topright.G + bottomleft.G + bottomright.G) / (8f * 255f);
            float envB = (top.B + bottom.B + left.B + right.B + topleft.B + topright.B + bottomleft.B + bottomright.B) / (8f * 255f);

            if ((envR + envG + envB) > 0.05f && GlowTextureSystem.TryGetColor("ArsenalLabsBeta/Tiles/BrassLabPanel", frame, out Color tileAvgColor))
            {
                //tile color
                float tileR = tileAvgColor.R / 155f;
                float tileG = tileAvgColor.G / 155f;
                float tileB = tileAvgColor.B / 155f;

                // Multiplicative Blending (had to read too much brain hurty)
                // tile's texture limits which wavelengths can reflect
                float reflectvity = 0.8f;

                r = tileR * envR * reflectvity;
                g = tileG * envG * reflectvity;
                b = tileB * envB * reflectvity;
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
            return false;
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 57 + Main.rand.Next(2), 0f, 0f, 1, new Color(255, 255, 255));
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
