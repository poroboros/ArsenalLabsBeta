using CalamityMod.Graphics.Renderers;
using ArsenalLabsBeta.Buffs.Mounts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace ArsenalLabsBeta.Items.Mounts
{
    public class JuggernautMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            Terraria.On_Mount.Draw += DisableMountFlip;
            Terraria.On_Mount.UpdateFrame += AllowReverseAnimation;
            MountData.buff = ModContent.BuffType<JuggernautBuff>();
            MountData.heightBoost = 0;
            MountData.fallDamage = 1f;
            MountData.runSpeed = 4f;
            MountData.dashSpeed = 2f;
            MountData.flightTimeMax = 0;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 0;
            MountData.acceleration = 0.2f;
            MountData.jumpSpeed = 0f;
            MountData.blockExtraJumps = true;
            MountData.totalFrames = 8;
            MountData.constantJump = false;
            MountData.spawnDust = 75;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 80;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = 0;
            MountData.bodyFrame = 3;
            MountData.yOffset = -18;
            MountData.playerHeadOffset = 0;
            MountData.standingFrameCount = 0;
            MountData.standingFrameDelay = 0;
            MountData.standingFrameStart = 0;
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 0;
            MountData.inAirFrameDelay = 0;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 0;
            MountData.idleFrameDelay = 0;
            MountData.idleFrameStart = 0;
            MountData.runningFrameCount = 8;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = 8;
            MountData.swimFrameDelay = 24;
            MountData.swimFrameStart = 0;
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            MountData.textureWidth = MountData.backTexture.Width();
            MountData.textureHeight = MountData.backTexture.Height();
        }
        public override void UpdateEffects(Player player)
        {
            MountData.runSpeed = player.accRunSpeed;
            MountData.acceleration = player.runAcceleration;
        }

        public static void DisableMountFlip(Terraria.On_Mount.orig_Draw orig, Mount self, List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
        {
            if (drawPlayer.mount.Type == ModContent.MountType<JuggernautMount>())
            {
                orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, SpriteEffects.None, shadow);
            }
            else
            {
                orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, playerEffect, shadow);
            }
        }

        public static void AllowReverseAnimation(Terraria.On_Mount.orig_UpdateFrame orig, Mount self, Player mountedPlayer, int state, Vector2 velocity)
        {
            if (mountedPlayer.mount.Type == ModContent.MountType<JuggernautMount>())
            {
                float speed = Math.Abs(velocity.X);

                self._frameCounter += speed / 2;
                if (speed >= 0f)
                {
                    if (mountedPlayer.velocity.X < 0)
                    {
                        if (self._frameCounter > (float)self._data.runningFrameDelay)
                        {
                            self._frameCounter -= self._data.runningFrameDelay;
                            self._frame--;
                        }
                        if (self._frame < self._data.runningFrameStart || self._frame >= self._data.runningFrameStart + self._data.runningFrameCount)
                        {
                            self._frame = self._data.runningFrameStart + self._data.runningFrameCount - 1;
                        }
                    }
                    else if (mountedPlayer.velocity.X > 0)
                    {
                        if (self._frameCounter > (float)self._data.runningFrameDelay)
                        {
                            self._frameCounter -= self._data.runningFrameDelay;
                            self._frame++;
                        }
                        if (self._frame < self._data.runningFrameStart || self._frame >= self._data.runningFrameStart + self._data.runningFrameCount)
                        {
                            self._frame = self._data.runningFrameStart;
                        }
                    }
                }
            }
            else
            {
                orig(self, mountedPlayer, state, velocity);
            }
        }
    }
}