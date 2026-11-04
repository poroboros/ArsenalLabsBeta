using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace ArsenalLabsBeta.NPCs.Arsenal;

public class PacifiedRustCreep : RustCreep
{
    public override void SetDefaults()
    {
        NPC.aiStyle = NPCAIStyleID.BlazingWheel;
        NPC.damage = 0;
        NPC.width = 14;
        NPC.height = 14;
        NPC.defense = 2;
        NPC.lifeMax = 18;
        NPC.knockBackResist = 1;
        AnimationType = 0;
        NPC.lavaImmune = false;
        NPC.noGravity = false;
        NPC.noTileCollide = false;

        tileCheckOffset = new Vector2(15, 0);
        movementSpeed = 3f;
        angularVel = .002f;
        noSuperCharge = true;

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        tileCheckOffset = new Vector2(10, 0);
        //SetDefaults();
        return true;
    }
}
