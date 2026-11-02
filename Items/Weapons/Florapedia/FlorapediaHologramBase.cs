using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.Magic.Florapedia
{
    public interface IFlorapediaHologram
    {
        public void Disappear();

        public static void UpdateVfx(Projectile proj, Color lightColor, int appearingTime, int attackTime, int minTimeleftToDisappear, ref float appearingProgress)
        {
            Lighting.AddLight(proj.Center, lightColor.ToVector3() / 2f);
            if (proj.timeLeft >= attackTime)
                // appearing
                appearingProgress = 1 - (proj.timeLeft - attackTime) / (float)appearingTime;
            else if (proj.timeLeft > minTimeleftToDisappear)
                appearingProgress = 1;
            else if (proj.timeLeft <= minTimeleftToDisappear && appearingProgress < 1)
            {
                appearingProgress -= 1.01f / minTimeleftToDisappear;
                if (appearingProgress < 0)
                    appearingProgress = 0;
                if (appearingProgress == 0)
                    proj.Kill();
            }
        }

        public static int FlorapediaFindTarget(Projectile proj, float distance)
        {
            int target = -1;
            bool foundBoss = false;
            float mindis = distance;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(proj))
                {
                    float dis = (Main.npc[i].Center - proj.Center).Length();
                    if (foundBoss)
                    {
                        if (!Main.npc[i].boss)
                            continue;
                        if (dis < mindis)
                        {
                            mindis = dis;
                            target = i;
                        }
                    }
                    else if (dis < mindis)
                    {
                        target = i;
                        mindis = dis;
                        if (Main.npc[i].boss)
                            foundBoss = true;
                    }
                }
            }
            return target;
        }

        public static void UpdateAppearingMovement(Projectile proj, Player owner, ref Vector2 relativePos, float decreasingValue = 0.9f)
        {
            proj.velocity *= decreasingValue;
            relativePos += proj.velocity;
            proj.Center = owner.RotatedRelativePoint(owner.MountedCenter) + relativePos * new Vector2(1, owner.gravDir);
            proj.alpha -= 20;
            if (proj.alpha <= 100)
                proj.alpha = 100;
        }

        public static void DrawHologram(Projectile proj, Texture2D texture, bool drawTriangleLines, Vector2 projectionCenter, float appearingProgress, Color projColor, float borderWidth = 0.05f)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = FlorapediaFlower.FlowerEffect.Value;
            float progress = appearingProgress;
            Color color = projColor * proj.Opacity;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            Matrix view = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            float gravDir = Main.player[proj.owner].gravDir;
            int dir = proj.direction;

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            // wireframe
            if (drawTriangleLines)
            {
                Vector2 weaponScreenPos = projectionCenter;
                FlorapediaVertexInfo[] triangle =
                [
                    new(weaponScreenPos, FlorapediaHoldout.ScreenColor * proj.Opacity, new Vector3(0, 0, 0)),
                    new(proj.Left + new Vector2(0, gravDir * proj.height / 2), color, new Vector3(0, 1, 1)),
                    new(proj.Right + new Vector2(0, gravDir * proj.height / 2), color, new Vector3(1, 0, 1)),
                ];
                int[] triangleIndex = [0, 1, 2];

                effect.Parameters["uTransform"].SetValue(view * Main.Transform * projection);
                effect.Parameters["uWidth"].SetValue(borderWidth);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, triangle, 0, triangle.Length, triangleIndex, 0, triangleIndex.Length / 3);
            }

            // proj
            FlorapediaVertexInfo[] projRect =
                [
                    new(proj.Center + new Vector2(-dir * proj.width / 2, -gravDir * proj.height / 2), color, new Vector3(0, 0, 0)),
                    new(proj.Center + new Vector2(dir * proj.width / 2, -gravDir * proj.height / 2), color, new Vector3(1, 0, 0)),
                    new(proj.Center + new Vector2(-dir * proj.width / 2, gravDir * proj.height / 2), color, new Vector3(0, 1, 1)),
                    new(proj.Center + new Vector2(dir * proj.width / 2, gravDir * proj.height / 2), color, new Vector3(1, 1, 1)),
                ];
            int[] rectIndex = [0, 2, 1, 1, 2, 3];
            view = Matrix.CreateTranslation(new Vector3(-proj.Center, 0)) * Matrix.CreateRotationZ(proj.rotation) * Matrix.CreateTranslation(new Vector3(proj.Center - Main.screenPosition, 0));
            effect.Parameters["uTransform"].SetValue(view * Main.Transform * projection);
            effect.Parameters["uProgress"].SetValue(progress);
            effect.Parameters["uWidth"].SetValue(borderWidth);
            effect.CurrentTechnique.Passes[1].Apply();
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, projRect, 0, projRect.Length, rectIndex, 0, rectIndex.Length / 3);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        public static void DrawHologram(Projectile proj, bool drawTriangleLines, float animationProgress, Color projColor)
        {
            Projectile screen = Main.projectile[(int)proj.ai[0]];
            if (screen.ModProjectile is FlorapediaHoldout)
            {
                Vector2 projectionCenter = screen.Center + new Vector2(Main.player[proj.owner].direction * 25, 3 * Main.player[proj.owner].gravDir);
                DrawHologram(proj, TextureAssets.Projectile[proj.type].Value, drawTriangleLines, projectionCenter, animationProgress, projColor);
            }
            else
                DrawHologram(proj, TextureAssets.Projectile[proj.type].Value, false, Vector2.Zero, animationProgress, projColor);
        }

        public static void DrawHologram(Projectile proj, Texture2D texture, FlorapediaVertexInfo[] vertices, int[] indices, float appearingProgress, Color projColor, float borderWidth = 0.05f)
        {
            SpriteBatch sb = Main.spriteBatch;
            Effect effect = FlorapediaFlower.FlowerEffect.Value;
            float progress = appearingProgress;
            Color color = projColor * proj.Opacity;
            float gravDir = Main.player[proj.owner].gravDir;
            int dir = proj.direction;

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            // proj
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            Matrix view = Matrix.CreateTranslation(new Vector3(-proj.Center, 0)) * Matrix.CreateRotationZ(proj.rotation) * Matrix.CreateTranslation(new Vector3(proj.Center - Main.screenPosition, 0));
            effect.Parameters["uTransform"].SetValue(view * Main.Transform * projection);
            effect.Parameters["uProgress"].SetValue(progress);
            effect.Parameters["uWidth"].SetValue(borderWidth);
            effect.CurrentTechnique.Passes[1].Apply();
            Main.graphics.GraphicsDevice.Textures[0] = texture;
            Main.graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }

    public struct FlorapediaVertexInfo(Vector2 position, Color color, Vector3 texCoord) : IVertexType
    {
        private static readonly VertexDeclaration vertexDeclaration = new(
        [
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        ]);
        public Vector2 Position = position;
        public Color Color = color;
        public Vector3 TexCoord = texCoord;

        public readonly VertexDeclaration VertexDeclaration => vertexDeclaration;
    }
}
