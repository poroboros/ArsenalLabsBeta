using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Audio;



namespace ArsenalLabsBeta
{
    public static class ArsenalLabsHelpers
    {    

        public static Color FurnitureColor => new Color(191, 142, 111);

        public static Vector2 TileDrawOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        public static string GetPath(this object o)
        {
            return GetPath(o.GetType());
        }

        public static string GetPath<T>()
        {
            return GetPath(typeof(T));
        }

        public static int Abs(this int value)
        {
            return value >= 0 ? value : value * -1;
        }

        public static float Abs(this float value)
        {
            return value >= 0 ? value : value * -1f;
        }


        public static Item ActiveItem(this Player player)
        {
            return player.HeldItem;
        }
        
        public static ArsenalLabsPlayer ArsenalLabs(this Player Player)
        {
            return Player.GetModPlayer<ArsenalLabsPlayer>();
        }

        public static void Slope(this Tile tile, byte value)
        {
            tile.Slope = (SlopeType)value;
        }
        public static void HalfBrick(this Tile tile, bool value)
        {
            tile.IsHalfBlock = value;
        }
        public static void Active(this Tile tile, bool value)
        {
            tile.HasTile = value;
        }

        public static SoundStyle WithVolume(this SoundStyle soundStyle, float volume)
        {
            var value = soundStyle;
            value.Volume = volume;
            return value;
        }
        public static SoundStyle WithPitch(this SoundStyle soundStyle, float pitch)
        {
            var value = soundStyle;
            value.PitchRange = (pitch, pitch);
            return value;
        }

        /// <summary>
        /// Draws a fishing line between a line of points
        /// </summary>
        /// <param name="list"></param>
        /// <param name="lineCol"></param>
        public static void DrawFishingLineBetweenPoints(List<Vector2> list, Color lineCol, bool useLighCol = true, float scaleMod = 1f)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];


            for (int i = 0; i < list.Count - 2; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = lineCol;
                if (useLighCol)
                    color = Lighting.GetColor(element.ToTileCoordinates(), lineCol);
                Vector2 scale = new Vector2(1 * scaleMod, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);


                pos += diff;
            }
        }

        /// <summary>
        /// Determines if this item has enough charge to be used.
        /// <br/> The <paramref name="pay"/> parameter can be used to consume the charge amount.
        /// </summary>
        /// <param name="item">The item to use.</param>
        /// <param name="amount">The amount of mana needed. If <c> &lt; 0</c>, use <see cref="CalamityGlobalItem.ChargePerUse"/>.</param>
        /// <param name="pay">If <see langword="true"/>, actually use the charge requested.</param>
        /// <returns><see langword="true"/> if the item has enough charge, <see langword="false"/> otherwise.</returns>
        public static bool CheckCharge(this Item item, float amount = -1, bool pay = true)
        {
            CalamityGlobalItem calItem = item.Calamity();
            if (!calItem.UsesCharge)
                return true;
            amount = amount < 0 ? calItem.ChargePerUse : amount;
            if (calItem.Charge < amount)
                return false;
            if (pay)
            {
                calItem.Charge -= amount;
            }
            return true;
        }
    }
}