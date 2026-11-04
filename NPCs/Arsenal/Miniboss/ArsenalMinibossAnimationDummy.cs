using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal.Miniboss;

public class ArsenalMinibossAnimationDummy : ModNPC
{

    public override string Texture => this.GetPath().Replace(GetType().Name, "ArsenalSpawn");

    private static Asset<Texture2D> tex;

    public static int SpawnAnimSpeed = 4;
    private int _currFrame;

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
        NPC.noGravity = true;
        //NPC.DeathSound = SoundID.NPCDeath37;
        NPC.value = 100 * 16f; //Balance later
        NPC.noTileCollide = true;
        NPC.immortal = true;
    }

    public override void SetStaticDefaults()
    {
        this.HideFromBestiary();
    }

    public override void Load()
    {
        if (!Main.dedServ)
        {
            tex = ModContent.Request<Texture2D>(Texture);
        }
    }

    public override bool PreAI()
    {
        //NPC.ai[0]++;


        _currFrame = (int)(NPC.ai[0] / SpawnAnimSpeed);

        if (_currFrame > 22)
        {
            NPC.velocity.Y *= 1.02f;
            // NPC.active = false;
            // return false;
        }
        else if (_currFrame > 20)
        {
            NPC.velocity.Y += .7f;
        }
        else if (_currFrame > 15)
        {
            NPC.velocity.Y += .15f;
        }
        else
        {
            NPC.velocity.Y = -1f;
        }

        if (_currFrame < 25)
        {
            NPC.ai[0]++;
        }
        return false;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy)
        {
            return false;
        }
        SpriteEffects effect = SpriteEffects.None;

        int frameNum = 45; //remove if this is to be animated

        var texture = tex.Value;




        Vector2 drawPos = NPC.Center - Main.screenPosition;


        Vector2 drawOrigin = new Vector2(texture.Width / 2, (texture.Height / (2 * frameNum)));
        if (NPC.spriteDirection == -1)
        {
            drawOrigin.X = texture.Width - drawOrigin.X;
            effect = SpriteEffects.FlipHorizontally;
        }


        int height = texture.Height / frameNum;
        int startingH = height * _currFrame;
        Rectangle frame = new Rectangle(0, startingH, texture.Width, height);
        Main.spriteBatch.Draw(texture, drawPos, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, effect, 0);

        return false;

    }
}