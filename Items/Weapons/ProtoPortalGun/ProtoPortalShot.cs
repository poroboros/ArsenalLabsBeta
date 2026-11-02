using System.Collections.Generic;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.ProtoPortalGun
{


    public class ProtoPortalShot : ModProjectile
    {
        public static List<int> validTiles = new List<int>
        {
            //ModContent.TileType<LaboratoryPlating>(),
            //ModContent.TileType<LaboratoryPipePlating>(),
            ModContent.TileType<LaboratoryPanels>(),
        };

        private static readonly Vector2[] EDGES = new Vector2[4]
            {
                new Vector2(0f, 1f),
                new Vector2(0f, -1f),
                new Vector2(1f, 0f),
                new Vector2(-1f, 0f)
            };

        private static readonly Vector2[] SLOPE_EDGES = new Vector2[4]
            {
                new Vector2(1f, -1f),
                new Vector2(-1f, -1f),
                new Vector2(1f, 1f),
                new Vector2(-1f, 1f)
            };

        private static readonly Point[] SLOPE_OFFSETS = new Point[4]
            {
                new Point(1, -1),
                new Point(-1, -1),
                new Point(1, 1),
                new Point(-1, 1)
            };
        public override string Texture => "ArsenalLabsBeta/Projectiles/InvisibleProj";

        Color col => PortalHelper.GetPortalColor(Projectile.owner, (int)Projectile.ai[0]);

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PortalGunBolt);

            AIType = ProjectileID.PortalGunBolt;
        }

        public override bool PreAI()
        {
            if (Projectile.wet)
            {
                Projectile.Kill();
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[ProjectileID.PortalGunBolt].Value;
            int frameNum = Main.projFrames[ProjectileID.PortalGunBolt];
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / (2 * frameNum));
            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                drawOrigin.X = texture.Width - drawOrigin.X;
                effects = SpriteEffects.FlipHorizontally;
            }
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            int height = texture.Height / frameNum;
            int startingH = Projectile.frame % frameNum;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);

            Projectile.alpha = Projectile.alpha > 255 ? 255 : Projectile.alpha + 15;
            Main.spriteBatch.Draw(texture, drawPos, frame, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return col;
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            Vector2? pos = ValidPortalPosition(Projectile);

            if (pos == null)
            {
                Projectile.Kill();
                return false;
            }

            Projectile.position = (Vector2)pos;

            if (Main.netMode != NetmodeID.Server && Projectile.alpha > 0)
            {
                if ((double)Projectile.ai[1] != 0.0)
                    SoundEngine.PlaySound(in SoundID.Item114, new Vector2?(Projectile.position));
                else
                    SoundEngine.PlaySound(in SoundID.Item115, new Vector2?(Projectile.position));
            }
            if (Projectile.owner == Main.myPlayer)
            {
                PortalHelper.TryPlacingPortal(Projectile, oldVelocity, Projectile.velocity);

                PortalHelper.UpdatePortalPoints();
            }
            Projectile.position = Vector2.Add(Projectile.position, Projectile.velocity);
            Projectile.Kill();
            return false;
        }

        public static Vector2? ValidPortalPosition(Projectile Projectile, bool ignoreTiles = false)
        {
            Vector2? ret = null;


            Vector2 vector = Projectile.velocity / Projectile.velocity.Length();
            Point position = FindCollision(Projectile.position, Projectile.position + Projectile.velocity + vector * 32f).ToTileCoordinates();
            Vector2 vector2 = new Vector2(position.X * 16 + 8, position.Y * 16 + 8);

            Tile tile = Main.tile[position.X, position.Y];
            bool flag = tile.IsHalfBlock;

            bool valid = false;

            int num = (int)tile.Slope;

            for (int i = 0; i < (flag ? 2 : EDGES.Length); i++)
            {
                //Main.NewText((EDGES[i]) + " "  + (Vector2.Dot(EDGES[i], vector) > 0f && FindValidLine(position, (int)EDGES[i].Y, (int)(0f - EDGES[i].X), out var temo)));
                if (Vector2.Dot(EDGES[i], vector) > 0f && FindValidLine(position, (int)EDGES[i].Y, (int)(0f - EDGES[i].X), out var bestPosition))
                {
                    valid = true;
                    ret = bestPosition.ToVector2() * 16;
                    break;
                }
            }

            if (num != 0 && !valid)
            {
                Vector2 value = SLOPE_EDGES[num - 1];
                if (Vector2.Dot(value, -vector) > 0f && FindValidLine(position, -SLOPE_OFFSETS[num - 1].Y, SLOPE_OFFSETS[num - 1].X, out var bestPosition2))
                {
                    valid = true;
                    ret = new Vector2(bestPosition2.X * 16 + 8, bestPosition2.Y * 16 + 8);
                }
            }

            return ret;
        }



        private static Vector2 FindCollision(Vector2 startPosition, Vector2 stopPosition)
        {
            int lastX = 0;
            int lastY = 0;
            Utils.PlotLine(startPosition.ToTileCoordinates(), stopPosition.ToTileCoordinates(), delegate (int x, int y)
            {
                lastX = x;
                lastY = y;
                return !WorldGen.SolidOrSlopedTile(x, y);
            }, jump: false);
            return new Vector2((float)lastX * 16f, (float)lastY * 16f);
        }

        private static bool FindValidLine(Point position, int xOffset, int yOffset, out Point bestPosition, bool ignoreTiles = false)
        {
            bestPosition = position;
            if (IsValidLine(position, xOffset, yOffset, ignoreTiles))
            {
                return true;
            }

            Point point = new Point(position.X - xOffset, position.Y - yOffset);
            if (IsValidLine(point, xOffset, yOffset, ignoreTiles))
            {
                bestPosition = point;
                return true;
            }

            Point point2 = new Point(position.X + xOffset, position.Y + yOffset);
            if (IsValidLine(point2, xOffset, yOffset, ignoreTiles))
            {
                bestPosition = point2;
                return true;
            }

            return false;
        }

        private static bool IsValidLine(Point position, int xOffset, int yOffset, bool ignoreTiles = false)
        {
            if (position.X + xOffset > Main.tile.Width || position.X - xOffset < 0 || position.Y + yOffset > Main.tile.Height || position.Y - yOffset < 0)
                return false;
            Tile tile = Main.tile[position.X, position.Y];
            Tile t = Main.tile[position.X - xOffset, position.Y - yOffset];
            Tile t2 = Main.tile[position.X + xOffset, position.Y + yOffset];

            //Main.NewText(tile.TileType + " " + t.TileType + " " + t2.TileType);


            if (ignoreTiles || (CanPlacePortalOn(tile) && CanPlacePortalOn(t) && CanPlacePortalOn(t2)))
            {
                return true;
            }

            return false;
        }

        private static bool CanPlacePortalOn(Tile t)
        {
            if (!validTiles.Contains(t.TileType))
            {
                return false;
            }

            return WorldGen.SolidOrSlopedTile(t);
        }
    }
}
