using System;
using CalamityMod;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal.Miniboss;

public class ArsenalMinibossLeg : ModNPC
{
    public static float maxLegLength = 30;
    public static float minlegLength = 17;

    private static Asset<Texture2D> _topTex1;
    private static Asset<Texture2D> _topTex2;
    private static Asset<Texture2D> _botTex1;
    private static Asset<Texture2D> _botTex2;

    //Difficulty stuff
    public const int NormalDamage = 20;//Works in Journey!
    public const int ExpertDamage = 40; //Works in Journey!
    public const int P1Life = 500; //Works in Normal, Journey
    public const int EXP1Life = 1000; //works in Expert, Journey
    public const int REVP1Life = 1500; //works in Revengeance, Journey


    public bool Phase1 = true;


    private float ticker = 0;
    public override string Texture => this.GetPath().Replace("ArsenalMinibossLeg", "LegTopPhase1");

    public string TopTexture2 => this.GetPath().Replace("ArsenalMinibossLeg", "LegTopPhase2");

    public string BottomTexture1 => this.GetPath().Replace("ArsenalMinibossLeg", "LegBottomPhase1");
    public string BottomTexture2 => this.GetPath().Replace("ArsenalMinibossLeg", "LegBottomPhase2");

    public string ArmTexture => this.GetPath().Replace("ArsenalMinibossLeg", "Arm");


    //AI 0 references the boss core
    public NPC Arm => Main.npc[(int)NPC.ai[0]];

    public ArsenalJuggernaut Core => Main.npc[(int)Arm.ai[0]].ModNPC as ArsenalJuggernaut;


    //AI 3 controls the extension of the leg
    public float LegExtension => NPC.ai[2];

    private int frameCounter = 0;
    private int currentFrame = 0;


    //AI 3 controls which leg it is
    //If ai 3 is positive, it's on the left side
    //If abs val of ai 3 is less than 2, its on the bottom

    public bool IsOnLeft => NPC.ai[3] > 0;
    public bool IsBelow => (int)MathF.Abs(NPC.ai[3]) < 2;


    public Vector2 ArmPos => Arm.Center;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            _topTex1 = ModContent.Request<Texture2D>(Texture);
            _topTex2 = ModContent.Request<Texture2D>(TopTexture2);

            _botTex1 = ModContent.Request<Texture2D>(BottomTexture1);
            _botTex2 = ModContent.Request<Texture2D>(BottomTexture2);

            //glowTex = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
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


        }
        else if (Main.expertMode)//this is for expert mode
        {
            int targetExpertHealth = (int)((float)balance * NPC.lifeMax * EXP1Life / P1Life);
            NPC.lifeMax = targetExpertHealth;

        }

        if (CalamityWorld.death || Main.masterMode)
        {
            NPC.damage = (int)(1.12f * NPC.damage);

        }
        NPC.lifeMax /= 2;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }


    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        Core.OnHitBehavior(projectile.owner, damageDone);
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        Core.OnHitBehavior(player.whoAmI, damageDone);

    }

    private int immuneTime = 120;

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 35; //Balance later
        NPC.width = 54;
        NPC.height = 100;
        NPC.defense = 10; //Balance later
        NPC.lifeMax = 500; //Balance later
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        AIType = -1;

        NPC.alpha = 150;



        for (var k = 0; k < NPC.buffImmune.Length; k++) //Balancer later this means immune to EVERYTHNG
            NPC.buffImmune[k] = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit4;
        //NPC.DeathSound = SoundID.NPCDeath37;
        NPC.value = 100 * 16f; //Balance later
    }


    private bool firstTime = true;
    public override bool PreAI()
    {


        immuneTime--;

        NPC.immortal = immuneTime > 0;


        NPC.alpha = NPC.alpha > 0 ? NPC.alpha - 5 : 0;
        if (!Arm.active)
        {
            NPC.active = false;
        }
        else
        {
            ArsenalMinibossArm coreNPC = Arm.ModNPC as ArsenalMinibossArm;
            Phase1 = coreNPC.phase1;
        }

        //NPC.immortal = !Phase1;


        if (!Phase1)
        {

            int velX = (int)Core.NPC.velocity.X;
            if (velX > 0)
            {
                frameCounter++;

                int frameSpeed = 10 - (velX / 2);

                frameSpeed = frameSpeed > 0 ? frameSpeed : 1;

                if (frameCounter > frameSpeed)
                {
                    currentFrame++;
                    frameCounter = 0;
                    currentFrame %= 4;
                }
            }
        }

        var floorCheck = new Vector2(0, 60 * (IsBelow ? 1 : -1));

        var dist = MathF.Abs(ArmPos.Y - (NPC.Center.Y));

        var pos = (NPC.Center + floorCheck).ToTileCoordinates();



        if (!NPC.noGravity)
        {

            int offset = IsBelow ? -10 : 10;
            NPC.ai[2] = dist + offset;

            Tile tile = Main.tile[pos];
            if (!(tile != null && tile.HasUnactuatedTile && Main.tileSolid[tile.TileType]) && dist > maxLegLength)
            {

                NPC.ai[2] = maxLegLength;
                NPC.noGravity = true;
            }
        }

        if (NPC.alpha < 2)
        {
            NPC.ai[2] = MathHelper.Clamp(NPC.ai[2], 10, 75);
        }
        else
        {
            NPC.ai[2] = NPC.ai[2] < 75 ? NPC.ai[2] + 1.2f : 0;
        }


        return true;
    }

    private bool oldGravity = true;

    public override void AI()
    {
        //Sets the leg positions dynamically based on extensions



        var armVector = new Vector2(29 * (IsOnLeft ? -1 : 1), 0).RotatedBy(MathF.PI / 12 * (IsOnLeft ? -1 : 1));

        if (!IsBelow) NPC.GravityMultiplier *= -1;

        var legOffset = new Vector2(0, LegExtension * (IsBelow ? 1 : -1));




        if (NPC.noGravity)
            NPC.Center = ArmPos + armVector + legOffset;
        else
            NPC.Center = new Vector2(ArmPos.X + armVector.X + legOffset.X, NPC.Center.Y);



    }

    public override bool? CanFallThroughPlatforms()
    {
        return false;
    }


    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var effect = SpriteEffects.None;

        var FRAME_NUM = Phase1 ? 1 : 4;

        var texture = Phase1 ? _topTex1.Value : _topTex2.Value;

        if (IsBelow)
            texture = Phase1 ? _botTex1.Value : _botTex2.Value;

        if (NPC.IsABestiaryIconDummy)
            /*var texture = awakeTexture1;
            int height = texture.Height / frameNum;
            int startingH = overrideFrame % frameNum;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);
            NPC.frame = frame;*/
            return;

        var drawPos = NPC.Center - Main.screenPosition;



        var drawOrigin = new Vector2(texture.Width / 2, texture.Height / (2 * FRAME_NUM));

        var height = texture.Height / FRAME_NUM;

        var startingH = height * currentFrame;

        Color alpha = new Color(255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha);
        var frame = new Rectangle(0, startingH, texture.Width, height);
        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor.MultiplyRGBA(alpha), NPC.rotation, drawOrigin, NPC.scale, effect, 0);
        //Main.spriteBatch.Draw(glowTexture, drawPos, frame, Color.White, NPC.rotation, drawOrigin, NPC.scale, effect, 0);

        //DrawArm(spriteBatch, screenPos, drawColor);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return false;
    }
}