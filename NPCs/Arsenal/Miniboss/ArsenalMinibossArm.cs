using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal.Miniboss;

public class ArsenalMinibossArm : ModNPC
{
    public override string Texture => this.GetPath().Replace("ArsenalMinibossArm", "Arm");



    //AI 0 references the boss core
    public NPC Core => Main.npc[(int)(NPC.ai[0])];


    //AI 1 controls the extension of the arm
    public float ArmExtension => NPC.ai[1];



    //AI 3 controls which leg it is
    //If ai 3 is positive, it's on the left side
    //If abs val of ai 3 is less than 2, its on the bottom

    public bool IsOnLeft => NPC.ai[3] > 0;
    public bool IsBelow => (int)(MathF.Abs((NPC.ai[3]))) < 2;


    public Vector2 CorePos => Core.Center;
    public float RotToCore => (NPC.Center - Core.Center).ToRotation();


    public bool phase1 = true;

    private static Asset<Texture2D> tex;
    private static Asset<Texture2D> glowTex;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            tex = ModContent.Request<Texture2D>(Texture);
            glowTex = ModContent.Request<Texture2D>(Texture + "Glow");
            //glowTex = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
    }

    public override void SetStaticDefaults()
    {
        this.HideFromBestiary();
        base.SetStaticDefaults();
    }

    private int immuneTime = 120;

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 35; //Balance later
        NPC.width = 54;
        NPC.height = 54;
        NPC.defense = 10; //Balance later
        NPC.lifeMax = 350; //Balance later
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.ShowNameOnHover = false;


        NPC.alpha = 150;

        AIType = -1;


        for (int k = 0; k < NPC.buffImmune.Length; k++) //Balancer later this means immune to EVERYTHNG
        {
            NPC.buffImmune[k] = true;
        }
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit4;
        //NPC.DeathSound = SoundID.NPCDeath37;
        NPC.value = 100 * 16f; //Balance later
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {

        if (phase1 && NPC.realLife < 0)
        {
            position += new Vector2(50, 0);
        }
        return phase1 && NPC.realLife < 0 ? null : false;
    }


    public override bool PreAI()
    {
        immuneTime--;

        NPC.immortal = immuneTime > 0;
        NPC.alpha = NPC.alpha > 0 ? NPC.alpha - 5 : 0;

        if (!Core.active)
            NPC.active = false;
        else
        {
            ArsenalJuggernaut coreNPC = Core.ModNPC as ArsenalJuggernaut;
            phase1 = coreNPC.Phase1;
        }

        //NPC.immortal = !phase1;

        return true;
    }


    public override void AI()
    {





        if (NPC.alpha < 2)
        {
            NPC.ai[1] = MathHelper.Clamp(NPC.ai[1], 10, 75);
        }
        else
        {
            NPC.ai[1] = NPC.ai[1] < 30 ? NPC.ai[1] + 1.2f : 30;
        }

        Vector2 armVector = new Vector2(ArmExtension * (IsOnLeft ? -1 : 1), 0).RotatedBy((MathF.PI / 12) * (IsOnLeft ? -1 : 1));

        if (!IsBelow)
        {
            armVector = new Vector2(ArmExtension * (IsOnLeft ? -1 : 1), 0).RotatedBy((MathF.PI / 12) * (IsOnLeft ? 1 : -1));
        }

        Vector2 baseOffset = new Vector2(0, IsBelow ? 20 : -20);


        if (NPC.noGravity)
            NPC.Center = CorePos + (armVector) + baseOffset;




    }

    public void DrawArm(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpriteEffects effect = SpriteEffects.None;

        int FRAME_NUM = 1; //remove if this is to be animated

        var texture = tex.Value;
        var glowTexture = glowTex.Value;
        if (NPC.IsABestiaryIconDummy)
        {
            /*var texture = awakeTexture1;
            int height = texture.Height / frameNum;
            int startingH = overrideFrame % frameNum;
            Rectangle frame = new Rectangle(0, startingH * height, texture.Width, height);
            NPC.frame = frame;*/
            return;
        }

        Vector2 drawPos = NPC.Center - Main.screenPosition;




        Vector2 drawOrigin = new Vector2(texture.Width / 2, (texture.Height / (2 * FRAME_NUM)));

        float rotation = 0;

        if (IsOnLeft)
        {
            rotation = MathF.PI;
            if (IsBelow)
            {
                effect = SpriteEffects.FlipVertically;
            }
        }
        else
        {
            if (!IsBelow)
            {
                effect = SpriteEffects.FlipVertically;
            }
        }


        int height = texture.Height / FRAME_NUM;
        height += 1;
        int startingH = 1;

        Color alpha = new Color(255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha, 255 - NPC.alpha);
        Rectangle frame = new Rectangle(0, 0, texture.Width, height);
        Main.EntitySpriteDraw(texture, drawPos, frame, drawColor.MultiplyRGBA(alpha), rotation, drawOrigin, NPC.scale / 2, effect, 0);
        Main.EntitySpriteDraw(glowTexture, drawPos, frame, drawColor.MultiplyRGBA(alpha), rotation, drawOrigin, NPC.scale / 2, effect, 0);
    }


    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        DrawArm(spriteBatch, screenPos, drawColor);

        return false;
    }
}
