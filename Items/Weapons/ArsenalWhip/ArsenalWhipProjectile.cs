using ArsenalLabsBeta.NPCs;
using ArsenalLabsBeta.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Items.Weapons.ArsenalWhip
{
    // ArsenalProjectile is a placeholder name
    public class ArsenalWhipProjectile : BaseWhip
    {
        public static int HitCount
        {
            get => ArsenalLabsPlayer.ArsenalWhipHitCount;
            set => ArsenalLabsPlayer.ArsenalWhipHitCount = value;
        }
        public override void SetWhipStats()
        {
            Projectile.width = 6;
            Projectile.height = 8;
            Projectile.WhipSettings.Segments = 35;
            Projectile.WhipSettings.RangeMultiplier = 0.5f;
            Projectile.ownerHitCheck = true;
            tagDuration = 300;
            fishingLineColor = Color.DarkGray;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            tagType = ArsenalLabsTagID.Arsenal;
            HitCount++;
            if (HitCount >= 300) // to keep it from getting stupid high
                HitCount = 0;

            if (ArsenalLabsNPC.IsWormSegment.Contains(target.type) && HitCount % 4 != 0)
                tagType = -1;
            Player owner = Main.player[Projectile.owner];
            ArsenalLabsTag taggedTarget = target.GetGlobalNPC<ArsenalLabsTag>();

            // if mine doesn't exist on target, spawn it
            if (target.GetGlobalNPC<ArsenalLabsTag>().tagType != ArsenalLabsTagID.Arsenal && tagType == ArsenalLabsTagID.Arsenal && target.CanBeChasedBy())
            {
                int mineindex = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center + Main.rand.NextVector2Circular(target.width, target.height) / 2, Vector2.Zero, ModContent.ProjectileType<ArsenalMine>(), 15, Projectile.knockBack, Projectile.owner, target.whoAmI);
                taggedTarget.mineindex = mineindex;
            }
            else if (taggedTarget.mineindex >= 0)
            {
                Main.projectile[taggedTarget.mineindex].timeLeft /= 2;
            }

            ArsenalLabsTag.ApplyTagDebuff(target, tagType.Value, tagDuration);
            owner.MinionAttackTargetNPC = target.whoAmI;
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
