using System;
using System.IO;
using CalamityMod;
using CalamityMod.World;
using ArsenalLabsBeta.NPCs.Arsenal.Miniboss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using NPCs.Arsenal.Projectiles;

namespace ArsenalLabsBeta.NPCs.Arsenal.Miniboss;

[AutoloadBossHead]
public class ArsenalJuggernaut : ModNPC
{

    //Difficulty Stuff
    public const int NormalWalkSpeed = 4;
    public const int NormalPhase2Accel = 1;
    public const int NormalFireDelay = 200;

    public const int ExpertWalkSpeed = 4;
    public const int ExpertPhase2Accel = 1;
    public const int ExpertFireDelay = 200;
    public const int ExpertNPCSpawnRate = 800;


    public const int RevWalkSpeed = 4;
    public const int RevPhase2Accel = 1;
    public const int RevFireDelay = 200;
    public const int RevNPCSpawnRate = 800;

    public const int MasterWalkSpeed = 4;
    public const int MasterPhase2Accel = 1;
    public const int MasterFireDelay = 200;
    public const int MasterNPCSpawnRate = 800;


    public const int NormalDamage = 20;//Works in Journey!
    public const int ExpertDamage = 40; //Works in Journey!
    public const int P1Life = 1200; //Works in Normal, Journey
    public const int EXP1Life = 2200; //works in Expert, Journey
    public const int REVP1Life = 3300; //works in Revengeance, Journey




    private int walkSpeed = NormalWalkSpeed;
    private float phase2Accel = NormalPhase2Accel;
    private int fireDelay = NormalFireDelay;
    private int npcSpawnRate = ExpertNPCSpawnRate;




    private static Asset<Texture2D> tex;
    private static Asset<Texture2D> tex2;
    private static Asset<Texture2D> glowTex;
    private static Asset<Texture2D> spawnAnim;
    private static Asset<Texture2D> transformAnim;

    public override string Texture => this.GetPath().Replace(GetType().Name, "BodyPhase1");
    private string GlowTexture => this.GetPath().Replace(GetType().Name, "BodyGlow");

    private string Phase2Texture => this.GetPath().Replace(GetType().Name, "BodyPhase2");
    private string SpawnTexture => this.GetPath().Replace(GetType().Name, "ArsenalSpawn");
    private string TransTexture => this.GetPath().Replace(GetType().Name, "Phase2Transition");

    public NPC Arm1 => Main.npc[(int)(NPC.ai[0])];
    public NPC Arm2 => Main.npc[(int)(NPC.ai[1])];

    public NPC Arm3 => Main.npc[(int)(NPC.ai[2])];
    public NPC Arm4 => Main.npc[(int)(NPC.ai[3])];

    public NPC[] Arms => new NPC[] { Arm1, Arm2, Arm3, Arm4 };

    public NPC Leg1 => Arm1 is null ? null : Main.npc[(int)(Arm1.ai[2])];
    public NPC Leg2 => Arm2 is null ? null : Main.npc[(int)(Arm2.ai[2])];
    public NPC Leg3 => Arm3 is null ? null : Main.npc[(int)(Arm3.ai[2])];
    public NPC Leg4 => Arm4 is null ? null : Main.npc[(int)(Arm4.ai[2])];

    public NPC[] Legs => new NPC[] { Leg1, Leg2, Leg3, Leg4 };

    public NPC Gun => GunIndex is not null ? Main.npc[(int)(GunIndex)] : null;

    public static int TargetDamageThreshold = 40;
    public static int MaxTargetCooldown = 300;

    public Player TargetPlayer => TargetIndex is not null && Main.player[(int)(TargetIndex)].active ? Main.player[(int)(TargetIndex)] : null;


    /// <summary>
    /// ID of the mounted gun NPC
    /// </summary>
    public byte? GunIndex;

    /// <summary>
    ///  Who the boss should pursue
    /// </summary>
    public byte? TargetIndex;

    public float TargetCooldown = 0;


    //Radius for collision in phase 2;
    public float HitRadius = 70;




    /// <summary>
    /// Used for timing walk animation in phase 1
    /// </summary>
    private float _globalWalkCounter = 0;

    public bool Phase1 = true;
    private short _phase2Gravity = 1;
    Vector2 _phase2TargetPos = Vector2.Zero;

