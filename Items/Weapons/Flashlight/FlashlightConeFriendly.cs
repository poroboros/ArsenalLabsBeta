using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod;
using ArsenalLabsBeta.Items.Ammo.Lens;
using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Flashlight
{
    public class FlashlightConeFriendly : FlashlightHoldOutProjectile
    {

        public override string Texture => "ArsenalLabsBeta/Assets/LensFlare";
        Texture2D line;

        /// <summary>
        /// Default color of this flashlight
        /// </summary>
        public Color drawColor = LensColors.WulfrumColor;


        /// <summary>
        /// Unused currently, TODO
        /// </summary>
        public float coneAngle = 8;


        /// <summary>
        /// Numbers of different flashlight beams drawn
        /// </summary>
        public int slices = 44;

        public float length = 1500;

        public int colorIndex = 0;

        List<int> colorList = new List<int>();

        bool firstTime = true;

        public int itemOwnerType = ModContent.ItemType<WulfrumFlashlight>();
        public float holdoutOffset = 35;

        public float coneRadian => MathHelper.ToRadians(coneAngle);

        WulfrumSpelunkerHelper spelkHelper;

        bool hasSpelunk = false;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteRGB(drawColor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            drawColor = reader.ReadRGB();
        }

        public int caramellcounter = 0;

        public override void SetDefaults()
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
            line = ModContent.Request<Texture2D>("ArsenalLabsBeta/Assets/FadeLine").Value;
            //this.DrawOriginOffsetX = 2354/2;
            //this.DrawOriginOffsetY = 770 / 2;
        }

        public override void AI()
        {

            Player owner = Main.player[Projectile.owner];


            if (firstTime)
            {


                //Loads each possible color for this flashlight dynamically from the lens color lists
                colorList.Add(Type);
                foreach (int color in LensColors.LensTypes)
                {

                    //Only if you have the lens in your inventory, that is
                    if (owner.HasItem(color))
                    {
                        colorList.Add(color);
                    }
                }

                hasSpelunk = owner.ArsenalLabs().spelunkChip;

                firstTime = false;
            }


            //Remove if not holding


            if ((owner.HeldItem.type != itemOwnerType) || owner.dead)
            {
                Projectile.Kill();
            }

            Projectile.spriteDirection = Projectile.rotation < MathF.PI ? -1 : 1;
            Projectile.timeLeft = 10;


            //Holdout projectile stuff
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (owner.whoAmI == Main.myPlayer)
            {
                UpdateHeldProjDoVelocity(owner.RotatedRelativePoint(owner.MountedCenter), offsetAmount: 30);
            }

            UpdateHeldProj(owner.RotatedRelativePoint(owner.MountedCenter), offsetAmount: holdoutOffset, updateArm: false);
            Projectile.spriteDirection = 1;


            /*SpelunkerProjectileHelper*/

            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle + (Projectile.spriteDirection == -1).ToInt() * (float)Math.PI;


            //Agent of the Demiurge
            if (ArsenalLabsBeta.AprilFools)
            {
                caramellcounter++;
                if (caramellcounter == 12)
                {
                    CycleColor();
                    caramellcounter = 0;
                }
            }

            if (colorList[colorIndex] == ModContent.ItemType<RainbowFlashlightLens>()) drawColor = Main.DiscoColor;

            Lighting.AddLight(Projectile.Center, drawColor.ToVector3());
        }

        public override Color? GetAlpha(Color lightColor)
        {

            return drawColor;

        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }


        /// <summary>
        /// Unused, will be fixed for submarine most likely
        /// </summary>
        /// <param name="point"></param>
        /// <param name="projectile"></param>
        /// <param name="length"></param>
        /// <param name="coneAngle"></param>
        /// <returns></returns>
        public static bool InCone(Vector2 point, Projectile projectile, float length, float coneAngle)
        {
            Vector2[] points = TrianglePointsFromCenterLengthAndAngle(projectile.Center, length, coneAngle, projectile.rotation);

            float mainArea = TriangleAreaFromPoints(projectile.Center, points[0], points[1]);

            float a1 = TriangleAreaFromPoints(point, points[0], points[1]);
            float a2 = TriangleAreaFromPoints(point, points[0], projectile.Center);
            float a3 = TriangleAreaFromPoints(point, points[1], projectile.Center);

            float sum = a1 + a2 + a3;

            float precision = 1;

            return sum - mainArea < precision;
        }


        public override bool PreDraw(ref Color lightColor)
        {

            //If using spelunking chip, do all that stuff
            if (spelkHelper == null && hasSpelunk)
                spelkHelper = new WulfrumSpelunkerHelper(5, DustID.MagicMirror);

            if (spelkHelper != null)
                spelkHelper.OnPreUpdateAllProjectiles();

            //Prevents weirdness
            if (Main.gamePaused)
                return false;


            //Draw each strip, with properly fading alpha towards the end
            for (int i = 0; i < slices; i++)
            {

                float alp = i / (float)slices * .05f;
                if (i > slices / 2)
                    alp = (slices - i) / (float)slices * .05f;
                Color alphaFade = new Color(alp, alp, alp, alp);
                //spelkHelper.OnPreUpdateAllProjectiles();
                Vector2? tilePoint = DrawFlashlightStrip((i - slices / 2f) / 4, .007f, drawColor.MultiplyRGBA(alphaFade));
            }


            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = SpriteEffects.None;
            /*if (Projectile.spriteDirection == -1)
            {
                drawOrigin.X = texture.Width - drawOrigin.X;
                effects = SpriteEffects.FlipHorizontally;
            }*/
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }


        //Does math to get the two "base points" of an isoceles triangle, given an angle and length
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

        //Area of the triangle
        static float TriangleAreaFromPoints(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float a1 = p1.X * (p2.Y - p3.Y);
            float a2 = p2.X * (p3.Y - p1.Y);
            float a3 = p3.X * (p1.Y - p2.Y);

            float a = .5f * MathF.Abs(a1 + a2 + a3);

            return a;
        }


        /// <summary>
        /// Draws an individual light ray of the flashlighy
        /// </summary>
        /// <param name="relAngle">Angle this line is cast at</param>
        /// <param name="step">How much does the lerp advance?</param>
        /// <param name="lineColor"></param>
        /// <returns></returns>
        Vector2? DrawFlashlightStrip(float relAngle, float step, Color lineColor)
        {

            Vector2 lineVec = new Vector2(length, 0);
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


                    //Spelunker glowstick call, limited number of calls due to lag and effective range being kinda high
                    if (lerp % (step * 4) < .01f && lerp < .6f && relAngle % coneAngle * 2 == 0 && spelkHelper != null)
                    {
                        spelkHelper.AddSpotToCheck(p1);
                    }
                }

                //Have the lines be stopped by tiles
                if (t.HasTile && Main.tileSolid[t.TileType] && !TileID.Sets.Platforms[t.TileType] && Main.tileBlockLight[t.TileType] && !t.IsActuated)
                {
                    DrawLineBetter(line, Projectile.Center, p1, lineColor, width);

                    return p1;
                }
                count++;

                //spelkHelper.AddSpotToCheck(p1);


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
        public void CycleColor()
        {
            Player owner = Main.player[Projectile.owner];

            colorIndex = colorIndex < colorList.Count - 1 ? colorIndex + 1 : 0;

            if (colorList.Count > 0)
                drawColor = LensColors.GetColorByItemType(colorList[colorIndex], owner);



            //Main.NewText(drawColor + " " + colorIndex);

            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].ArsenalLabs().hasFlashlight = false;
        }

        void ConeDebug(Vector2[] points)
        {
            if (Main.mouseLeft)
            {
                length += 5;
            }
            if (Main.mouseRight)
            {
                length -= 5;
                length = length > 0 ? length : 1;
            }
            if (Main.mouseMiddle)
            {
                coneAngle += 1;
                coneAngle = coneAngle > 90 ? 1 : coneAngle;
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

                    Vector2 p1 = Vector2.Lerp(Projectile.Center, corner, lerp);
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


    /// <summary>
    /// Modified Spelunker Helper class made for the flashlight chip
    /// </summary>
    internal class WulfrumSpelunkerHelper
    {

        private HashSet<Vector2> _positionsChecked = new HashSet<Vector2>();

        private HashSet<Point> _tilesChecked = new HashSet<Point>();

        private Rectangle _clampBox;

        private int _frameCounter;

        public int range = 30;

        public int dustType = 204;

        internal WulfrumSpelunkerHelper(int range = 30, int dustType = 204)
        {
            this.range = range;
            this.dustType = dustType;
        }

        public void OnPreUpdateAllProjectiles()
        {
            _clampBox = new Rectangle(2, 2, Main.maxTilesX - 2, Main.maxTilesY - 2);
            if (++_frameCounter >= 10)
            {
                _frameCounter = 0;
                _tilesChecked.Clear();
                _positionsChecked.Clear();
            }
        }

        public void AddSpotToCheck(Vector2 spot)
        {
            if (_positionsChecked.Add(spot))
            {
                CheckSpot(spot);
            }
        }

        private void CheckSpot(Vector2 Center)
        {
            int num = (int)Center.X / 16;
            int num2 = (int)Center.Y / 16;

            int num3 = Utils.Clamp(num - range, _clampBox.Left, _clampBox.Right);
            int num4 = Utils.Clamp(num + range, _clampBox.Left, _clampBox.Right);
            int num5 = Utils.Clamp(num2 - range, _clampBox.Top, _clampBox.Bottom);
            int num6 = Utils.Clamp(num2 + range, _clampBox.Top, _clampBox.Bottom);
            Point item = default(Point);
            Vector2 position = default(Vector2);
            for (int i = num3; i <= num4; i++)
            {
                for (int j = num5; j <= num6; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile != null && Main.IsTileSpelunkable(i, j) && !(new Vector2(num - i, num2 - j).Length() > 30f))
                    {
                        item.X = i;
                        item.Y = j;
                        if (_tilesChecked.Add(item) && Main.rand.NextBool(4))
                        {
                            position.X = i * 16;
                            position.Y = j * 16;
                            Dust dust = Dust.NewDustDirect(position, 16, 16, dustType, 0f, 0f, 150, default(Color), 0.3f);
                            dust.fadeIn = 0.75f;
                            dust.velocity *= 0.1f;
                            dust.noLight = true;
                        }
                    }
                }
            }
        }
    }
}

