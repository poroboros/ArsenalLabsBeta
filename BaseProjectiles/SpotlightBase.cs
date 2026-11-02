using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ArsenalLabsBeta.Items.Ammo.Lens;

namespace ArsenalLabsBeta.Projectiles.BaseProjectiles
{
    public abstract class SpotlightBase : ModProjectile
    {
        public override string Texture => "ArsenalLabs/Assets/LensFlare";

        Texture2D line;

        public Color drawColor = LensColors.WulfrumColor;

        public float coneAngle = 8;

        public float length = 1500;

        public float coneRadian => MathHelper.ToRadians(coneAngle);

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteRGB(drawColor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            drawColor = reader.ReadRGB();
        }



        public override void SetDefaults()
        {
            line = ModContent.Request<Texture2D>("ArsenalLabs/Assets/FadeLine").Value;
            SetFlashlightDefaults();
        }

        public virtual void SetFlashlightDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.netImportant = true;

            Projectile.hide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return drawColor;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public static bool InCone(Vector2 point, Projectile projectile, float length, float coneAngle)
        {
            Vector2[] points = TrianglePointsFromCenterLengthAndAngle(projectile.Center, length, coneAngle * .5f, projectile.rotation);


            /*if (Main.rand.NextBool(100))
            {
                //ConeDebug(projectile.ModProjectile as SpotlightBase, points);
                Dust.NewDust(projectile.Center, 5, 5, DustID.PurpleTorch);
                Dust.NewDust(point, 5, 5, DustID.RedTorch);

                foreach (Vector2 pz in points)
                {
                    Dust.NewDust(pz, 5, 5, DustID.GreenTorch);
                }
            }*/



            float mainArea = TriangleAreaFromPoints(projectile.Center, points[0], points[1]);

            float a1 = TriangleAreaFromPoints(point, points[0], points[1]);
            float a2 = TriangleAreaFromPoints(point, points[0], projectile.Center);
            float a3 = TriangleAreaFromPoints(point, points[1], projectile.Center);

            float sum = a1 + a2 + a3;

            float precision = 1;

            //Main.NewText(sum - mainArea);
            return sum - mainArea < precision;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < slices; i++)
            {

                bool result = LineCollisionCheck((i - slices / 2f) / 4, .007f, targetHitbox);

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public bool LineCollisionCheck(float relAngle, float step, Rectangle hitbox)
        {
            Vector2 lineVec = new Vector2(length, 0);
            float radianAngle = MathHelper.ToRadians(relAngle);
            Vector2 rotatedVec = lineVec.RotatedBy(Projectile.rotation).RotatedBy(radianAngle);
            Vector2 corner = rotatedVec + Projectile.Center;
            float lerp = 0;

            while (lerp < 1)
            {

                lerp += step;

                Vector2 p1 = Vector2.Lerp(Projectile.Center, corner, lerp);
                Tile t = Main.tile[p1.ToSafeTileCoordinates()];
                if (lerp % (step * 2) < .01f)
                {
                    //Main.NewText("light at " + lerp + " s " + step);
                    float strength = .3f;
                    Color strengthCol = new Color(strength, strength, strength);
                    Lighting.AddLight(p1, drawColor.MultiplyRGB(strengthCol).ToVector3());
                }

                Rectangle check = new Rectangle((int)p1.X, (int)p1.Y, 5, 5);

                if (hitbox.Intersects(check))
                {
                    return true;
                }

                if (t.HasTile && Main.tileSolid[t.TileType] && !TileID.Sets.Platforms[t.TileType])
                {
                    if (t.HasTile && Main.tileSolid[t.TileType] && !TileID.Sets.Platforms[t.TileType] &&
                        Main.tileBlockLight[t.TileType])
                    {

                        return false;
                    }
                }

            }

            return false;
        }


        int slices = 88;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.gamePaused)
                return false;

            for (int i = 0; i < slices; i++)
            {

                float alp = i / (float)slices * .05f;
                if (i > slices / 2)
                    alp = (slices - i) / (float)slices * .05f;
                Color alphaFade = new Color(alp, alp, alp, alp);
                Vector2? tilePoint = DrawFlashlightStrip((i - slices / 2f) / 4, .007f, drawColor.MultiplyRGBA(alphaFade));
            }


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

        static Vector2[] TrianglePointsFromCenterLengthAndAngle(Vector2 center, float length, float angle, float rotation)
        {
            Vector2[] toReturn = new Vector2[2];

            Vector2 lineVec = new Vector2(length, 0);


            float radianAngle = MathHelper.ToRadians(angle);
            Vector2 rotatedVec = lineVec.RotatedBy(rotation);

            toReturn[0] = rotatedVec.RotatedBy(-radianAngle) + center;
            toReturn[1] = rotatedVec.RotatedBy(radianAngle) + center;

            return toReturn;
        }