    private static int spawnAnimSpeed = 4;
    private static int transAnimSpeed = 4;

    /// <summary>
    /// Tme before action is taken when spawned
    /// </summary>
    private int _spawnDelay = 45 * spawnAnimSpeed;


    /// <summary>
    /// TIme before action is taken when entering phase 2
    /// </summary>
    private float _transformDelay = 16 * transAnimSpeed;

    /// <summary>
    /// Controls the shakiness of the core when it stomps
    /// </summary>
    private float _wobble = .08f;
    public override void Load()
    {
        if (!Main.dedServ)
        {
            tex = ModContent.Request<Texture2D>(Texture);
            glowTex = ModContent.Request<Texture2D>(GlowTexture);
            tex2 = ModContent.Request<Texture2D>(Phase2Texture);
            spawnAnim = ModContent.Request<Texture2D>(SpawnTexture);
            transformAnim = ModContent.Request<Texture2D>(TransTexture);
        }
    }


    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 35; //Balance later
        NPC.width = 90;
        NPC.height = 220;
        NPC.defense = 10; //Balance later
        NPC.lifeMax = 1200; //Balance later
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        AIType = -1;
        for (int k = 0; k < NPC.buffImmune.Length; k++) //Balancer later this means immune to EVERYTHNG
        {
            NPC.buffImmune[k] = true;
        }
        NPC.noGravity = false;
        NPC.HitSound = SoundID.NPCHit4;
        //NPC.DeathSound = SoundID.NPCDeath37;
        NPC.value = 100 * 16f; //Balance later
        NPC.noTileCollide = false;
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

