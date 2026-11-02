using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using ArsenalLabsBeta.Projectiles.BaseProjectiles.Interfaces;
using CalamityMod;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace ArsenalLabsBeta.Projectiles.Magic.Florapedia
{
    public class FlorapediaHoldout : BaseHoldOutShooterProjectile
    {
        public static readonly string SoundPath = "Item/Magic/Florapedia/";
        private const int FlowerCD = 5;
        private const int FlowerCount = 3;

        public static readonly SoundStyle HeldStart = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaStart");
        public static readonly SoundStyle HeldLoop = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaLoop") with { MaxInstances = 2 };
        public static readonly SoundStyle FlowerFabricate1 = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaFabricateProjectile1") with { MaxInstances = 3, Volume = 0.5f };
        public static readonly SoundStyle FlowerFabricate2 = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaFabricateProjectile2") with { MaxInstances = 3, Volume = 0.5f };
        public static readonly SoundStyle FlowerFabricate3 = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaFabricateProjectile3") with { MaxInstances = 3, Volume = 0.5f };
        public static readonly SoundStyle FlowerShoot1 = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaShoot1") with { MaxInstances = 3, Volume = 0.5f };
        public static readonly SoundStyle FlowerShoot2 = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaShoot2") with { MaxInstances = 3, Volume = 0.5f };
        public static readonly SoundStyle FlowerShoot3 = ArsenalLabsBeta.GetSound(SoundPath + "FlorapediaShoot3") with { MaxInstances = 3, Volume = 0.5f };
        public static readonly Color ScreenColor = new(31, 251, 255, 0);

        private bool hasChargeOrMana;
        private bool firstShot;
        private int attackCD;

        private SlotId? bookLoopSlot;

        private readonly List<int> hologram = [];
        public EntityNetSoundManager SoundManager { get; set; }

        public override void InitProjectile()
        {
            Projectile.DamageType = DamageClass.Magic;
            hasChargeOrMana = true;
            firstShot = true;
            hologram.Clear();
            attackCD = FlowerCD;
            bookLoopSlot = null;
            SoundManager = new EntityNetSoundManager([HeldStart, FlowerFabricate1, FlowerFabricate2, FlowerFabricate3]);
        }

        protected override void AI_ShootThings()
        {
            if (!OwnerClient)
            {
                SoundManager.PlaySoundPassively(Projectile.Center);
                return;
            }
            if (ShouldKill)
                return;
            if (FirstFrame)
                SoundManager.PlaySoundActively(0, Projectile.Center);

            int currentFlower = Timer / attackCD;
            if (Timer % attackCD == 0 && currentFlower < FlowerCount)
            {
                if (!Owner.channel ||
                    !firstShot && (!Owner.CheckMana(SItem, pay: true) || !SItem.CheckCharge()))
                {
                    ShouldKill = true;
                    return;
                }

                int damage = Owner.GetWeaponDamage(SItem);
                float knockBack = Owner.GetWeaponKnockback(SItem);
                Vector2 pos = Owner.RotatedRelativePoint(Owner.MountedCenter);

                // create a flower
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos + new Vector2(Owner.direction * 30, 0), new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-15, -10)), ModContent.ProjectileType<FlorapediaFlower>(), damage, knockBack, Projectile.owner, Projectile.whoAmI, 0, 0);

                hologram.Add(proj);
                if (currentFlower == 0)
                {
                    GlobalShoot(SItem, pos, damage, knockBack);
                    SoundManager.PlaySoundActively(Main.rand.Next(1, 4), Projectile.Center);
                }

                firstShot = false;
            }
        }

        protected override void ResetProjectile()
        {
            if (!OwnerClient)
                base.ResetProjectile();
            else if (!hasChargeOrMana || ShouldKill)
                Projectile.Kill();
            else
            {
                Timer = 0;
                ShouldKill = false;
                float useTimeMult = CombinedHooks.TotalUseTimeMultiplier(Owner, SItem);
                attackCD = Math.Max(1, (int)(FlowerCD * useTimeMult));
                WeaponUsetime = Math.Max(1, (int)(SItem.useTime * useTimeMult));
                if (WeaponUsetime <= attackCD * 3)
                    WeaponUsetime = attackCD * 3 + 1;
            }
        }

        protected override void AI_UpdateOwnerAndProj()
        {
            Projectile.timeLeft++;
            Projectile.Center = Owner.Center;
            Projectile.direction = Owner.direction;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.heldProj = Projectile.whoAmI;

            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, -Owner.direction * 0.4f);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -Owner.direction * (0.1f * MathF.Sin(Main.GlobalTimeWrappedHourly * 13) + 1.5f));

            if (FirstFrame && bookLoopSlot is null)
                bookLoopSlot = SoundEngine.PlaySound(HeldStart, Owner.Center);
            else if (SoundEngine.TryGetActiveSound(bookLoopSlot.Value, out ActiveSound activeSound))
            {
                if (!Owner.channel)
                    activeSound.Volume *= 0.95f;
                activeSound.Position = Projectile.Center;
            }
            if (!Main.dedServ)
                Lighting.AddLight(Projectile.Center, ScreenColor.ToVector3() / 2);
        }

        public override void OnKill(int timeLeft)
        {
            ClearAllHolograms();
            if (SoundEngine.TryGetActiveSound(bookLoopSlot.Value, out ActiveSound activeSound))
                activeSound.Stop();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 heldDrawCenter = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(26f * Owner.direction, 0f);
            Matrix mat = Matrix.CreateTranslation(new Vector3(-heldDrawCenter, 0))
                * Matrix.CreateScale(new Vector3(1, 0.5f, 1))
                * Matrix.CreateRotationZ(Owner.direction * (-0.2f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3) * 0.05f))
                * new Matrix(
                    1, 0, 0, 0,
                    -Owner.direction * 0.5f * Owner.gravDir * Owner.gravDir, Owner.gravDir, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1) // shear
                * Matrix.CreateTranslation(new Vector3(heldDrawCenter - Main.screenPosition, 0))
                * Main.Transform;

            Main.spriteBatch.End();
            // cullnone -> gravdir :<
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, mat);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects spriteEffects = Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle screen = texture.Frame(1, 2, 0, 0);
            Rectangle framework = texture.Frame(1, 2, 0, 1);
            Vector2 origin = screen.Size() / 2;
            const float drawScale = 1;

            Main.EntitySpriteDraw(texture, heldDrawCenter, framework, lightColor, 0, origin, drawScale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, heldDrawCenter, screen, new Color(200, 200, 200, 50), 0, origin, drawScale, spriteEffects, 0);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            SoundManager.SendSoundSignal(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            SoundManager.ReceiveSoundSignal(reader);
        }

        private void GlobalShoot(Item SItem, Vector2 pos, int damage, float knockBack)
        {
            CalamityPlayer calPlayer = Owner.Calamity();

            bool savedLuxorGift = calPlayer.luxorsGift;
            if (calPlayer.luxorsGift)
            {
                // int luxorDmg = Owner.ApplyArmorAccDamageBonusesTo(damage); // buffed, should be x0.3
                int luxorDmg = 3; // buffed, should be x0.3, temporarily set to arbitrary number to make sure that it works first
                pos += new Vector2(Owner.direction * 30, 0);
                Vector2 vel = new(Main.rand.Next(-5, 5), Main.rand.Next(-24, -20));

                int luxor = Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<FlorapediaLuxor>(), luxorDmg, knockBack, Projectile.owner, Projectile.whoAmI);
                hologram.Add(luxor);
            }

            Owner.channel = false;
            SItem.channel = false;
            calPlayer.luxorsGift = false;
            ArsenalLabsProjectile.CombindHookShoot(Owner, SItem, ItemID.None, pos, (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero) * SItem.shootSpeed);
            SItem.channel = true;
            Owner.channel = true;

            calPlayer.luxorsGift = savedLuxorGift;
        }


        private void ClearAllHolograms()
        {
            foreach (int i in hologram)
            {
                if (Main.projectile[i].ModProjectile is IFlorapediaHologram hologram)
                    hologram.Disappear();
            }
        }

        public void RemoveProj(int projID)
        {
            hologram.Remove(projID);
        }

        public static void RandomSound(Vector2 pos, params SoundStyle[] sounds)
        {
            int len = sounds.Length;
            SoundEngine.PlaySound(sounds[Main.rand.Next(len)], pos);
        }
    }
}