        static float TriangleAreaFromPoints(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float a1 = p1.X * (p2.Y - p3.Y);
            float a2 = p2.X * (p3.Y - p1.Y);
            float a3 = p3.X * (p1.Y - p2.Y);
            float a = .5f * MathF.Abs(a1 + a2 + a3);
            return a;
        }


        Vector2? DrawFlashlightStrip(float relAngle, float step, Color lineColor)
        {

            float extraLength = (MathF.Sin(relAngle / coneAngle) + 1) * 5;
            Vector2 lineVec = new Vector2(length + extraLength, 0);
            float radianAngle = MathHelper.ToRadians(relAngle);
            Vector2 rotatedVec = lineVec.RotatedBy(Projectile.rotation).RotatedBy(radianAngle);
            Vector2 corner = rotatedVec + Projectile.Center;
            int count = 0;

            float width = 10;

            float lerp = 0;

            while (lerp < 1)
            {

                lerp += step;

                Vector2 p1 = Vector2.Lerp(Projectile.Center, corner, lerp);
                Tile t = Main.tile[p1.ToSafeTileCoordinates()];
                if (lerp % (step * 2) < .01f)
                {
                    //Main.NewText("light at " + lerp + " s " + step);
                    float strength = .3f;
                    Color strengthCol = new Color(strength, strength, strength);
                    Lighting.AddLight(p1, drawColor.MultiplyRGB(strengthCol).ToVector3());
                }
                if (t.HasTile && Main.tileSolid[t.TileType] && !TileID.Sets.Platforms[t.TileType])
                    if (t.HasTile && Main.tileSolid[t.TileType] && !TileID.Sets.Platforms[t.TileType] && Main.tileBlockLight[t.TileType])
                    {
                        DrawLineBetter(line, Projectile.Center, p1, lineColor, width);

                        return p1;
                    }
                count++;

            }
            DrawLineBetter(line, Projectile.Center, corner, lineColor, width);
            return null;
        }


        void DrawLineBetter(Texture2D value, Vector2 start, Vector2 end, Color color, float width)
        {
            if (!(start == end))
            {
                start -= Main.screenPosition;
                end -= Main.screenPosition;
                float rotation = (end - start).ToRotation();
                Main.spriteBatch.Draw(scale: new Vector2(Vector2.Distance(start, end) / value.Width, width), texture: value, position: start, sourceRectangle: null, color: color, rotation: rotation, origin: value.Size() * Vector2.UnitY * 0.5f, effects: SpriteEffects.None, layerDepth: 0f);
            }
        }

        public static void ConeDebug(SpotlightBase instance, Vector2[] points, bool control = false)
        {
            if (control)
            {
                if (Main.mouseLeft)
                {
                    instance.length += 5;
                }
                if (Main.mouseRight)
                {
                    instance.length -= 5;
                    instance.length = instance.length > 0 ? instance.length : 1;
                }
                if (Main.mouseMiddle)
                {
                    instance.coneAngle += 1;
                    instance.coneAngle = instance.coneAngle > 90 ? 1 : instance.coneAngle;
                }
            }

            foreach (Vector2 corner in points)
            {
                int d1 = Dust.NewDust(corner, 5, 5, DustID.UltraBrightTorch);
                float lerp = 0;

                Main.dust[d1].noGravity = true;
                Main.dust[d1].velocity = Vector2.Zero;

                while (lerp < 1)
                {
                    lerp += .01f;

                    Vector2 p1 = Vector2.Lerp(instance.Projectile.Center, corner, lerp);
                    int d2 = Dust.NewDust(p1, 5, 5, DustID.UltraBrightTorch);
                    Main.dust[d2].noGravity = true;
                    Main.dust[d2].velocity = Vector2.Zero;
                }
            }
            float lerp2 = 0;
            while (lerp2 < 1)
            {
                lerp2 += .05f;

                Vector2 p1 = Vector2.Lerp(points[0], points[1], lerp2);
                /*int d2 = Dust.NewDust(p1, 5, 5, DustID.UltraBrightTorch);
                Main.dust[d2].noGravity = true;
                Main.dust[d2].velocity = Vector2.Zero;*/
            }

            return;
            /*foreach (NPC npc in Main.npc)
            {
                if (InCone(npc.Center))
                {
                    //Main.NewText(npc.FullName + Main.time);
                    int d2 = Dust.NewDust(npc.Center, 5, 5, DustID.FireworkFountain_Red);
                    //Main.dust[d2].noGravity = true;
                    //Main.dust[d2].velocity = Vector2.Zero;
                }
            }*/
        }
        public override bool? CanCutTiles() { return false; }

    }
}