            walkSpeed = RevWalkSpeed;
            fireDelay = RevFireDelay;
            phase2Accel = RevPhase2Accel;
            npcSpawnRate = RevNPCSpawnRate;

        }
        else if (Main.expertMode)//this is for expert mode
        {
            int targetExpertHealth = (int)((float)balance * NPC.lifeMax * EXP1Life / P1Life);
            NPC.lifeMax = targetExpertHealth;

            walkSpeed = ExpertWalkSpeed;
            fireDelay = ExpertFireDelay;
            phase2Accel = ExpertPhase2Accel;
            npcSpawnRate = ExpertNPCSpawnRate;

        }

        if (CalamityWorld.death || Main.masterMode)
        {
            NPC.damage = (int)(1.12f * NPC.damage);

            walkSpeed = MasterWalkSpeed;
            fireDelay = MasterFireDelay;
            phase2Accel = MasterPhase2Accel;
            npcSpawnRate = MasterNPCSpawnRate;
        }
        NPC.lifeMax /= 2;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }


    public override void SendExtraAI(BinaryWriter writer)
    {
        if (GunIndex is not null)
            writer.Write((byte)GunIndex);
        if (TargetIndex is not null)
            writer.Write((byte)TargetIndex);
        writer.Write(TargetCooldown);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        GunIndex = reader.ReadByte();
        TargetIndex = reader.ReadByte();
        TargetCooldown = reader.ReadSingle();
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        ;
        OnHitBehavior(projectile.owner, damageDone);
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        OnHitBehavior(player.whoAmI, damageDone);

    }


    public void OnHitBehavior(int owner, int damage)
    {

        if (TargetCooldown < 1)
        {

            if (TargetPlayer is null || damage > TargetDamageThreshold)
            {

                TargetIndex = (byte)owner;
                TargetCooldown = MaxTargetCooldown;

                NPC.netUpdate = true;
            }
        }

    }

    /// <summary>
    /// Utility to netsync all his children
    /// </summary>
    public void NetUpdateAll()
    {

        NPC.netUpdate = true;
        Arm1.netUpdate = true;
        Arm2.netUpdate = true;
        Arm3.netUpdate = true;
        Arm4.netUpdate = true;
        Leg1.netUpdate = true;
        Leg2.netUpdate = true;
        Leg3.netUpdate = true;
        Leg4.netUpdate = true;
    }


    public override bool PreAI()
    {
        NPC.height = 220;
        //setup
        if (_spawnDelay > 0)
        {


            _spawnDelay--;

            NPC.noGravity = true;

            int spawnFrame = 45 - (_spawnDelay / 4);




            if (spawnFrame is > 3 and < 10)
                NPC.Center += new Vector2(0, -.2f);

            if (_spawnDelay == 50)
                InitializeBoss();
            return false;
        }


        NPC.velocity.Y = 8;

        if (!Phase1)
        {
            NPC.velocity.Y = _phase2Gravity * 8;
            NPC.GravityMultiplier *= _phase2Gravity;
        }
        TargetCooldown--;



        //Make sure that if one set of legs is destroyed, they all are
        if (!Arm1.active && Phase1)
        {
            Phase1 = false;

            _transformDelay = 120;
            _phase2Gravity = -1; //if bottom legs die, he goes to the top
            Arm2.active = false;
            Leg1.active = false;
            Leg2.active = false;
        }
        if (!Arm3.active && Phase1)
        {
            Phase1 = false;
            _transformDelay = 120;
            _phase2Gravity = 1; //
            Arm4.active = false;
            Leg3.active = false;
            Leg4.active = false;
        }



        if (!Phase1 && _transformDelay > 0)
        {
            PhaseShiftRoutine();
            _transformDelay--;
            return false;
        }





        return true;
    }


    private int fireTimer = 0;

    public override void AI()
    {


        Vector2 cannonPos = new Vector2(0, -50).RotatedBy(NPC.rotation);


        if (Phase1)
        {
            MovementRoutinePhase1();

            if (Main.expertMode || Main.masterMode || CalamityWorld.death || CalamityWorld.revenge)
            {
                fireTimer++;

                if (fireTimer > 800)
                {

                    Vector2 spawnPos = NPC.Center + new Vector2(0, 100);
                    int enemyType = ModContent.NPCType<DisposalUnit>();


                    for (int i = 0; i < 3; i++)
                        Dust.NewDust(spawnPos, 5, 5, DustID.Electric);
                    NPC.NewNPCDirect(new EntitySource_SpawnNPC(), spawnPos, enemyType);

                    fireTimer = 0;
                }
            }
        }
        else
        {
            MovementRoutinePhase2();

            fireTimer++;


            if (fireTimer > fireDelay && (NPC.rotation < -.7f || NPC.rotation > .7f))
            {
                Vector2 vel2 = new Vector2(0, -5).RotatedBy(NPC.rotation) + new Vector2(NPC.velocity.X, 0);

                int projType = ModContent.ProjectileType<ArsenalBomb>();
                int proj2 = Projectile.NewProjectile(new EntitySource_Parent(NPC), NPC.Center + cannonPos, vel2, projType, 10, 0);

                Main.projectile[proj2].friendly = false;
                fireTimer = 0;
            }

        }

    }

    private void PhaseShiftRoutine()
    {
        Vector2 gunOffset = Vector2.Zero;


        //wobble
        NPC.rotation = MathF.Sin(_transformDelay / 6) * MathF.PI / 4;
        NPC.rotation = MathHelper.Clamp(NPC.rotation, -MathHelper.Pi / 4, MathHelper.Pi / 4);



        //level out arms and legs for phase 2

        foreach (var leg in Legs)
        {
            if (leg.active)
            {
                //leg.ai[2] = leg.ai[2] < 30 ? leg.ai[2] + 0.8f : 30;
                leg.realLife = NPC.whoAmI;
                leg.noGravity = false;
            }
        }

        foreach (var arm in Arms)
        {
            if (arm.active)
            {
                arm.ai[1] += 0.8f;
                arm.realLife = NPC.whoAmI;
            }
        }




        //at last minute, spawn the gun
        if (_transformDelay < 1.1f)
        {
            GunIndex = (byte)NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + gunOffset.X),
                (int)(NPC.Center.Y + gunOffset.Y),
                ModContent.NPCType<RapidFireGun>(), ai0: NPC.whoAmI);


            foreach (var leg in Legs)
            {
                if (leg.active)
                {
                    leg.noGravity = true;
                }
            }

            _globalWalkCounter = 0;

            if (CalamityWorld.revenge || CalamityWorld.death || Main.masterMode)
            {
                ArsenalLabsNPC.MimicWulfrumAmplifier(NPC, 5000, DustID.Electric, 99999f);
            }

        }
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }


    //These all make the core have a circular hitbox in phase 2
    public override bool? CanBeHitByProjectile(Projectile projectile)
    {
        if (projectile.Distance(NPC.Center) < HitRadius && !Phase1)
            return null;
        return false;
    }

    public override bool? CanBeHitByItem(Player player, Item item)
    {


        Vector2 mod = item.Size * player.direction;
        if ((player.Center + mod).Distance(NPC.Center) < HitRadius && !Phase1)
            return null;
        return false;
    }


    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        if (target.Distance(NPC.Center) < HitRadius && !Phase1)
            return true;
        return false;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return Phase1 ? false : null;
    }

    private bool InitializeBoss()
    {
        Phase1 = true;
        NPC.direction = 1;
        Vector2 leg1Offset = new Vector2(100, -25);
        NPC.ai[0] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg1Offset.X), (int)(NPC.Center.Y + leg1Offset.Y),
            ModContent.NPCType<ArsenalMinibossArm>(), ai0: NPC.whoAmI, ai1: 0, ai3: 1);


        Arm1.ai[2] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg1Offset.X), (int)(NPC.Center.Y + leg1Offset.Y),
            ModContent.NPCType<ArsenalMinibossLeg>(), ai0: Arm1.whoAmI, ai2: 0, ai3: 1);


        Vector2 leg2Offset = new Vector2(-100, -25);
        NPC.ai[1] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg2Offset.X), (int)(NPC.Center.Y + leg2Offset.Y),
            ModContent.NPCType<ArsenalMinibossArm>(), ai0: NPC.whoAmI, ai1: 0, ai3: -1);

        Arm2.ai[2] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg2Offset.X), (int)(NPC.Center.Y + leg2Offset.Y),
            ModContent.NPCType<ArsenalMinibossLeg>(), ai0: Arm2.whoAmI, ai2: 0, ai3: -1);


        Vector2 leg3Offset = new Vector2(100, 50);
        NPC.ai[2] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg1Offset.X), (int)(NPC.Center.Y + leg3Offset.Y),
            ModContent.NPCType<ArsenalMinibossArm>(), ai0: NPC.whoAmI, ai1: 0, ai3: 2);

        Arm3.ai[2] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg1Offset.X), (int)(NPC.Center.Y + leg3Offset.Y),
            ModContent.NPCType<ArsenalMinibossLeg>(), ai0: Arm3.whoAmI, ai2: 0, ai3: 2);



        Vector2 leg4Offset = new Vector2(-100, 50);
        NPC.ai[3] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg2Offset.X), (int)(NPC.Center.Y + leg4Offset.Y),
            ModContent.NPCType<ArsenalMinibossArm>(), ai0: NPC.whoAmI, ai1: 0, ai3: -2);


        Arm4.ai[2] = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(NPC.Center.X + leg2Offset.X), (int)(NPC.Center.Y + leg4Offset.Y),
            ModContent.NPCType<ArsenalMinibossLeg>(), ai0: Arm4.whoAmI, ai2: 0, ai3: -2);

        Leg1.realLife = Arm1.whoAmI;
        Leg2.realLife = Arm1.whoAmI;
        Arm2.realLife = Arm1.whoAmI;

        Leg3.realLife = Arm3.whoAmI;
        Leg4.realLife = Arm3.whoAmI;
        Arm4.realLife = Arm3.whoAmI;

        return true;
    }


    private void MovementRoutinePhase1()
    {
        _globalWalkCounter += walkSpeed;


        Vector2 wallCheck = new Vector2(160, 0);
        NPC.rotation = MathHelper.Clamp(NPC.rotation, -MathHelper.Pi / 4, MathHelper.Pi / 4);



        //move both sets of legs
        if (NPC.direction == 1)
        {
            WalkLegs(Leg1, Leg2, Arm1, Arm2, _globalWalkCounter, new Vector2(-1.4f, 1), armSpeed: 1.6f);
            WalkLegs(Leg3, Leg4, Arm3, Arm4, _globalWalkCounter, new Vector2(0, 1), armSpeed: 1.6f);
        }
        else
        {
            WalkLegs(Leg2, Leg1, Arm2, Arm1, _globalWalkCounter, new Vector2(1.6f, 1), armSpeed: 1.6f);
            WalkLegs(Leg4, Leg3, Arm4, Arm3, _globalWalkCounter, new Vector2(0, 1), armSpeed: 1.6f);
        }


        //shake body after the stomp
        if (_globalWalkCounter is > 210 and < 390)
        {
            float period = (_globalWalkCounter - 210) / 180;
            NPC.rotation = (MathF.Sin(period * MathHelper.TwoPi * 2) * _wobble);

            _wobble = MathHelper.Lerp(.6f, .01f, period / 1f);
        }

        if (_globalWalkCounter is > 510 and < 690)
        {
            float period = (_globalWalkCounter - 510) / 180;
            NPC.rotation = (MathF.Sin(period * MathHelper.TwoPi * 2) * _wobble);

            _wobble = MathHelper.Lerp(.6f, .01f, period / 1f);
        }



        //Change target only after 1 routine is finished to prevent weird stuff
        if (_globalWalkCounter > 700)
        {

            if (TargetPlayer is not null)
            {
                if (TargetPlayer.Center.X > NPC.Center.X)
                {
                    NPC.direction = -1;

                }
                else
                {
                    NPC.direction = 1;
                }
            }

            //turn around if close enough to wall
            Point pos = (NPC.Center + wallCheck).ToTileCoordinates();
            if (Main.tile[pos].IsTileSolid())
            {

                NPC.direction = 1;
            }
            pos = (NPC.Center - wallCheck).ToTileCoordinates();

            if (Main.tile[pos].IsTileSolid())
            {

                NPC.direction = -1;
            }
            _globalWalkCounter = 0;
        }
    }


    private float _slowDownCounter = 0.02f;
    private void MovementRoutinePhase2()
    {

        //Main.NewText(NPC.velocity);
        Vector2 wallCheck = new Vector2(200, 0);
        Vector2 wallCheck2 = new Vector2(120, 0);
        _globalWalkCounter += walkSpeed / 2;


        float accelMod = ((_globalWalkCounter / 100) + 1f) * phase2Accel;

        NPC.rotation += .005f * accelMod * NPC.direction;




        //Prevents running into walls
        for (int i = 0; i < 2; i++)
        {

            int mod = i == 0 ? -1 : 1;
            Point pos = (NPC.Center + wallCheck * mod).ToTileCoordinates();
            Point pos2 = (NPC.Center + wallCheck2 * mod).ToTileCoordinates();

            if (Main.tile[pos].IsTileSolid() && _globalWalkCounter > 300)
            {
                //Main.NewText(_globalWalkCounter);
                float accelDecay = (1 - _slowDownCounter);

                _slowDownCounter *= 1.1f;



                accelDecay = MathHelper.Max(accelDecay, 0);

                accelMod *= accelDecay;

                NPC.rotation -= .005f * accelMod * NPC.direction;

            }


            //Hard stops if near enough to walls
            if (Main.tile[pos2].IsTileSolid() && _globalWalkCounter > 100)
            {
                //Main.NewText(_globalWalkCounter);
                float accelDecay = (1 - _slowDownCounter);
                _slowDownCounter *= 1.1f;

                accelMod = 0;

                NPC.rotation -= .005f * accelMod * NPC.direction;



            }


        }


        //Move towards the target
        if (TargetPlayer is not null)
        {



            int playDir = TargetPlayer.Center.X > NPC.Center.X ? 1 : -1;

            if (playDir != NPC.direction)
            {
                float accelDecay = (1 - _slowDownCounter);

                _slowDownCounter *= 1.1f;


                accelDecay = MathHelper.Max(accelDecay, 0);

                accelMod *= accelDecay;

                NPC.rotation -= .005f * accelMod * NPC.direction;

            }
        }


        //Change target position
        if (accelMod < .01f && _globalWalkCounter > 200)

        {

            _globalWalkCounter = 0;
            _slowDownCounter = 0.02f;

            NPC.direction *= -1;

            _phase2TargetPos = NPC.Center + new Vector2(2000, 0) * NPC.direction;
        }
        Vector2 vel = (_phase2TargetPos - NPC.Center).SafeNormalize(Vector2.Zero) * accelMod;

        NPC.velocity.X = vel.X;

        NPC.rotation = MathHelper.Clamp(NPC.rotation, -MathHelper.Pi / 4, MathHelper.Pi / 4);
    }


    /// <summary>
    /// Controls leg movement for phase 1
    /// </summary>
    /// <param name="leg1"></param>
    /// <param name="leg2"></param>
    /// <param name="arm1"></param>
    /// <param name="arm2"></param>
    /// <param name="walkCounter"> controls progress of movement routine</param>
    /// <param name="dir"></param>
    /// <param name="raiseSpeed">How fast legs move </param>
    /// <param name="armSpeed">How fast arms move</param>
    private void WalkLegs(NPC leg1, NPC leg2, NPC arm1, NPC arm2, float walkCounter, Vector2 dir, float raiseSpeed = .8f, float armSpeed = .8f)
    {
        if (walkCounter < 100)
        {

            leg1.noGravity = true;

            if (leg1.ai[2] > 20 && walkCounter > 20)
                leg1.ai[2] -= raiseSpeed;

        }

        if (walkCounter is > 100 and < 200)
        {
            arm1.ai[1] += armSpeed;
            leg1.ai[2] -= raiseSpeed * .3f;

            //NPC.rotation += .005f;

        }



        if (walkCounter is > 200 and < 300)
        {


            if (leg1.ai[2] < ArsenalMinibossLeg.maxLegLength - 1)
                leg1.noGravity = false;

        }




        if (walkCounter is > 300 and < 400)
        {

            leg2.noGravity = true;
            if (walkCounter > 22)
                leg2.ai[2] -= raiseSpeed;
        }

        if (walkCounter is > 400 and < 500)
        {

            arm1.ai[1] -= armSpeed;
            NPC.Center += new Vector2(dir.X, 0);

        }

        if (walkCounter is > 500 and < 600)
        {
            if (leg2.ai[2] < ArsenalMinibossLeg.maxLegLength - 1)
                leg2.noGravity = false;

        }
    }


    /// <summary>
    /// Called in Detours for layering reasons
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="screenPos"></param>
    /// <param name="drawColor"></param>
    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpriteEffects effect = SpriteEffects.None;

        int frameNum = 1; //remove if this is to be animated

        var texture = Phase1 ? tex.Value : tex2.Value;

        int currFrame = 0;



        //sets frames for spawn
        if (_spawnDelay > 0)
        {
            frameNum = 45;
            texture = spawnAnim.Value;

            currFrame = ((frameNum * spawnAnimSpeed) - _spawnDelay) / spawnAnimSpeed;
        }


        //sets frames for transition
        if (_transformDelay > 0 && _spawnDelay < 0)
        {
            frameNum = 16;
            texture = transformAnim.Value;

            currFrame = (int)(((frameNum * transAnimSpeed) - _transformDelay) / transAnimSpeed);
        }

        var glowTexture = glowTex.Value;

        Vector2 drawPos = NPC.Center - Main.screenPosition;


        Vector2 drawOrigin = new Vector2(texture.Width / 2, (texture.Height / (2 * frameNum)));
        Vector2 drawOrigin2 = new Vector2(glowTexture.Width / 2, (glowTexture.Height / (2 * frameNum)));
        if (NPC.spriteDirection == -1)
        {
            drawOrigin.X = texture.Width - drawOrigin.X;
            effect = SpriteEffects.FlipHorizontally;
        }


        int height = texture.Height / frameNum;
        int startingH = height * currFrame;
        Rectangle frame = new Rectangle(0, startingH, texture.Width, height);
        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);

        if (_spawnDelay < 1)
        {
            Main.spriteBatch.Draw(glowTexture, drawPos, frame, Color.White, NPC.rotation, drawOrigin2, NPC.scale,
                effect, 0);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        if (NPC.IsABestiaryIconDummy)
        {
            /*var texture = awakeTexture1;
            int height = texture.Height / frameNum;
            int startingH = overrideFrame % frameNum;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);
            NPC.frame = frame;*/
            return false;
        }
        else
        {
            //Draw(spriteBatch, screenPos, drawColor);

            // if (Gun is not null && Gun.active)
            // {
            //     if (Gun.ModNPC is RapidFireGun armobj) armobj.Draw(spriteBatch, screenPos, drawColor);
            // }

            return false;
        }
    }
}

