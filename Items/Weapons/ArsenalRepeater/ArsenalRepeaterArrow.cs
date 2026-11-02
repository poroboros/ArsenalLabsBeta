using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.Projectiles.Ranged
{


    public class ArsenalRepeaterArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 10;

            Projectile.aiStyle = ProjAIStyleID.Arrow;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.CloneDefaults(ProjectileID.IchorArrow);

        }

        /*
         * This is the Arsenal Repeater, a weapon where its alt fire
         * shoots the arrows normally like its a repeater, but
         * the main fire is where it does not shoot, and instead charges to maximum
         * at some point if the player stops holding the fire button it
         * shoots with extra damage and velocity.
         * 
         * How does one do that? Lighty wants to deal with it like this:
         * On the weapon item itself, it just shoots normally like its a mere projectile.
         * But the projectile itself is going to have that charge AI.
         * 
         * For the main fire:
         * 1. The projectile should NOT move at all while the player holds the charge
         * Instead follows around the player's mouse movement like Last Prism's projectile.
         * 2. If the player at some point in time stops holding the main fire, then it counts as
         * it being fired, at which it should shoot the projectile that has the base values taken
         * from Shoot() function + extra damage and velocity.
         * 
         * Alternate main fire:
         * 1. It shoots an invisible projectile that basically doesn't move and doesn't do damage, and
         * it basically acts as a charge counter 
         * 2. If the player stops at holding at any point, it should basically summon in the actual projectile
         * that has all the changed stuff
         * 
         * Alt alt main fire:
         * 1. We get to hold the projectile like usual, disabling the AI if the player didn't release it yet
         * otherwise:
         * 2. Translation by velocity: We make the projectile follow the player by basically applying
         * Player's velocity to the projectile.
         * 3. Rotation by mouse's velocity: We somehow calculate the direction of the mouse compared to
         * player's center, at which it gives us the angle to set the projectile to.
         * 
         * For the alt fire:
         * 1. The projectile should recognize that alt fire has been pressed, which is enabled
         * by the weapon's AltFunctionUse().
         * 2. Then the projectile should just shoot like normal with the values stored in Shoot().
         * 
         */


    }



}
