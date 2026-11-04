using System;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Turret;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal.Miniboss;

public class RapidFireGun : ModNPC
{

    //Difficulty Stuff
    public const int NormalFocusToFire = 250; //This is how long it takes for the gun to lock on and fire
    public const float NormalLaserVelMod = 0.5f;

    public const int ExpertFocusToFire = 250;
    public const float ExpertLaserVelMod = 0.5f;

    public const int RevFocusToFire = 250;
    public const float RevLaserVelMod = 0.5f;

    public const int MasterFocusToFire = 250;
    public const float MasterLaserVelMod = 0.5f;

    int minFocusToFire = NormalFocusToFire;
    float laserVelMod = NormalLaserVelMod;

    public const int NormalDamage = 20;//Works in Journey!
    public const int ExpertDamage = 40; //Works in Journey!
    public const int P1Life = 350; //Works in Normal, Journey
    public const int EXP1Life = 700; //works in Expert, Journey
    public const int REVP1Life = 1400; //works in Revengeance, Journey


    private static Asset<Texture2D> tex;
    private static Asset<Texture2D> glowTex;

    //AI 0 references the boss core
    public NPC Core => Main.npc[(int)NPC.ai[0]];
    public Player Target => NPC.ai[1] >= 0 ? Main.player[(int)NPC.ai[1]] : null;

    private float focus => NPC.ai[3];

    float spotlightDeg => (NPC.ai[2] + 360) % 360;
    public float spotlightRotation => MathHelper.ToRadians(spotlightDeg);

    public override void Load()
    {
        if (!Main.dedServ)
        {
            tex = ModContent.Request<Texture2D>(Texture);

            glowTex = ModContent.Request<Texture2D>(Texture + "Glow");
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 35; //Balance later
        NPC.width = 25;
        NPC.height = 25;
        NPC.defense = 10; //Balance later
        NPC.lifeMax = 350; //Balance later
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        AIType = -1;


        for (var k = 0; k < NPC.buffImmune.Length; k++) //Balancer later this means immune to EVERYTHNG
            NPC.buffImmune[k] = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit4;
        //NPC.DeathSound = SoundID.NPCDeath37;
        NPC.value = 100 * 16f; //Balance later
    }

    public override void SetStaticDefaults()
    {
        this.HideFromBestiary();
        base.SetStaticDefaults();
    }


    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
    {
        //the reason for using these divisors and multipliers early is because of the necesity to create compatability in both Expert/Master, and Journey
        //In expert, multipliers are run first, so you can set values easily, but they are run second in Journey Mode (for some reason tmodloader thought this was smart, and yes, it is a Tmodloader issue, not vanilla)
        //This means that in Journey mode, NPC.lifeMax is equal to Normal Mode NPC.lifeMax, and then multiplied by 2 after THIS method is run
        //However, in Expert mode, NPC.lifeMax is equal to 2x Normal Mode NPC.LifeMax
        //Thus, by manipulating numbers and dividing by 2, we can synchronise both values by the commutative property of multiplication.
        //The same logic must be maintained for the NPC.damage stat calculations
        //Since Phase 2 stat changes happen real-time, they are not subject to the Journey Mode slider, and thus don't need to be changed. Though right now they do not scale dynamically with the slider.
        //This should also solve our problem with weird multiplayer health scaling :)
        //The reason for casting to Floats is to make the order easier to manage so there is no accidental truncation when dividing
        NPC.damage = NPC.damage * ExpertDamage / NormalDamage / 2;
        if (CalamityWorld.revenge)
        {
            NPC.lifeMax = (int)((float)balance * NPC.lifeMax * REVP1Life / P1Life);
            NPC.damage = (int)(1.12f * NPC.damage);

            minFocusToFire = RevFocusToFire;
            laserVelMod = RevLaserVelMod;


        }
        else if (Main.expertMode)//this is for expert mode
        {
            int targetExpertHealth = (int)((float)balance * NPC.lifeMax * EXP1Life / P1Life);
            NPC.lifeMax = targetExpertHealth;


            minFocusToFire = ExpertFocusToFire;
            laserVelMod = ExpertLaserVelMod;
        }

        if (CalamityWorld.death || Main.masterMode)
        {
            NPC.damage = (int)(1.12f * NPC.damage);

            minFocusToFire = MasterFocusToFire;
            laserVelMod = MasterLaserVelMod;

        }
        NPC.lifeMax /= 2;
    }


    private bool firstTime;

    private int fireCounter = 100;
    public override bool PreAI()
    {

        Vector2 posOffset = new Vector2(0, 55);
        if (Core is not null && Core.active)
        {
            NPC.Center = Core.Center + posOffset.RotatedBy(Core.rotation);
        }
        else
        {
            NPC.active = false;
        }


        if (firstTime)
        {
            NPC.ai[1] = NPC.FindClosestPlayer();
            firstTime = false;
        }

        Vector2 vel = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);


        if (focus > minFocusToFire)
        {
            fireCounter = 70;
            NPC.ai[3] = 0;
        }

        fireCounter--;

        if (fireCounter > 0 && fireCounter % 10 == 0)
        {
            int projType = ProjectileID.MartianWalkerLaser; //TODO
            int proj2 = Projectile.NewProjectile(new EntitySource_Parent(NPC), NPC.Center + vel * 2, vel * 8 * laserVelMod, projType, 10, 0);

            Main.projectile[proj2].friendly = false;
        }

        NPC.rotation = spotlightRotation - MathHelper.PiOver2;
        return base.PreAI();
    }

    public override void AI()
    {

        Vector2 vel = (-NPC.Center + Target.Center).SafeNormalize(Vector2.Zero);
        float playerRad = vel.ToRotation();
        float playerRot = (360 + MathHelper.ToDegrees(playerRad)) % 360;

        //This code makes it so the spotlight doesn't weirdly take the longer arc, it should always take the shortest rotation
        if (playerRot <= 180 && (spotlightDeg - playerRot) >= 180)
        {
            playerRot += 360;
        }
        else if (playerRot >= 180 && (playerRot - spotlightDeg) >= 180)
        {
            playerRot -= 360;
        }
        if (MathF.Abs(playerRot - spotlightDeg) < 2)
        {

        }

        else if (playerRot > spotlightDeg)
        {

            NPC.ai[2] += 2.5f;

        }
        else
        {
            NPC.ai[2] -= 2.5f;
        }

        if (Math.Abs(playerRot - spotlightDeg) < 10)
            NPC.ai[3] += 1;

    }

    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        var effect = SpriteEffects.None;

        var FRAME_NUM = 1; //remove if this is to be animated

        var texture = tex.Value;
        var glowTexture = glowTex.Value;


        var drawPos = NPC.Center - Main.screenPosition;



        var drawOrigin = new Vector2(texture.Width / 2, texture.Height / (2 * FRAME_NUM));

        var height = texture.Height / FRAME_NUM;
        //height += 1;
        var frame = new Rectangle(0, 0, texture.Width, height);
        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);
        Main.spriteBatch.Draw(glowTexture, drawPos, frame, Color.White, NPC.rotation, drawOrigin, NPC.scale, effect, 0);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {


        return false;
    }


}