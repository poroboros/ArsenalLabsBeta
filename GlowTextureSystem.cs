using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;

namespace ArsenalLabsBeta
{
    //Got a lot of help making this from my partner was not easy.
    public class GlowTextureSystem : ModSystem
    {
        // Store averages for selected textures to avoid recalculating every frame
        private static Dictionary<string, Dictionary<Point, Color>> _textureAverages = new();

        public override void Unload() {
            _textureAverages?.Clear();
            _textureAverages = null;
        }

        /// Registers a texture to be pre-calculated for average frame colors. Registered Textures are done in Post Setup Content in ArsenalLabsBeta.cs
        public static void RegisterTexture(string texturePath) {
            if (_textureAverages.ContainsKey(texturePath)) return;

            Main.QueueMainThreadAction(() => {
                var asset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad);
                if (asset.Value == null) return;

                Texture2D texture = asset.Value;
                Color[] pixels = new Color[texture.Width * texture.Height];
                texture.GetData(pixels);

                var frameAverages = new Dictionary<Point, Color>();

                for (int y = 0; y < texture.Height; y += 18) {
                    for (int x = 0; x < texture.Width; x += 18) {
                        frameAverages[new Point(x, y)] = CalculateAverage(pixels, x, y, texture.Width);
                    }
                }

                _textureAverages[texturePath] = frameAverages;
            });
        }

        private static Color CalculateAverage(Color[] pixels, int startX, int startY, int texWidth) {
            long r = 0, g = 0, b = 0;
            int count = 0;

            for (int y = startY; y < startY + 16; y++) {
                for (int x = startX; x < startX + 16; x++) {
                    int index = y * texWidth + x;
                    if (index < pixels.Length) {
                        Color c = pixels[index];
                        if (c.A > 0) {
                            r += c.R; g += c.G; b += c.B;
                            count++;
                        }
                    }
                }
            }
            return count > 0 ? new Color((byte)(r / count), (byte)(g / count), (byte)(b / count)) : Color.White;
        }

        public static bool TryGetColor(string texturePath, Point frame, out Color color) {
            color = Color.White;
            return _textureAverages != null && 
                   _textureAverages.TryGetValue(texturePath, out var frames) && 
                   frames.TryGetValue(frame, out color);
        }
    }
}