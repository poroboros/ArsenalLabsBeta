using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ArsenalLabsBeta.NPCs.Arsenal;

public class BabyRustCreep : ModNPC
{
    public override void SetDefaults()
    {

        NPC.aiStyle = -1;
        NPC.damage = 0;
        NPC.width = 14;
        NPC.height = 14;
        NPC.defense = 2;
        NPC.netOffset = new Vector2(0, -2);
        NPC.lifeMax = 5;
        NPC.knockBackResist = 1;
        AnimationType = 0;
        NPC.lavaImmune = false;
        NPC.noGravity = false;
        NPC.noTileCollide = false;

    }

    private float rotationDelay = 90;
    private bool firstTime = true;
    private float speed = 5;
    private float fallSpeed = 5;
    private float angularVelocity = 0.002f;

    private float rotAmount = 0;

    //NPC.ai[0] = old rotation, used for rotation calculation
    //NPC.ai[1] = Unused
    //NPC.ai[2] = movement delay
    //NPC.ai[3] = rotation counter

    public override void AI()
    {

        if (NPC.velocity.Y != 0)
            return;
        SetDefaults();
        rotationDelay = 90;
        speed = 2;
        fallSpeed = 5;
        NPC.aiStyle = 0;
        NPC.ai[2] += 5;



        if (firstTime)
        {
            NPC.ai[1] = 1;
            NPC.rotation = 0;
            firstTime = false;
        }

        Vector2 foot = new Vector2(15, -10);

        Point pos = (NPC.Center + foot).ToTileCoordinates();

        //Dust.NewDust(pos.ToWorldCoordinates(), 5, 5, DustID.RedTorch);

        if (Main.tile[pos].IsTileSolid())
        {
            NPC.ai[1] = -1;
        }

        foot.X *= -1;
        pos = (NPC.Center + foot).ToTileCoordinates();

        //Dust.NewDust(pos.ToWorldCoordinates(), 5, 5, DustID.PurpleTorch);

        if (Main.tile[pos].IsTileSolid())
        {
            NPC.ai[1] = 1;
        }

        if (NPC.ai[2] >= rotationDelay)
        {

            if (NPC.ai[3] == 0)
            {
                NPC.ai[0] = NPC.rotation;
                NPC.ai[3] = 0;
            }

            //allows AI to run, rotating the creep
            NPC.rotation = MathHelper.Lerp(NPC.ai[0], NPC.ai[0] - MathHelper.PiOver2, NPC.ai[3]);

            NPC.ai[3] += rotAmount;

            rotAmount += angularVelocity;

            NPC.velocity = new Vector2(speed * NPC.ai[1], 0);


            if (NPC.ai[3] > 1)
            {


                NPC.ai[3] = 0;
                NPC.ai[2] = 0;
                rotAmount = 0;

                //round to nearest 90 degree interval
                int degree = -(int)MathHelper.ToDegrees(NPC.rotation);
                int mDegree = degree % 360;
                if (mDegree > -45 && mDegree < 45)
                    NPC.rotation = -MathHelper.ToRadians(0);
                if (mDegree > 45 && mDegree < 135)
                    NPC.rotation = -MathHelper.ToRadians(90);
                if (mDegree > 135 && mDegree < 180 + 45)
                    NPC.rotation = -MathHelper.ToRadians(180);
                if (mDegree > 180 + 45 && mDegree < 270)
                    NPC.rotation = -MathHelper.ToRadians(270);
                if (mDegree > 270 + 45)
                    NPC.rotation = -MathHelper.ToRadians(0);


            }

        }
        else if (NPC.ai[2] < rotationDelay - 30)
        {
            //freeze if not rotating
            NPC.velocity = Vector2.Zero;
        }
    }
}
