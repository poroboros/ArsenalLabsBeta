using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArsenalLabsBeta;
using ArsenalLabsBeta.Common.EntitySources;
using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.Ranged
{
    public class ArsenalRepeaterProj : BaseHoldOutShooterProjectile
    {
        private const int MaxChargedArrow = 3;

        private struct ShootInfo
        {
            public int damage;
            public float knockBack;
            public int usedAmmoItemId;
            public int charge;
            public ShootInfo()
            {
                damage = 0; knockBack = 0; usedAmmoItemId = 0; charge = 0;
            }
            public ShootInfo(int dmg, float kb, int ammo, int charge)
            {
                damage = dmg; knockBack = kb; usedAmmoItemId = ammo; this.charge = charge;
            }
        };
        private ShootInfo savedShootInfo = new();

        private bool oldLeft;
        private bool oldRight;
        private bool ChargeShoot
        {
            get => Projectile.ai[0] == 0;
            set => Projectile.ai[0] = value ? 0 : 1;
        }
        private int ChargeTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private bool CanShootChargedArrow
        {
            get => Projectile.ai[2] == 0;
            set => Projectile.ai[2] = value ? 0 : 1;
        }
        public override string Texture => "ArsenalLabsBeta/Items/Weapons/Ranged/Repeaters/ArsenalRepeater";
        public override void InitProjectile()
        {
            Offset = new Vector2(10, 0);
            RotationLerpAmount = 0;
            oldLeft = oldRight = false;
        }


        // dirty code sry
        // will refactor soon (soon
        protected override void AI_ShootThings()
        {
            if (!ShouldKill)
            {
                float speed = SItem.shootSpeed;
                Vector2 baseVec = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
                int damage = Owner.GetWeaponDamage(SItem);
                float knockBack = Owner.GetWeaponKnockback(SItem);
                if (ChargeShoot)
                {
                    if (ChargeTime >= 0)
                    {
                        // prevent reset
                        Timer--;
                    }
                    int timePerCharge = (int)(WeaponUsetime * 1.5f);
                    if (ChargeTime / timePerCharge < MaxChargedArrow)
                    {
                        if (OwnerClient && ChargeTime % timePerCharge == 0 &&
                        Owner.PickAmmo(SItem, out _, out _, out damage, out knockBack, out int usedAmmoItemId, FirstFrame) && SItem.CheckCharge(pay: !FirstFrame))
                        {
                            savedShootInfo = new(damage, knockBack, usedAmmoItemId, 1 + (ChargeTime / timePerCharge));
                            CombatText.NewText(new Rectangle((int)(Owner.position.X), (int)(Owner.position.Y - 20), 100, 20), Color.White, $"charge {ChargeTime / timePerCharge}");
                        }
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SoundEngine.PlaySound(ArsenalLabsBeta.GetSound("Item/Ranged/ArsenalRepeater/charge") with
                            {
                                //MaxInstances = 2,
                                Volume = 0.55f,
                                Pitch = 0.8f,
                                IsLooped = false,
                                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                            }, Projectile.Center);
                        }
                    }
                    if (OwnerClient && ((!Main.mouseLeft && oldLeft) || CanShootChargedArrow))
                    {
                        CanShootChargedArrow = true;
                        if (ChargeTime / WeaponUsetime >= 1)
                        {
                            float damageMult = Math.Min((float)ChargeTime / WeaponUsetime, 3);
                            var source = Owner.ArsenalLabs().GetSource_ItemUse_WithPotentialAmmo_ShouldNotTriggerShoot(SItem, savedShootInfo.usedAmmoItemId);

                            SItem.channel = false;
                            ArsenalLabsProjectile.CombindHookShoot(Owner, SItem, source as EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot, Owner.Center, baseVec * savedShootInfo.charge);
                            SItem.channel = true;

                            Projectile.NewProjectile(source, Projectile.Center, baseVec * savedShootInfo.charge, ModContent.ProjectileType<ArsenalRepeaterArrow>(), (int)(savedShootInfo.damage * damageMult), savedShootInfo.knockBack, Projectile.owner);
                            savedShootInfo = new();
                            // TODO: net code for sound playing... needs refactor
                            // my plan is using signals (implemented as a bitarray) and sync them to notify other clients to play the sound, instead of sync the shooting state
                            SoundEngine.PlaySound(ArsenalLabsBeta.GetSound("Item/Ranged/ArsenalRepeater/fire"), Projectile.Center);
                            ChargeTime = -1;
                            ShouldKill = true;
                        }
                        else
                        {
                            ChargeTime++;
                        }
                    }
                    else
                    {
                        ChargeTime++;
                    }
                }
                else
                {
                    if (Timer == 0 && OwnerClient &&
                        Owner.PickAmmo(SItem, out _, out _, out damage, out knockBack, out int usedAmmoItemId, FirstFrame) && SItem.CheckCharge(pay: !FirstFrame))
                    {
                        var source = Owner.ArsenalLabs().GetSource_ItemUse_WithPotentialAmmo_ShouldNotTriggerShoot(SItem, usedAmmoItemId);

                        SItem.channel = false;
                        ArsenalLabsProjectile.CombindHookShoot(Owner, SItem, source as EntitySource_ItemUse_WithAmmo_ShouldNotTriggerShoot, Owner.Center, baseVec);
                        SItem.channel = true;

                        Projectile.NewProjectile(source, Projectile.Center, baseVec, ModContent.ProjectileType<ArsenalRepeaterArrow>(), damage, knockBack, Projectile.owner);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SoundEngine.PlaySound(ArsenalLabsBeta.GetSound("Item/Ranged/ArsenalRepeater/fire"), Projectile.Center);
                        }
                    }
                }
            }
        }
        protected override void AI_UpdateOwnerAndProj()
        {
            if (OwnerClient)
            {
                if ((!Main.mouseLeft && oldLeft && !CanShootChargedArrow) ||
                (!Main.mouseRight && oldRight))
                {
                    ShouldKill = true;
                }
                if (Main.mouseLeft && ChargeShoot && ChargeTime >= 0)
                {
                    ChargeTime++;
                }
                oldLeft = Main.mouseLeft;
                oldRight = Main.mouseRight;
            }
            base.AI_UpdateOwnerAndProj();
        }
        protected override void ResetProjectile()
        {
            if (!OwnerClient)
            {
                base.ResetProjectile();
                return;
            }
            if (ShouldKill && !Main.mouseLeft && !Main.mouseRight)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Timer = 0;
                ShouldKill = false;
                WeaponUsetime = CombinedHooks.TotalUseTime(SItem.useTime, Owner, SItem);
            }
            if (Main.mouseLeft)
            {
                ChargeShoot = true;
            }
            else
            {
                ChargeShoot = false;
            }
            ChargeTime = 0;
            CanShootChargedArrow = false;
        }
    }
}
