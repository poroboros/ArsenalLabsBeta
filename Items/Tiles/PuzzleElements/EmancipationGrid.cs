using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Tiles.PuzzleElements
{
    public class EmancipationGrid : ModTile
    {
        private bool Inactive = false;
        public static readonly SoundStyle MineSound = new("ArsenalLabsBeta/Assets/Sounds/Tiles/EmancipationGrid", 3) { Volume = 0.5f };
        public static readonly SoundStyle PortalDie = new("ArsenalLabsBeta/Assets/Sounds/Tiles/PortalDie", 1) { Volume = 0.5f };
        public override void SetStaticDefaults()
        {
            //Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileNoSunLight[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            AddMapEntry(new Color(26, 210, 214));

            //TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            //TileObjectData.addTile(Type);
            DustType = 33;
            HitSound = MineSound;
            //ItemDrop = ModContent.ItemType<Items.Placeable.PuzzleElements.EmancipationGrid>();
            TileID.Sets.GemsparkFramingTypes[Type] = Type;
        }
        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false; //(needed here for some reason)
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Inactive)
            {
                r = g = b = 0f;
            }
            else
            {
                float brightness = 0.1f;
                float waveScale1 = Main.GameUpdateCount * 0.028f;
                float waveScale2 = Main.GameUpdateCount * 0.1f;
                int yScale = -j / 2;
                int xScale = i / 2;
                float wave1 = Main.GameUpdateCount * 0.24f * -50 + ((-xScale) + yScale) * 70;
                float wave2 = waveScale2 * -10 + ((-xScale) + yScale) * 45;
                float wave3 = waveScale1 * -15 + ((-xScale) + yScale) * 25;
                float wave4 = Main.GameUpdateCount * 0.55f * 10 + ((-xScale) + yScale) * 90;
                float wave5 = waveScale1 * -70 + ((-xScale) + yScale) * 20;
                float wave6 = waveScale2 * -10 + (xScale + yScale) * 45;
                float bigwave = Main.GameUpdateCount * 0.1f * -70 + ((-xScale) + yScale) * 40;
                float wave1angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave1));
                float wave2angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave2));
                float wave3angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave3));
                float wave4angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave4));
                float wave5angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave5));
                float wave6angle = 0.55f + 0.45f * (float)Math.Sin(MathHelper.ToRadians(wave6));
                float bigwaveangle = 0.80f + 0.80f * (float)Math.Sin(MathHelper.ToRadians(bigwave));
                r = 0f;
                g = 193f / 300f;
                b = 214f / 300f;
                r *= brightness + (wave1angle + wave2angle + wave3angle + wave4angle + wave5angle + wave6angle + bigwaveangle) / 7.3f;
                g *= brightness + (wave1angle + wave2angle + wave3angle + wave4angle + wave5angle + wave6angle + bigwaveangle) / 7.3f;
                b *= brightness + (wave1angle + wave2angle + wave3angle + wave4angle + wave5angle + wave6angle + bigwaveangle) / 7.3f;
            }
        }
        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            ///int topY = j - tile.TileFrameY / 18 % 3;
            //short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);

            //Main.tile[i, topY].TileFrameX += frameAdjustment;
            //Main.tile[i, topY + 1].TileFrameX += frameAdjustment;
            //Main.tile[i, topY + 2].TileFrameX += frameAdjustment;



            //Wiring.SkipWire(i, topY + 1);
            //Wiring.SkipWire(i, topY + 2);

            // Avoid trying to send packets in singleplayer.
            if (Inactive)
                Inactive = false;
            else
                Inactive = true;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }
        }


        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool PortalDying = false;
            if (!Inactive)
            {
                //if (Main.LocalPlayer.position / 8 == new Vector2(i, j) * 2)
                if (Main.LocalPlayer.position.ToTileCoordinates() == new Point(i, j))
                {
                    foreach (Projectile Proj in Main.projectile)
                    {
                        if (Proj.type == ProjectileID.PortalGunGate)
                        {
                            Proj.Kill();
                            if (PortalDying == false)
                            {
                                SoundEngine.PlaySound(PortalDie);
                                PortalDying = true;
                            }
                        }
                    }
                }
                foreach (Projectile Proj in Main.projectile)
                {
                }
            }
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Framing.SelfFrame8Way(i, j, Main.tile[i, j], resetFrame);
            return false;
        }
    }
}
