using System;
using System.Collections.Generic;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.FurnitureMonolith;
using CalamityMod.Tiles.Ores;
using ArsenalLabsBeta.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Tiles
{
    public class ArsenalLabsTile : GlobalTile, IPostSetupContent
    {


        public static Action ResetTileRenderPoints;
        public static Action DrawSpecialTilePoints;

        public override void Load()
        {
            Terraria.GameContent.Drawing.On_TileDrawing.PreDrawTiles += TileDrawing_PreDrawTiles;
        }
        private static void TileDrawing_PreDrawTiles(Terraria.GameContent.Drawing.On_TileDrawing.orig_PreDrawTiles orig, Terraria.GameContent.Drawing.TileDrawing self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets)
        {
            orig(self, solidLayer, forRenderTargets, intoRenderTargets);
            bool flag = intoRenderTargets || Lighting.UpdateEveryFrame;
            if (!solidLayer && flag)
            {
                ResetTileRenderPoints?.Invoke();
            }
        }


        public override void Unload()
        {
            ResetTileRenderPoints = null;
            DrawSpecialTilePoints = null;
        }

        public static void QuickGlowmask(ModTile modTile, SpriteBatch spriteBatch, int i, int j, Color? color = null, float horizontalFlipXOffset = 0f)
        {
            var glowmask = ModContent.Request<Texture2D>(modTile.Texture + "_Glow");

            var effects = SpriteEffects.None;
            TileLoader.SetSpriteEffects(i, j, modTile.Type, ref effects);
            var drawPosition = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + ArsenalLabsHelpers.TileDrawOffset;
            if (effects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                drawPosition.X += horizontalFlipXOffset;
            }

            spriteBatch.Draw(glowmask.Value, drawPosition, new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 18, 18),
                color == null ? Color.White : color.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }
        public static void DrawSlopedTile(int i, int j, int type, Texture2D texture, Color drawColor, Vector2 positionOffset)
        {
            Tile tile = Main.tile[i, j];
            int frameX = tile.TileFrameX;
            int TileFrameY = tile.TileFrameY;
            int width = 16;
            int height = 16;
            Vector2 location = new Vector2(i * 16, j * 16);
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            Vector2 offsets = -Main.screenPosition + zero + positionOffset;
            Vector2 drawCoordinates = location + offsets;
            if (tile.Slope == 0 && !tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, TileFrameY, width, height), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X, drawCoordinates.Y + 8), new Rectangle(frameX, TileFrameY, width, 8), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                byte b = (byte)tile.Slope;
                Rectangle frame;
                Vector2 drawPos;
                if (b == 1 || b == 2)
                {
                    int length;
                    int height2;
                    for (int a = 0; a < 8; ++a)
                    {
                        if (b == 2)
                        {
                            length = 16 - a * 2 - 2;
                            height2 = 14 - a * 2;
                        }
                        else
                        {
                            length = a * 2;
                            height2 = 14 - length;
                        }
                        frame = new Rectangle(frameX + length, TileFrameY, 2, height2);
                        drawPos = new Vector2(i * 16 + length, j * 16 + a * 2) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }
                    frame = new Rectangle(frameX, TileFrameY + 14, 16, 2);
                    drawPos = new Vector2(i * 16, j * 16 + 14) + offsets;
                    Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    int length;
                    int height2;
                    for (int a = 0; a < 8; ++a)
                    {
                        if (b == 3)
                        {
                            length = a * 2;
                            height2 = 16 - length;
                        }
                        else
                        {
                            length = 16 - a * 2 - 2;
                            height2 = 16 - a * 2;
                        }
                        frame = new Rectangle(frameX + length, TileFrameY + 16 - height2, 2, height2);
                        drawPos = new Vector2(i * 16 + length, j * 16) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }
                    drawPos = new Vector2(i * 16, j * 16) + offsets;
                    frame = new Rectangle(frameX, TileFrameY, 16, 2);
                    Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
        }

        internal static Vector2 GetWorldPositionOnTopOfTile(Tile tile, float offsetFromTheFarLeftSideOfTheTile, Vector2 tileWorldCoordinate)
        {
            switch (tile.Slope)
            {
                case SlopeType.SlopeDownLeft:
                    {
                        tileWorldCoordinate.X += offsetFromTheFarLeftSideOfTheTile;
                        tileWorldCoordinate.Y += offsetFromTheFarLeftSideOfTheTile;
                    }
                    break;

                case SlopeType.SlopeDownRight:
                    {
                        tileWorldCoordinate.X += offsetFromTheFarLeftSideOfTheTile;
                        tileWorldCoordinate.Y += 16 - offsetFromTheFarLeftSideOfTheTile;
                    }
                    break;

                default: // this appears ON TOP of tiles so the slopes with the flat tops are considered a regular tile
                    {
                        tileWorldCoordinate.X += offsetFromTheFarLeftSideOfTheTile;
                        if (tile.IsHalfBlock)
                        {
                            tileWorldCoordinate.Y += 8f;
                        }
                    }
                    break;
            }
            return tileWorldCoordinate;
        }

        public void PostSetupContent(ArsenalLabsBeta arslab)
        {
            throw new NotImplementedException();
        }
    }
}
